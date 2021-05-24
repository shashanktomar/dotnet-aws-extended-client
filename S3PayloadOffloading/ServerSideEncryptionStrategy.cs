namespace S3PayloadOffloading
{
    using Amazon.S3.Model;

    public interface ServerSideEncryptionStrategy
    {
        void Decorate(PutObjectRequest putObjectRequest);
    }
}
