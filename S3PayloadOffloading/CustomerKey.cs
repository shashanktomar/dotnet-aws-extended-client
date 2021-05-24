namespace S3PayloadOffloading
{
    using Amazon.S3;
    using Amazon.S3.Model;

    public class CustomerKey : ServerSideEncryptionStrategy
    {
        private readonly string awsKmsKeyId;

        public CustomerKey(string awsKmsKeyId) => this.awsKmsKeyId = awsKmsKeyId;

        public void Decorate(PutObjectRequest putObjectRequest)
        {
            putObjectRequest.ServerSideEncryptionMethod = ServerSideEncryptionMethod.AWSKMS;
            putObjectRequest.ServerSideEncryptionKeyManagementServiceKeyId = this.awsKmsKeyId;
        }
    }
}
