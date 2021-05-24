namespace S3PayloadOffloading.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.S3;
    using Moq;
    using Xunit;

    public class S3BackedPayloadStoreTest
    {
        private readonly Mock<IAmazonS3> s3Client = new Mock<IAmazonS3>();
        private const string bucketName = "bucketName";
        private const string bucketKey = "bucketKey";
        private const string payload = "payload";

        private readonly Mock<S3Dao> s3Dao;

        public S3BackedPayloadStoreTest() =>
            this.s3Dao = new Mock<S3Dao>(MockBehavior.Default, this.s3Client.Object, null, null, null);

        [Fact]
        public async Task StoreOriginalPayload_ShouldStoreThePayloadWithGivenKey()
        {
            var s3BackedPayloadStore = new S3BackedPayloadStore(bucketName, this.s3Dao.Object);

            var payloadPointer = await s3BackedPayloadStore.StoreOriginalPayload(payload, bucketKey);

            this.s3Dao.Verify(dao => dao.StoreTextInS3(bucketName, bucketKey, payload, It.IsAny<CancellationToken>()));
            Assert.Equal($@"{{""s3BucketName"":""{bucketName}"",""s3Key"":""{bucketKey}""}}", payloadPointer);
        }

        [Fact]
        public async Task StoreOriginalPayload_ShouldStoreThePayloadWithKeyGeneratedByGuidKeyFactoryIfFactoryNotProvided()
        {
            var s3BackedPayloadStore = new S3BackedPayloadStore(bucketName, this.s3Dao.Object);
            var keyCapture = "";
            this.s3Dao.Setup(dao =>
                    dao.StoreTextInS3(bucketName, It.IsAny<string>(), payload, It.IsAny<CancellationToken>()))
                .Callback<string, string, string, CancellationToken>((name, key, payload, token) =>
                {
                    keyCapture = key;
                });

            var payloadPointer = await s3BackedPayloadStore.StoreOriginalPayload(payload);

            this.s3Dao.Verify(dao =>
                dao.StoreTextInS3(bucketName, It.IsAny<string>(), payload, It.IsAny<CancellationToken>()));
            Assert.Equal($@"{{""s3BucketName"":""{bucketName}"",""s3Key"":""{keyCapture}""}}", payloadPointer);
        }

        [Fact]
        public async Task StoreOriginalPayload_ShouldStoreThePayloadWithKeyGeneratedByProvidedKeyFactory()
        {

            var s3BackedPayloadStore = new S3BackedPayloadStore(bucketName, this.s3Dao.Object, new TestS3KeyFactory());
            var payloadPointer = await s3BackedPayloadStore.StoreOriginalPayload(payload);

            this.s3Dao.Verify(dao =>
                dao.StoreTextInS3(bucketName, It.IsAny<string>(), payload, It.IsAny<CancellationToken>()));
            Assert.Equal($@"{{""s3BucketName"":""{bucketName}"",""s3Key"":""testKey""}}", payloadPointer);
        }

        [Fact]
        public async Task GetOriginalPayload_ShouldGetThePayloadForGivenPointer()
        {
            var s3BackedPayloadStore = new S3BackedPayloadStore(bucketName, this.s3Dao.Object);
            this.s3Dao.Setup(dao => dao.GetTextFromS3(bucketName, bucketKey, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(S3BackedPayloadStoreTest.payload));

            var result =
                await s3BackedPayloadStore.GetOriginalPayload(
                    $@"{{""s3BucketName"":""{bucketName}"",""s3Key"":""{bucketKey}""}}");

            Assert.Equal(payload, result);
        }

        [Fact]
        public async Task DeleteOriginalPayload_ShouldDeleteTheGivenPointer()
        {
            var s3BackedPayloadStore = new S3BackedPayloadStore(bucketName, this.s3Dao.Object);

            await s3BackedPayloadStore.DeleteOriginalPayload(
                $@"{{""s3BucketName"":""{bucketName}"",""s3Key"":""{bucketKey}""}}");

            this.s3Dao.Verify(dao => dao.DeletePayloadFromS3(bucketName, bucketKey, It.IsAny<CancellationToken>()));
        }

        private class TestS3KeyFactory: IS3KeyFactory
        {
            public string GenerateKeyName() => "testKey";
        }
    }
}
