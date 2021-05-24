namespace S3PayloadOffloading.Tests
{
    using Amazon.S3;
    using Amazon.S3.Model;
    using Xunit;

    public class AwsManagedCmkTest
    {
        [Fact]
        public void ShouldSetCorrectEncryptionValue()
        {
            var awsManagedCmk = new AwsManagedCmk();

            var putObjectRequest = new PutObjectRequest();
            awsManagedCmk.Decorate(putObjectRequest);

            Assert.Equal(ServerSideEncryptionMethod.AWSKMS, putObjectRequest.ServerSideEncryptionMethod);
        }
    }
}
