using System.Net;
namespace BuildingBlocks.Domain.Exceptions
{
    public class NotFoundException(string message = "Resource not found.") 
        : BaseException(message, HttpStatusCode.NotFound)
    {
    }
}
