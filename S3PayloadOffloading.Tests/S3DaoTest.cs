namespace S3PayloadOffloading.Tests
{
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.Runtime;
    using Amazon.S3;
    using Amazon.S3.Model;
    using Moq;
    using Xunit;

    public class S3DaoTest
    {
        private readonly Mock<IAmazonS3> s3Client = new Mock<IAmazonS3>();

        [Fact]
        public async Task GetTextFromS3_ShouldReadTextFromS3()
        {

            var response = new GetObjectResponse
            {
                ResponseStream = new MemoryStream(Encoding.UTF8.GetBytes("S3 file content"))
            };
            this.s3Client.Setup(client =>
                    client.GetObjectAsync(
                        It.Is<GetObjectRequest>(req => req.BucketName == "bucketName" && req.Key == "bucketKey"),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var s3Dao = new S3Dao(this.s3Client.Object);
            var result = await s3Dao.GetTextFromS3("bucketName", "bucketKey");

            Assert.Equal("S3 file content", result);
        }

        [Fact]
        public async Task GetTextFromS3_ShouldThrowExceptionIfS3ClientThrowException()
        {
            this.s3Client.Setup(client =>
                    client.GetObjectAsync(
                        It.Is<GetObjectRequest>(req => req.BucketName == "bucketName" && req.Key == "bucketKey"),
                        It.IsAny<CancellationToken>()))
                .Throws(new AmazonClientException("original exception"));

            var s3Dao = new S3Dao(this.s3Client.Object);

            var exception =
                await Assert.ThrowsAsync<AmazonClientException>(() => s3Dao.GetTextFromS3("bucketName", "bucketKey"));
            Assert.Equal("Failed to get the S3 object bucketKey from bucketName", exception.Message);
        }

        [Fact]
        public async Task StoreTextInS3_ShouldWriteTextToS3()
        {
            var cancellationToken = new CancellationToken();
            var s3Dao = new S3Dao(this.s3Client.Object);

            await s3Dao.StoreTextInS3("bucketName", "bucketKey", "payload", cancellationToken);

            this.s3Client.Verify(
                client => client.PutObjectAsync(
                    It.Is<PutObjectRequest>(req => req.BucketName == "bucketName" && req.Key == "bucketKey"),
                    cancellationToken), Times.Once);
            this.s3Client.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task StoreTextInS3_ShouldWriteTextToS3WithCannedAcl()
        {
            var s3Dao = new S3Dao(this.s3Client.Object, null, S3CannedACL.Private);

            await s3Dao.StoreTextInS3("bucketName", "bucketKey", "payload");

            this.s3Client.Verify(
                client => client.PutObjectAsync(
                    It.Is<PutObjectRequest>(req => req.CannedACL == S3CannedACL.Private),
                    It.IsAny<CancellationToken>()), Times.Once);
            this.s3Client.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task StoreTextInS3_ShouldWriteTextToS3WithKmsKeyTypeForAwsManagedCmk()
        {
            var s3Dao = new S3Dao(this.s3Client.Object, new AwsManagedCmk());

            await s3Dao.StoreTextInS3("bucketName", "bucketKey", "payload");

            this.s3Client.Verify(
                client => client.PutObjectAsync(
                    It.Is<PutObjectRequest>(req =>
                        req.ServerSideEncryptionMethod == ServerSideEncryptionMethod.AWSKMS),
                    It.IsAny<CancellationToken>()), Times.Once);
            this.s3Client.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task StoreTextInS3_ShouldWriteTextToS3WithKmsKeyTypeAndIdForCustomerCmk()
        {
            var s3Dao = new S3Dao(this.s3Client.Object, new CustomerKey("kmsKey"));

            await s3Dao.StoreTextInS3("bucketName", "bucketKey", "payload");

            this.s3Client.Verify(
                client => client.PutObjectAsync(
                    It.Is<PutObjectRequest>(req =>
                        req.ServerSideEncryptionMethod == ServerSideEncryptionMethod.AWSKMS &&
                        req.ServerSideEncryptionKeyManagementServiceKeyId == "kmsKey"),
                    It.IsAny<CancellationToken>()), Times.Once);
            this.s3Client.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task StoreTextInS3_ShouldThrowExceptionIfS3ClientThrowException()
        {
            this.s3Client.Setup(client =>
                    client.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new AmazonClientException("original exception"));

            var s3Dao = new S3Dao(this.s3Client.Object);

            var exception =
                await Assert.ThrowsAsync<AmazonClientException>(() => s3Dao.StoreTextInS3("bucket-name", "bucketKey", "payload"));
            Assert.Equal("Failed to store the message content in an S3 object bucketKey in bucket bucket-name", exception.Message);
        }

        [Fact]
        public async Task DeletePayloadFromS3_ShouldDeleteFileFromS3()
        {
            var s3Dao = new S3Dao(this.s3Client.Object);

            await s3Dao.DeletePayloadFromS3("bucketName", "bucketKey");

            this.s3Client.Verify(
                client => client.DeleteObjectAsync(
                    It.Is<DeleteObjectRequest>(req =>
                        req.BucketName == "bucketName" &&
                        req.Key == "bucketKey"),
                    It.IsAny<CancellationToken>()), Times.Once);
            this.s3Client.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeletePayloadFromS3_ShouldThrowExceptionIfS3ClientThrowException()
        {
            this.s3Client.Setup(client =>
                    client.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new AmazonClientException("original exception"));

            var s3Dao = new S3Dao(this.s3Client.Object);

            var exception =
                await Assert.ThrowsAsync<AmazonClientException>(() => s3Dao.DeletePayloadFromS3("bucket-name", "bucketKey"));
            Assert.Equal("Failed to delete S3 object bucketKey in bucket bucket-name", exception.Message);
        }
    }
}
