using System.Net;

namespace ExpressMeals.Domains.Exceptions;

public sealed class BadRequestException : BaseApiException
{
    public BadRequestException(string message = "Bad Request", object additionalData = null)
        : base(message, HttpStatusCode.BadRequest, additionalData)
    {
    }
}