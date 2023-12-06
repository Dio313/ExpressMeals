using System.Net;

namespace ExpressMeals.Domains.Exceptions;

public sealed class ForbiddenException : BaseApiException
{
    public ForbiddenException(string message = "You are unauthorized to access this resource.", object additionalData = null)
      : base(message, HttpStatusCode.Forbidden, additionalData)
    {
    }
}