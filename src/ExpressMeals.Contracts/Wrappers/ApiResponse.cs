namespace ExpressMeals.Contracts.Wrappers;

public class ApiResponse
{
   public ApiResponse(bool isSuccess, List<string> errorMessages)
    {
        IsSuccess = isSuccess;
        ErrorMessages = errorMessages;
    }

    public bool IsSuccess { get; set; }
    public List<string> ErrorMessages { get; set; }
}

