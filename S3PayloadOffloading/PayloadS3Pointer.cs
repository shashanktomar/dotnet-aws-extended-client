namespace S3PayloadOffloading
{
    using Amazon.Runtime;
    using Newtonsoft.Json;

    public class PayloadS3Pointer
    {
        [JsonProperty("s3BucketName")] public string S3BucketName { get; set; } = null!;

        [JsonProperty("s3Key")] public string S3Key { get; set; } = null!;

        public void Deconstruct(out string S3BucketName, out string S3Key)
        {
            S3BucketName = this.S3BucketName;
            S3Key = this.S3Key;
        }

        public string ToJson() => JsonConvert.SerializeObject(this);

        public static PayloadS3Pointer FromJson(string json)
        {
            var errorMsg = $"Failed to read the S3 object pointer from given string {json}";
            try
            {
                var pointer = JsonConvert.DeserializeObject<PayloadS3Pointer>(json);
                if (pointer == null)
                {
                    throw new AmazonClientException(errorMsg);
                }

                return pointer;
            }
            catch (JsonSerializationException ex)
            {
                throw new AmazonClientException(errorMsg, ex);
            }
        }
    }
}
