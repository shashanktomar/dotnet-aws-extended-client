namespace S3PayloadOffloading
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An AWS storage service that supports saving high payload sizes.
    /// </summary>
    public interface IPayloadStore
    {
        /// <summary>
        /// Stores payload in a store that has higher payload size limit than that is supported by original payload store.
        /// </summary>
        /// <returns>
        /// A pointer that must be used to retrieve the original payload later.
        /// </returns>
        /// <exception cref="Amazon.Runtime.AmazonClientException">If any internal errors are encountered on the client side while
        /// attempting to make the request or handle the response. For example
        /// if a network connection is not available.</exception>
        /// <exception cref="Amazon.S3.AmazonS3Exception">If an error response is returned by actual PayloadStore indicating
        /// either a problem with the data in the request, or a server side issue.</exception>
        /// <param name="payload"></param>
        /// <param name="cancellationToken"></param>
        Task<string> StoreOriginalPayload(string payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// Stores payload in a store that has higher payload size limit than that is supported by original payload store.
        /// </summary>
        /// <returns>
        /// A pointer that must be used to retrieve the original payload later.
        /// </returns>
        /// <exception cref="Amazon.Runtime.AmazonClientException">If any internal errors are encountered on the client side while
        /// attempting to make the request or handle the response. For example
        /// if a network connection is not available.</exception>
        /// <exception cref="Amazon.S3.AmazonS3Exception">If an error response is returned by actual PayloadStore indicating
        /// either a problem with the data in the request, or a server side issue.</exception>
        /// <param name="payload"></param>
        /// <param name="s3Key"></param>
        /// <param name="cancellationToken"></param>
        Task<string> StoreOriginalPayload(string payload, string s3Key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the original payload using the given payloadPointer. The pointer must have been obtained using <see cref="IPayloadStore.StoreOriginalPayload(string, CancellationToken)"/>
        /// </summary>
        /// <returns>
        /// Original payload
        /// </returns>
        /// <exception cref="Amazon.Runtime.AmazonClientException">If any internal errors are encountered on the client side while
        /// attempting to make the request or handle the response. For example
        /// if a network connection is not available.</exception>
        /// <exception cref="Amazon.S3.AmazonS3Exception">If an error response is returned by actual PayloadStore indicating a server side issue.</exception>
        /// <param name="payloadPointer">The pointer must have been obtained using <see cref="IPayloadStore.StoreOriginalPayload(string, CancellationToken)"/></param>
        /// <param name="cancellationToken"></param>
        Task<string> GetOriginalPayload(string payloadPointer, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes the original payload using the given payloadPointer. The pointer must have been obtained using <see cref="IPayloadStore.StoreOriginalPayload(string, CancellationToken)"/>
        /// </summary>
        /// <exception cref="Amazon.Runtime.AmazonClientException">If any internal errors are encountered on the client side while
        /// attempting to make the request or handle the response. For example
        /// if a network connection is not available.</exception>
        /// <exception cref="Amazon.S3.AmazonS3Exception">If an error response is returned by actual PayloadStore indicating a server side issue.</exception>
        /// <param name="payloadPointer">The pointer must have been obtained using <see cref="IPayloadStore.StoreOriginalPayload(string, CancellationToken)"/></param>
        /// <param name="cancellationToken"></param>
        Task DeleteOriginalPayload(string payloadPointer, CancellationToken cancellationToken = default);
    }
}
