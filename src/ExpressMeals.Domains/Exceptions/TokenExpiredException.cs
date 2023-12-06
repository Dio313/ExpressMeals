using System.Net;

namespace ExpressMeals.Domains.Exceptions;

public sealed class TokenExpiredException : BaseApiException
{
    public TokenExpiredException(string message = "Authenticate failure.", object additionalData = null)
      : base(message, HttpStatusCode.Unauthorized, additionalData)
    {
    }
}