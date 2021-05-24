namespace S3PayloadOffloading
{
    using Amazon.S3;
    using Amazon.S3.Model;

    public class AwsManagedCmk : ServerSideEncryptionStrategy
    {
        public void Decorate(PutObjectRequest putObjectRequest) => putObjectRequest.ServerSideEncryptionMethod = ServerSideEncryptionMethod.AWSKMS;
    }
}
