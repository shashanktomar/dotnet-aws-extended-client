namespace SqsExtendedClient
{
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.Lambda.SQSEvents;

    public interface ILambdaSqsEventClient
    {
        public Task FetchS3PayloadAsync(SQSEvent sqsEvent, CancellationToken cancellationToken = default);
    }
}
