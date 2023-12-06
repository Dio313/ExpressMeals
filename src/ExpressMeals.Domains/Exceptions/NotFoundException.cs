using System.Net;

namespace ExpressMeals.Domains.Exceptions;

public sealed class NotFoundException : BaseApiException
{
    public NotFoundException(string message = "Not found", object additionalData = null)
       : base(message, HttpStatusCode.NotFound, additionalData)
    {
    }
}
