﻿using System.Collections.Generic;
 using System.Linq;
 using System.Threading;
 using System.Threading.Tasks;
 using Amazon.Lambda.SQSEvents;
 using Amazon.Runtime;
 using Amazon.SQS;
 using Amazon.SQS.Model;
 using Microsoft.Extensions.Logging;
 using Microsoft.Extensions.Logging.Abstractions;
 using S3PayloadOffloading;

 namespace SqsExtendedClient
{
    public class SqsExtendedClient : SqsExtendedClientBase, ILambdaSqsEventClient
    {
        private readonly S3BackedPayloadStore? payloadStore;
        private readonly IReceiptHandleFactory receiptHandleFactory;
        private readonly ExtendedClientConfig config;
        private readonly ILogger logger;

        public SqsExtendedClient(IAmazonSQS amazonSqs,
            IReceiptHandleFactory? receiptHandleFactory = null,
            ExtendedClientConfig? extendedClientConfig = null,
            ILogger? logger = null) : base(amazonSqs)
        {
            this.config = extendedClientConfig ?? new ExtendedClientConfig();
            this.logger = logger ?? NullLogger.Instance;

            this.payloadStore = CreatePayloadStore(this.config, this.logger);
            this.receiptHandleFactory = receiptHandleFactory ?? new ReceiptHandleFactory();
        }

        public override async Task<SendMessageResponse> SendMessageAsync(SendMessageRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!this.config.S3PayloadEnabled())
            {
                return await base.SendMessageAsync(request, cancellationToken);
            }

            if (string.IsNullOrEmpty(request.MessageBody))
            {
                var msg = "messageBody cannot be null or empty";
                this.logger.LogError(msg);
                throw new AmazonClientException(msg);
            }

            await this.StoreMessageInS3(request, cancellationToken);
            return await base.SendMessageAsync(request, cancellationToken);
        }

        public override Task<SendMessageResponse> SendMessageAsync(string queueUrl, string messageBody,
            CancellationToken cancellationToken = default) =>
            this.SendMessageAsync(new SendMessageRequest(queueUrl, messageBody), cancellationToken);

        public override async Task<SendMessageBatchResponse> SendMessageBatchAsync(SendMessageBatchRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            if (!this.config.S3PayloadEnabled())
            {
                return await base.SendMessageBatchAsync(request, cancellationToken);
            }

            var tasks = request.Entries.Select(entry => this.StoreMessageInS3(entry, cancellationToken));
            await Task.WhenAll(tasks);
            return await base.SendMessageBatchAsync(request, cancellationToken);
        }

        public override Task<SendMessageBatchResponse> SendMessageBatchAsync(string queueUrl,
            List<SendMessageBatchRequestEntry> entries,
            CancellationToken cancellationToken = default) =>
            this.SendMessageBatchAsync(new SendMessageBatchRequest(queueUrl, entries), cancellationToken);

        public override async Task<ReceiveMessageResponse> ReceiveMessageAsync(ReceiveMessageRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!this.config.S3PayloadEnabled())
            {
                return await base.ReceiveMessageAsync(request, cancellationToken);
            }

            request.MessageAttributeNames.AddUnique(Constants.RESERVED_ATTRIBUTE_NAME);

            var result = await base.ReceiveMessageAsync(request, cancellationToken);
            var tasks = result.Messages.Select(message => this.FetchS3PayloadForMessage(message, cancellationToken));
            await Task.WhenAll(tasks);
            return result;
        }

        public override Task<ReceiveMessageResponse> ReceiveMessageAsync(string queueUrl,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.ReceiveMessageAsync(new ReceiveMessageRequest(queueUrl), cancellationToken);

        public async Task FetchS3PayloadAsync(SQSEvent sqsEvent,
            CancellationToken cancellationToken = default)
        {
            if (!this.config.S3PayloadEnabled())
            {
                return;
            }

            var tasks = sqsEvent.Records.Select(record => this.FetchS3PayloadForEvent(record, cancellationToken));
            await Task.WhenAll(tasks);
        }

        public override async Task<DeleteMessageResponse> DeleteMessageAsync(DeleteMessageRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            if (!this.config.S3PayloadEnabled())
            {
                return await base.DeleteMessageAsync(request, cancellationToken);
            }

            if (this.receiptHandleFactory.IsS3ReceiptHandle(request.ReceiptHandle) && this.config.CleanupS3Payload)
            {
                var payloadPointer =
                    this.receiptHandleFactory.GetMessagePointerFromReceiptHandle(request.ReceiptHandle);
                await this.payloadStore!.DeleteOriginalPayload(payloadPointer, cancellationToken);
            }

            request.ReceiptHandle = this.receiptHandleFactory.GetOriginalReceiptHandle(request.ReceiptHandle);
            return await base.DeleteMessageAsync(request, cancellationToken);
        }

        public override Task<DeleteMessageResponse> DeleteMessageAsync(string queueUrl, string receiptHandle,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.DeleteMessageAsync(new DeleteMessageRequest {QueueUrl = queueUrl, ReceiptHandle = receiptHandle},
                cancellationToken);

        public override async Task<DeleteMessageBatchResponse> DeleteMessageBatchAsync(
            DeleteMessageBatchRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            if (!this.config.S3PayloadEnabled())
            {
                return await base.DeleteMessageBatchAsync(request, cancellationToken);
            }

            var tasks = request.Entries.Select(entry => this.DeleteEntry(entry, cancellationToken));
            await Task.WhenAll(tasks);
            return await base.DeleteMessageBatchAsync(request, cancellationToken);
        }

        public override Task<DeleteMessageBatchResponse> DeleteMessageBatchAsync(string queueUrl,
            List<DeleteMessageBatchRequestEntry> entries,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.DeleteMessageBatchAsync(new DeleteMessageBatchRequest {QueueUrl = queueUrl, Entries = entries},
                cancellationToken);

        public override Task<ChangeMessageVisibilityResponse> ChangeMessageVisibilityAsync(
            ChangeMessageVisibilityRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            request.ReceiptHandle = this.receiptHandleFactory.GetOriginalReceiptHandle(request.ReceiptHandle);
            return base.ChangeMessageVisibilityAsync(request, cancellationToken);
        }

        public override Task<ChangeMessageVisibilityResponse> ChangeMessageVisibilityAsync(string queueUrl,
            string receiptHandle, int visibilityTimeout,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.ChangeMessageVisibilityAsync(
                new ChangeMessageVisibilityRequest {QueueUrl = queueUrl, ReceiptHandle = receiptHandle},
                cancellationToken);

        public override Task<ChangeMessageVisibilityBatchResponse> ChangeMessageVisibilityBatchAsync(
            ChangeMessageVisibilityBatchRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in request.Entries)
            {
                entry.ReceiptHandle = this.receiptHandleFactory.GetOriginalReceiptHandle(entry.ReceiptHandle);
            }

            return base.ChangeMessageVisibilityBatchAsync(request, cancellationToken);
        }

        public override Task<ChangeMessageVisibilityBatchResponse> ChangeMessageVisibilityBatchAsync(string queueUrl,
            List<ChangeMessageVisibilityBatchRequestEntry> entries,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.ChangeMessageVisibilityBatchAsync(
                new ChangeMessageVisibilityBatchRequest {QueueUrl = queueUrl, Entries = entries}, cancellationToken);

        private void ValidateMessageAttributes(Dictionary<string, MessageAttributeValue> attributes)
        {
            var messageAttributesSize = attributes.GetMessageAttributesSize();
            if (messageAttributesSize > this.config.PayloadSizeThreshold)
            {
                var msg = $"Total size of Message attributes is {messageAttributesSize}" +
                          $" bytes which is larger than the threshold of {this.config.PayloadSizeThreshold}" +
                          " Bytes. Consider including the payload in the message body instead of message attributes.";
                this.logger.LogError(msg);
                throw new AmazonClientException(msg);
            }

            if (attributes.Count > Constants.MAX_ALLOWED_ATTRIBUTES)
            {
                var msg = $"Number of message attributes [{attributes.Count}] exceeds the " +
                          $"maximum allowed for large-payload messages [{Constants.MAX_ALLOWED_ATTRIBUTES}].";
                this.logger.LogError(msg);
                throw new AmazonClientException(msg);
            }

            if (attributes.ContainsKey(Constants.RESERVED_ATTRIBUTE_NAME))
            {
                var msg =
                    $"Message attribute name {Constants.RESERVED_ATTRIBUTE_NAME} is reserved for use by SQS extended client.";
                this.logger.LogError(msg);
                throw new AmazonClientException(msg);
            }
        }

        private async Task StoreMessageInS3(SendMessageRequest request,
            CancellationToken cancellationToken)
        {
            this.ValidateMessageAttributes(request.MessageAttributes);
            if (this.config.AlwaysThroughS3 || request.IsLarge(this.config.PayloadSizeThreshold))
            {
                var contentSizeAttribute =
                    new MessageAttributeValue
                    {
                        DataType = "Number", StringValue = request.MessageBody.BytesSize().ToString()
                    };
                request.MessageAttributes.Add(Constants.RESERVED_ATTRIBUTE_NAME, contentSizeAttribute);

                request.MessageBody =
                    await this.payloadStore!.StoreOriginalPayload(request.MessageBody, cancellationToken);
            }
        }

        private async Task StoreMessageInS3(SendMessageBatchRequestEntry request,
            CancellationToken cancellationToken)
        {
            this.ValidateMessageAttributes(request.MessageAttributes);
            if (this.config.AlwaysThroughS3 || request.IsLarge(this.config.PayloadSizeThreshold))
            {
                var contentSizeAttribute =
                    new MessageAttributeValue
                    {
                        DataType = "Number", StringValue = request.MessageBody.BytesSize().ToString()
                    };
                request.MessageAttributes.Add(Constants.RESERVED_ATTRIBUTE_NAME, contentSizeAttribute);

                request.MessageBody =
                    await this.payloadStore!.StoreOriginalPayload(request.MessageBody, cancellationToken);
            }
        }

        private async Task FetchS3PayloadForMessage(Message message, CancellationToken cancellationToken)
        {
            if (!message.Attributes.ContainsKey(Constants.RESERVED_ATTRIBUTE_NAME)) return;

            var s3Pointer = message.Body;
            var originalPayload = await this.payloadStore!.GetOriginalPayload(s3Pointer, cancellationToken);
            message.Body = originalPayload;
            message.Attributes.Remove(Constants.RESERVED_ATTRIBUTE_NAME);
            message.ReceiptHandle =
                this.receiptHandleFactory.ReceiptHandleWithEmbeddedS3Pointer(message.ReceiptHandle, s3Pointer);
        }

        private async Task FetchS3PayloadForEvent(SQSEvent.SQSMessage message, CancellationToken cancellationToken)
        {
            if (!message.Attributes.ContainsKey(Constants.RESERVED_ATTRIBUTE_NAME)) return;

            var s3Pointer = message.Body;
            var originalPayload = await this.payloadStore!.GetOriginalPayload(s3Pointer, cancellationToken);
            message.Body = originalPayload;
            message.Attributes.Remove(Constants.RESERVED_ATTRIBUTE_NAME);
            message.ReceiptHandle =
                this.receiptHandleFactory.ReceiptHandleWithEmbeddedS3Pointer(message.ReceiptHandle, s3Pointer);
        }

        private async Task DeleteEntry(DeleteMessageBatchRequestEntry entry, CancellationToken cancellationToken)
        {
            if (this.receiptHandleFactory.IsS3ReceiptHandle(entry.ReceiptHandle) && this.config.CleanupS3Payload)
            {
                var payloadPointer =
                    this.receiptHandleFactory.GetMessagePointerFromReceiptHandle(entry.ReceiptHandle);
                await this.payloadStore!.DeleteOriginalPayload(payloadPointer, cancellationToken);
            }

            entry.ReceiptHandle = this.receiptHandleFactory.GetOriginalReceiptHandle(entry.ReceiptHandle);
        }

        private static S3BackedPayloadStore? CreatePayloadStore(ExtendedClientConfig extendedClientConfig,
            ILogger logger)
        {
            var s3Client = extendedClientConfig.S3;
            var bucketName = extendedClientConfig.S3BucketName;
            if (s3Client == null || bucketName == null) return null;

            var s3Dao = new S3Dao(s3Client, extendedClientConfig.ServerSideEncryptionStrategy,
                extendedClientConfig.S3CannedAcl, logger);
            return new S3BackedPayloadStore(bucketName, s3Dao, null, logger);
        }
    }
}
