namespace SqsExtendedClient
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.Runtime;
    using Amazon.SQS;
    using Amazon.SQS.Model;

    public abstract class SqsExtendedClientBase : IAmazonSQS
    {
        private readonly IAmazonSQS amazonSqs;

        protected SqsExtendedClientBase(IAmazonSQS amazonSqs) => this.amazonSqs = amazonSqs;

        public void Dispose() => this.amazonSqs.Dispose();

        public Task<Dictionary<string, string>> GetAttributesAsync(string queueUrl) =>
            this.amazonSqs.GetAttributesAsync(queueUrl);

        public Task SetAttributesAsync(string queueUrl, Dictionary<string, string> attributes) =>
            this.amazonSqs.SetAttributesAsync(queueUrl, attributes);

        public IClientConfig Config => this.amazonSqs.Config;

        public Task<string> AuthorizeS3ToSendMessageAsync(string queueUrl, string bucket) =>
            this.amazonSqs.AuthorizeS3ToSendMessageAsync(queueUrl, bucket);

        public Task<AddPermissionResponse> AddPermissionAsync(string queueUrl, string label, List<string> awsAccountIds,
            List<string> actions,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.AddPermissionAsync(queueUrl, label, awsAccountIds, actions, cancellationToken);

        public Task<AddPermissionResponse> AddPermissionAsync(AddPermissionRequest request,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.AddPermissionAsync(request, cancellationToken);

        public virtual Task<ChangeMessageVisibilityResponse> ChangeMessageVisibilityAsync(string queueUrl, string receiptHandle,
            int visibilityTimeout,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.ChangeMessageVisibilityAsync(queueUrl, receiptHandle, visibilityTimeout, cancellationToken);

        public virtual Task<ChangeMessageVisibilityResponse> ChangeMessageVisibilityAsync(
            ChangeMessageVisibilityRequest request,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.ChangeMessageVisibilityAsync(request, cancellationToken);

        public virtual Task<ChangeMessageVisibilityBatchResponse> ChangeMessageVisibilityBatchAsync(string queueUrl,
            List<ChangeMessageVisibilityBatchRequestEntry> entries,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.ChangeMessageVisibilityBatchAsync(queueUrl, entries, cancellationToken);

        public virtual Task<ChangeMessageVisibilityBatchResponse> ChangeMessageVisibilityBatchAsync(
            ChangeMessageVisibilityBatchRequest request,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.ChangeMessageVisibilityBatchAsync(request, cancellationToken);

        public Task<CreateQueueResponse> CreateQueueAsync(string queueName,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.CreateQueueAsync(queueName, cancellationToken);

        public Task<CreateQueueResponse> CreateQueueAsync(CreateQueueRequest request,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.CreateQueueAsync(request, cancellationToken);

        public virtual Task<DeleteMessageResponse> DeleteMessageAsync(string queueUrl, string receiptHandle,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.DeleteMessageAsync(queueUrl, receiptHandle, cancellationToken);

        public virtual Task<DeleteMessageResponse> DeleteMessageAsync(DeleteMessageRequest request,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.DeleteMessageAsync(request, cancellationToken);

        public virtual Task<DeleteMessageBatchResponse> DeleteMessageBatchAsync(string queueUrl,
            List<DeleteMessageBatchRequestEntry> entries,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.DeleteMessageBatchAsync(queueUrl, entries, cancellationToken);

        public virtual Task<DeleteMessageBatchResponse> DeleteMessageBatchAsync(DeleteMessageBatchRequest request,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.DeleteMessageBatchAsync(request, cancellationToken);

        public Task<DeleteQueueResponse> DeleteQueueAsync(string queueUrl,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.DeleteQueueAsync(queueUrl, cancellationToken);

        public Task<DeleteQueueResponse> DeleteQueueAsync(DeleteQueueRequest request,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.DeleteQueueAsync(request, cancellationToken);

        public Task<GetQueueAttributesResponse> GetQueueAttributesAsync(string queueUrl, List<string> attributeNames,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.GetQueueAttributesAsync(queueUrl, attributeNames, cancellationToken);

        public Task<GetQueueAttributesResponse> GetQueueAttributesAsync(GetQueueAttributesRequest request,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.GetQueueAttributesAsync(request, cancellationToken);

        public Task<GetQueueUrlResponse> GetQueueUrlAsync(string queueName,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.GetQueueUrlAsync(queueName, cancellationToken);

        public Task<GetQueueUrlResponse> GetQueueUrlAsync(GetQueueUrlRequest request,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.GetQueueUrlAsync(request, cancellationToken);

        public Task<ListDeadLetterSourceQueuesResponse> ListDeadLetterSourceQueuesAsync(
            ListDeadLetterSourceQueuesRequest request,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.ListDeadLetterSourceQueuesAsync(request, cancellationToken);

        public Task<ListQueuesResponse> ListQueuesAsync(string queueNamePrefix,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.ListQueuesAsync(queueNamePrefix, cancellationToken);

        public Task<ListQueuesResponse> ListQueuesAsync(ListQueuesRequest request,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.ListQueuesAsync(request, cancellationToken);

        public Task<ListQueueTagsResponse> ListQueueTagsAsync(ListQueueTagsRequest request,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.ListQueueTagsAsync(request, cancellationToken);

        public Task<PurgeQueueResponse> PurgeQueueAsync(string queueUrl,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.PurgeQueueAsync(queueUrl, cancellationToken);

        public Task<PurgeQueueResponse> PurgeQueueAsync(PurgeQueueRequest request,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.PurgeQueueAsync(request, cancellationToken);

        public virtual Task<ReceiveMessageResponse> ReceiveMessageAsync(string queueUrl,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.ReceiveMessageAsync(queueUrl, cancellationToken);

        public virtual Task<ReceiveMessageResponse> ReceiveMessageAsync(ReceiveMessageRequest request,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.ReceiveMessageAsync(request, cancellationToken);

        public Task<RemovePermissionResponse> RemovePermissionAsync(string queueUrl, string label,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.RemovePermissionAsync(queueUrl, label, cancellationToken);

        public Task<RemovePermissionResponse> RemovePermissionAsync(RemovePermissionRequest request,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.RemovePermissionAsync(request, cancellationToken);

        public virtual Task<SendMessageResponse> SendMessageAsync(string queueUrl, string messageBody,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.SendMessageAsync(queueUrl, messageBody, cancellationToken);

        public virtual Task<SendMessageResponse> SendMessageAsync(SendMessageRequest request,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.SendMessageAsync(request, cancellationToken);

        public virtual Task<SendMessageBatchResponse> SendMessageBatchAsync(string queueUrl,
            List<SendMessageBatchRequestEntry> entries,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.SendMessageBatchAsync(queueUrl, entries, cancellationToken);

        public virtual Task<SendMessageBatchResponse> SendMessageBatchAsync(SendMessageBatchRequest request,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.SendMessageBatchAsync(request, cancellationToken);

        public Task<SetQueueAttributesResponse> SetQueueAttributesAsync(string queueUrl,
            Dictionary<string, string> attributes,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.SetQueueAttributesAsync(queueUrl, attributes, cancellationToken);

        public Task<SetQueueAttributesResponse> SetQueueAttributesAsync(SetQueueAttributesRequest request,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.SetQueueAttributesAsync(request, cancellationToken);

        public Task<TagQueueResponse> TagQueueAsync(TagQueueRequest request,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.TagQueueAsync(request, cancellationToken);

        public Task<UntagQueueResponse> UntagQueueAsync(UntagQueueRequest request,
            CancellationToken cancellationToken = new CancellationToken()) =>
            this.amazonSqs.UntagQueueAsync(request, cancellationToken);

        public ISQSPaginatorFactory Paginators => this.amazonSqs.Paginators;
    }
}
