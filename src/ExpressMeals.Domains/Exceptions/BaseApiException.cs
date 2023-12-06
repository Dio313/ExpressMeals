using System.Net;

namespace ExpressMeals.Domains.Exceptions;

public class BaseApiException : Exception
{
    public HttpStatusCode HttpStatusCode { get; }

   public object AdditionalData { get; set; }

    public BaseApiException(string message = null, HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError, object additionalData = null)
        : base(message)
    {
        HttpStatusCode = httpStatusCode;
        AdditionalData = additionalData;
    }
}