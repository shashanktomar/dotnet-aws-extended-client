namespace SqsExtendedClient
{
    public interface IReceiptHandleFactory
    {
        string ReceiptHandleWithEmbeddedS3Pointer(string receiptHandle, string pointer);
        bool IsS3ReceiptHandle(string receiptHandle);
        string GetOriginalReceiptHandle(string receiptHandle);

        public string GetMessagePointerFromReceiptHandle(string receiptHandle);

    }
}
