namespace S3PayloadOffloading
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    ///<inheritdoc/>
    public class S3BackedPayloadStore : IPayloadStore
    {
        private readonly string bucketName;
        private readonly S3Dao s3Dao;
        private readonly IS3KeyFactory s3KeyFactory;
        private readonly ILogger logger;

        public S3BackedPayloadStore(string bucketName, S3Dao s3Dao, IS3KeyFactory? s3KeyFactory = null,
            ILogger? logger = null)
        {
            this.bucketName = bucketName;
            this.s3Dao = s3Dao;
            this.s3KeyFactory = s3KeyFactory ?? new GuidS3KeyFactory();
            this.logger = logger ?? NullLogger.Instance;
        }

        ///<inheritdoc/>
        public Task<string> StoreOriginalPayload(string payload, CancellationToken cancellationToken = default) =>
            this.StoreOriginalPayload(payload, this.s3KeyFactory.GenerateKeyName(), cancellationToken);

        ///<inheritdoc/>
        public async Task<string> StoreOriginalPayload(string payload, string s3Key,
            CancellationToken cancellationToken = default)
        {
            await this.s3Dao.StoreTextInS3(this.bucketName, s3Key, payload, cancellationToken);
            this.logger.LogInformation($"S3 object created, Bucket name: {this.bucketName}, Object key: {s3Key}");
            return new PayloadS3Pointer {S3BucketName = this.bucketName, S3Key = s3Key}.ToJson();
        }

        ///<inheritdoc/>
        public async Task<string> GetOriginalPayload(string payloadPointer,
            CancellationToken cancellationToken = default)
        {
            var (s3BucketName, s3Key) = PayloadS3Pointer.FromJson(payloadPointer);
            var payload = await this.s3Dao.GetTextFromS3(s3BucketName, s3Key, cancellationToken);
            this.logger.LogInformation($"S3 object read, Bucket: {s3BucketName}, Object key: {s3Key}");
            return payload;
        }

        ///<inheritdoc/>
        public async Task DeleteOriginalPayload(string payloadPointer, CancellationToken cancellationToken = default)
        {
            var (s3BucketName, s3Key) = PayloadS3Pointer.FromJson(payloadPointer);
            await this.s3Dao.DeletePayloadFromS3(s3BucketName, s3Key, cancellationToken);
        }
    }
}
