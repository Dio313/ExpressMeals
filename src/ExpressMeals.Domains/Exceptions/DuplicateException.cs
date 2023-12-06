using System.Net;

namespace ExpressMeals.Domains.Exceptions;

public sealed class DuplicateException : BaseApiException
{
    public DuplicateException(string message = "Duplication", object additionalData = null)
        : base(message, HttpStatusCode.Conflict, additionalData)
    {
    }
}