namespace SqsExtendedClient
{
    using System;
    using Amazon.Runtime;
    using S3PayloadOffloading;

    public class ReceiptHandleFactory : IReceiptHandleFactory
    {
        public string ReceiptHandleWithEmbeddedS3Pointer(string receiptHandle, string pointer)
        {
            if (this.IsS3ReceiptHandle(receiptHandle))
            {
                throw new AmazonClientException("receiptHandle already has s3 pointer information");
            }
            var (s3BucketName, s3Key) = PayloadS3Pointer.FromJson(pointer);
            return Constants.S3_BUCKET_NAME_MARKER + s3BucketName + Constants.S3_BUCKET_NAME_MARKER +
                   Constants.S3_KEY_MARKER + s3Key + Constants.S3_KEY_MARKER +
                   receiptHandle;
        }

        public bool IsS3ReceiptHandle(string receiptHandle) =>
            receiptHandle.Contains(Constants.S3_BUCKET_NAME_MARKER) &&
            receiptHandle.Contains(Constants.S3_KEY_MARKER);

        public string GetOriginalReceiptHandle(string receiptHandle)
        {
            if (!this.IsS3ReceiptHandle(receiptHandle)) return receiptHandle;

            var firstOccurence = receiptHandle.IndexOf(Constants.S3_KEY_MARKER, StringComparison.Ordinal);
            var secondOccurence =
                receiptHandle.IndexOf(Constants.S3_KEY_MARKER, firstOccurence + 1, StringComparison.Ordinal);
            return receiptHandle.Substring(secondOccurence + Constants.S3_KEY_MARKER.Length);
        }

        public string GetMessagePointerFromReceiptHandle(string receiptHandle)
        {
            if (!this.IsS3ReceiptHandle(receiptHandle))
            {
                throw new AmazonClientException("receiptHandle does not have s3 pointer information");
            }

            var bucketName = this.GetFromReceiptHandleByMarker(receiptHandle, Constants.S3_BUCKET_NAME_MARKER);
            var keyName = this.GetFromReceiptHandleByMarker(receiptHandle, Constants.S3_KEY_MARKER);
            return new PayloadS3Pointer{S3BucketName = bucketName, S3Key = keyName}.ToJson();
        }

        private string GetFromReceiptHandleByMarker(string receiptHandle, string marker)
        {
            var firstOccurence = receiptHandle.IndexOf(marker, StringComparison.Ordinal);
            var secondOccurence = receiptHandle.IndexOf(marker, firstOccurence + 1, StringComparison.Ordinal);
            return receiptHandle.Substring(firstOccurence + marker.Length, secondOccurence);
        }
    }
}
