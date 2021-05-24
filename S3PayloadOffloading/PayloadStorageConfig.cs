namespace S3PayloadOffloading
{
    using Amazon.S3;

    /// <summary>
    /// Amazon payload storage configuration options such as Amazon S3 client,
    /// bucket name, and payload size threshold for payloads.
    /// </summary>
    /// <remarks>
    /// <para>Server side encryption is optional and can be enabled using ServerSideEncryptionStrategy</para>
    /// <para>There are two possible options for server side encryption. This can be using a customer managed key or AWS managed CMK</para>
    /// </remarks>
    public class PayloadStorageConfig
    {
        public readonly AmazonS3Client S3;
        public readonly string S3BucketName;
        /// <summary>
        /// Sets the payload size threshold for storing payloads in Amazon S3. Default: 256KB
        /// </summary>
        public readonly int PayloadSizeThreshold;
        /// <summary>
        /// Sets whether or not all payloads regardless of their size should be stored in Amazon S3
        /// </summary>
        public readonly bool AlwaysThroughS3;
        public readonly ServerSideEncryptionStrategy? serverSideEncryptionStrategy;
        public readonly S3CannedACL? s3CannedAcl;

        public PayloadStorageConfig(
            AmazonS3Client s3,
            string s3BucketName,
            int payloadSizeThreshold = 0,
            bool alwaysThroughS3 = false,
            ServerSideEncryptionStrategy? serverSideEncryptionStrategy = null,
            S3CannedACL? s3CannedAcl = null)
        {
            this.S3 = s3;
            this.S3BucketName = s3BucketName;
            this.PayloadSizeThreshold = payloadSizeThreshold;
            this.AlwaysThroughS3 = alwaysThroughS3;
            this.serverSideEncryptionStrategy = serverSideEncryptionStrategy;
            this.s3CannedAcl = s3CannedAcl;
        }
    }
}
