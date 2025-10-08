using System.Net;

namespace BuildingBlocks.Domain.Exceptions
{
    public class BaseException(string message, HttpStatusCode statusCode) : Exception(message)
    {
        public HttpStatusCode StatusCode { get; } = statusCode;
    }
}
