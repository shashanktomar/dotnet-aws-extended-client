namespace SqsExtendedClient
{
    using System.Collections.Generic;
    using System.Linq;
    using Amazon.SQS.Model;

    public static class Extensions
    {
        public static int BytesSize(this string? value) =>
            value == null ? 0 : System.Text.Encoding.UTF8.GetByteCount(value);

        public static int GetMessageAttributesSize(this Dictionary<string, MessageAttributeValue> attributes) =>
            attributes.Select(attributePair =>
            {
                var (key, value) = attributePair;
                var keySize = key.BytesSize();
                if (value == null)
                {
                    return keySize;
                }

                var valueSize = value.StringValue.BytesSize() +
                                value.DataType.BytesSize() +
                                (value.BinaryValue?.ToArray().Length ?? 0);
                return keySize + valueSize;
            }).Sum();

        public static bool IsLarge(this SendMessageRequest request, int limit)
        {
            var contentSize = request.MessageBody.BytesSize();
            var attributesSize = request.MessageAttributes.GetMessageAttributesSize();
            return (contentSize + attributesSize) > limit;
        }

        public static bool IsLarge(this SendMessageBatchRequestEntry request, int limit)
        {
            var contentSize = request.MessageBody.BytesSize();
            var attributesSize = request.MessageAttributes.GetMessageAttributesSize();
            return (contentSize + attributesSize) > limit;
        }

        public static void AddUnique<T>(this List<T> list, T value)
        {
            list.Remove(value);
            list.Add(value);
        }
    }
}
