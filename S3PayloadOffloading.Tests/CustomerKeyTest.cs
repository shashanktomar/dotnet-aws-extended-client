namespace S3PayloadOffloading.Tests
{
    using Amazon.S3;
    using Amazon.S3.Model;
    using Xunit;

    public class CustomerKeyTest
    {
        [Fact]
        public void ShouldSetCorrectEncryptionValueAndKey()
        {
            var customerKey = new CustomerKey("keyId");

            var putObjectRequest = new PutObjectRequest();
            customerKey.Decorate(putObjectRequest);

            Assert.Equal(ServerSideEncryptionMethod.AWSKMS, putObjectRequest.ServerSideEncryptionMethod);
            Assert.Equal("keyId", putObjectRequest.ServerSideEncryptionKeyManagementServiceKeyId);
        }
    }
}
