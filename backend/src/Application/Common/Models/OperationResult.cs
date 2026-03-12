namespace backend.src.Application.Common.Models;

public class OperationResult
{
    public bool IsSuccess { get; protected init; }
    public string Message { get; protected init; } = string.Empty;
    public int StatusCode { get; protected init; }

    public static OperationResult Success(string message) => new()
    {
        IsSuccess = true,
        Message = message,
        StatusCode = StatusCodes.Status200OK
    };

    public static OperationResult Failure(string message, int statusCode) => new()
    {
        IsSuccess = false,
        Message = message,
        StatusCode = statusCode
    };
}

public sealed class OperationResult<T> : OperationResult
{
    public T? Data { get; private init; }

    public static OperationResult<T> Success(T data, string message, int statusCode = StatusCodes.Status200OK) => new()
    {
        IsSuccess = true,
        Data = data,
        Message = message,
        StatusCode = statusCode
    };

    public new static OperationResult<T> Failure(string message, int statusCode) => new()
    {
        IsSuccess = false,
        Message = message,
        StatusCode = statusCode
    };
}
