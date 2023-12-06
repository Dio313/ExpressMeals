namespace ExpressMeals.Contracts.Wrappers;

public class ApiResponse
{
    public bool IsSuccess { get; set; }

    public List<string> ErrorMessages { get; set; }

    public ApiResponse(bool isSuccess, List<string> errorMessages)
    {
        IsSuccess = isSuccess;
        ErrorMessages = errorMessages;
    }
}

public class ApiResponse<TData> : ApiResponse
{
    public TData Data { get; set; }

    public ApiResponse(bool isSuccess, List<string> errorMessages, TData data)
       : base(isSuccess, errorMessages)
    {
        Data = data;
    }
}
