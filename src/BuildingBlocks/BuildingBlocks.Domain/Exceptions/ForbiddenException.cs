using System.Net;
namespace BuildingBlocks.Domain.Exceptions
{
    public class ForbiddenException(string message = "Access to the resource is forbidden.") 
        : BaseException(message, HttpStatusCode.Forbidden)
    {
    }
}
