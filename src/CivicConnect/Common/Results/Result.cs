namespace CivicConnect.Common
{
    /// <summary>
    /// Outcome of a service call that can fail for business reasons.
    /// Services return Result instead of throwing for expected failures (validation, forbidden, invalid transition).
    /// This class is COMPLETE - do not change it.
    /// </summary>
    public class Result
    {
        protected Result(bool succeeded, string? errorCode, string? errorMessage)
        {
            Succeeded = succeeded;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public bool Succeeded { get; }
        public bool Failed => !Succeeded;
        public string? ErrorCode { get; }
        public string? ErrorMessage { get; }

        public static Result Success() => new Result(true, null, null);
        public static Result Failure(string errorCode, string errorMessage) => new Result(false, errorCode, errorMessage);
    }

    /// <summary>Result that carries a value on success. COMPLETE - do not change.</summary>
    public class Result<T> : Result
    {
        private Result(bool succeeded, T? value, string? errorCode, string? errorMessage)
            : base(succeeded, errorCode, errorMessage)
        {
            Value = value;
        }

        public T? Value { get; }

        public static Result<T> Success(T value) => new Result<T>(true, value, null, null);
        public new static Result<T> Failure(string errorCode, string errorMessage) => new Result<T>(false, default, errorCode, errorMessage);
    }
}
