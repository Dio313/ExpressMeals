using System.Net;

namespace ExpressMeals.Domains.Exceptions;

public sealed class AuthenticationException : BaseApiException
{
    public AuthenticationException(string message = "Authenticate failure.", object additionalData = null)
      : base(message, HttpStatusCode.Unauthorized,  additionalData)
    {
    }
}