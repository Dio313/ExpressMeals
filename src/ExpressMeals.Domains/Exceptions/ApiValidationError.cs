using ExpressMeals.Contracts.Wrappers;

namespace ExpressMeals.Domains.Exceptions
{
    public class ApiValidationError : ApiResponse
    {
        public ApiValidationError() : base(false, new List<string>())
        {

        }
        public IEnumerable<string> Errors { get; set;} = null!;
    }
}