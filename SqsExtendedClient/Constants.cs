namespace SqsExtendedClient
{
    public static class Constants
    {
        // This is copied from AWS Java SQS Extended Lib
        public const int DEFAULT_MESSAGE_SIZE_THRESHOLD = 262144;

        // TODO: This version should be read from csproj file
        public const string USER_AGENT_HEADER = "SqsExtendedClient/3.7.0.24";

        public const int MAX_ALLOWED_ATTRIBUTES = 10 - 1; // 10 for SQS, 1 for the reserved attribute

        public const string RESERVED_ATTRIBUTE_NAME = "ExtendedPayloadSize";

        public const string S3_BUCKET_NAME_MARKER = "-..s3BucketName..-";
        public const string S3_KEY_MARKER = "-..s3Key..-";
    }
}
