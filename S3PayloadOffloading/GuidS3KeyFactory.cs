namespace S3PayloadOffloading
{
    using System;

    public class GuidS3KeyFactory: IS3KeyFactory
    {
        public string GenerateKeyName() => Guid.NewGuid().ToString();
    }
}
