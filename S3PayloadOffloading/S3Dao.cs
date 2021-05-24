namespace S3PayloadOffloading
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.Runtime;
    using Amazon.S3;
    using Amazon.S3.Model;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    public class S3Dao
    {
        private readonly ILogger logger;
        private readonly IAmazonS3 s3Client;
        private readonly ServerSideEncryptionStrategy? encryptionStrategy;
        private readonly S3CannedACL? s3CannedAcl;

        public S3Dao(IAmazonS3 s3Client, ServerSideEncryptionStrategy? encryptionStrategy = null,
            S3CannedACL? s3CannedAcl = null, ILogger? logger = null)
        {
            this.s3Client = s3Client;
            this.encryptionStrategy = encryptionStrategy;
            this.s3CannedAcl = s3CannedAcl;
            this.logger = logger ?? NullLogger.Instance;
        }

        public virtual async Task<string> GetTextFromS3(string bucketName, string bucketKey,
            CancellationToken cancellationToken = default)
        {
            var request = new GetObjectRequest {BucketName = bucketName, Key = bucketKey};

            GetObjectResponse response;
            try
            {
                response = await this.s3Client.GetObjectAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (!(ex is AmazonClientException) && !(ex is AmazonServiceException)) throw;

                var msg = $"Failed to get the S3 object {bucketKey} from {bucketName}";
                this.logger.LogError(ex, msg);
                throw new AmazonClientException(msg, ex);
            }

            using (response)
            {
                using var streamReader = new StreamReader(response.ResponseStream);
                try
                {
                    return await streamReader.ReadToEndAsync();
                }
                catch (IOException ex)
                {
                    var msg =
                        $"Failure when handling the message which was read from S3 object {bucketKey} from bucket {bucketName}";
                    this.logger.LogError(ex, msg);
                    throw new AmazonClientException(msg, ex);
                }
            }
        }

        public virtual async Task StoreTextInS3(string bucketName, string bucketKey, string payload,
            CancellationToken cancellationToken = default)
        {
            var request = new PutObjectRequest {BucketName = bucketName, Key = bucketKey, ContentBody = payload};
            if (this.s3CannedAcl != null)
            {
                request.CannedACL = this.s3CannedAcl;
            }

            this.encryptionStrategy?.Decorate(request);

            try
            {
                await this.s3Client.PutObjectAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (!(ex is AmazonClientException) && !(ex is AmazonServiceException)) throw;

                var msg = $"Failed to store the message content in an S3 object {bucketKey} in bucket {bucketName}";
                this.logger.LogError(ex, msg);
                throw new AmazonClientException(msg, ex);
            }
        }

        public virtual async Task DeletePayloadFromS3(string bucketName, string bucketKey,
            CancellationToken cancellationToken = default)
        {
            var deleteObjectRequest = new DeleteObjectRequest {BucketName = bucketName, Key = bucketKey};
            try
            {
                await this.s3Client.DeleteObjectAsync(deleteObjectRequest, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (!(ex is AmazonClientException) && !(ex is AmazonServiceException)) throw;

                var msg = $"Failed to delete S3 object {bucketKey} in bucket {bucketName}";
                this.logger.LogError(ex, msg);
                throw new AmazonClientException(msg, ex);
            }
        }
    }
}
