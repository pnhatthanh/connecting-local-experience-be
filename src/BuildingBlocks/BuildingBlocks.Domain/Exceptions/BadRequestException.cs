using System.Net;
namespace BuildingBlocks.Domain.Exceptions
{
    public class BadRequestException(string message = "Invalid request.") 
        : BaseException(message, HttpStatusCode.BadRequest)
    {
    }
}
