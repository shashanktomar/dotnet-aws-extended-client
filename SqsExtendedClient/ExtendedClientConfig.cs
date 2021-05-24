namespace SqsExtendedClient
{
    using Amazon.S3;
    using S3PayloadOffloading;

    public class ExtendedClientConfig
    {
        public AmazonS3Client? S3 { get; private set; }
        public string? S3BucketName { get; private set; }

        public int PayloadSizeThreshold { get; private set; } = Constants.DEFAULT_MESSAGE_SIZE_THRESHOLD;

        public bool AlwaysThroughS3 { get; private set; }
        public bool CleanupS3Payload { get; private set; } = false;

        public ServerSideEncryptionStrategy? ServerSideEncryptionStrategy { get; private set; }
        public S3CannedACL? S3CannedAcl { get; private set; }

        public ExtendedClientConfig()
        {
        }

        public bool S3PayloadEnabled() => this.S3 != null && this.S3BucketName != null;

        public ExtendedClientConfig WithPayloadSupportEnabled(AmazonS3Client s3Client, string bucketName,
            bool cleanupS3Payload = true)
        {
            this.S3 = s3Client;
            this.S3BucketName = bucketName;
            this.CleanupS3Payload = cleanupS3Payload;
            return this;
        }

        public ExtendedClientConfig WithAlwaysThroughS3(bool alwaysThroughS3)
        {
            this.AlwaysThroughS3 = alwaysThroughS3;
            return this;
        }

        public ExtendedClientConfig WithPayloadSizeThreshold(int payloadSizeThreshold)
        {
            this.PayloadSizeThreshold = payloadSizeThreshold;
            return this;
        }

        public ExtendedClientConfig WithS3EncryptionStrategy(ServerSideEncryptionStrategy serverSideEncryptionStrategy)
        {
            this.ServerSideEncryptionStrategy = serverSideEncryptionStrategy;
            return this;
        }

        public ExtendedClientConfig WithS3CannedAcl(S3CannedACL s3CannedAcl)
        {
            this.S3CannedAcl = s3CannedAcl;
            return this;
        }
    }
}
