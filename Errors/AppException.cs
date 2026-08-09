public class AppException : Exception
{
    public int StatusCode { get; set; }
    public AppException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(StatusCodes.Status404NotFound, message)
    {
    }
}

public class DuplicateException : AppException
{
    public DuplicateException(string message) : base(StatusCodes.Status409Conflict, message)
    {
    }
}

