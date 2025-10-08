using System.Net;
namespace BuildingBlocks.Domain.Exceptions
{
    public class DuplicatedException(string message = "Resource already exists.") 
        : BaseException(message, HttpStatusCode.Conflict)
    {
    }
}
