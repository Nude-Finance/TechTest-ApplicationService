namespace Services.Common.Abstractions.Model;

public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        Error = error;
        IsSuccess = isSuccess;
    }
    
    public Error Error { get; }
    public bool IsSuccess { get; }
    
    public static Result Success() => new(true, Error.Empty);

    public static Result<T> Success<T>(T value) => new(true, Error.Empty, value);

    public static Result Failure(Error error) => new(false, error);
    
    public static Result<T> Failure<T>(Error error) => new(false, error, default);
}

public class Result<TValue>(bool isSuccess, Error error, TValue value) : Result(isSuccess, error)
{
    public TValue Value { get; } = value;
}