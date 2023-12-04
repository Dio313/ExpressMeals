using ExpressMeals.Contracts.Wrappers;

namespace ExpressMeals.Domains.Exceptions;

public class ApiException : ApiResponse
{
    public ApiException(string details) : base(false, new List<string>())
    {
        Details = details;
    }

    public string Details {get; set;}
}