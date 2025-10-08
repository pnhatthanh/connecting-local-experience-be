using System.Net;

namespace BuildingBlocks.Domain.Exceptions
{
    public class UnAuthorizedException(string message = "Unauthorized.") 
        : BaseException(message, HttpStatusCode.Unauthorized)
    {
    }
}
