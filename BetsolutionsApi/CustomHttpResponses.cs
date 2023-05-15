namespace BetsolutionsApi;

public static class CustomHttpResponses
{
    public static object InactiveToken401 { get; } = new
    {
        StatusCode = 401,
        message = "Inactive token"
    };
    
    public static object InvalidHash403 { get; } = new
    {
        StatusCode = 403,
        message = "Invalid hash"
    };
    
    public static object UserNotFound406 { get; } = new
    {
        StatusCode = 406,
        message = "User not found"
    };
    
    public static object InvalidAmount407 { get; } = new
    {
        StatusCode = 407,
        message = "Invalid amount"
    };
    
    public static object InvalidRequest411 { get; } = new
    {
        StatusCode = 411,
        message = "Invalid request"
    };
    
    public static object GeneralError500 { get; } = new
    {
        StatusCode = 500,
        message = "General error"
    };
}