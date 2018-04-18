namespace SampleMVC.Protocol.Model.Results
{
    public class ServiceResult : IResult
    {
        public bool IsSuccess { get; set; }

        public bool IsFailure { get; set; }

        public string Message { get; set; }

        public static ServiceResult Success()
        {
            return new ServiceResult { IsSuccess = true };
        }

        public static ServiceResult Failure(string message)
        {
            return new ServiceResult { IsFailure = true, Message = message };
        }
    }

    public class ServiceResult<TValue> : IResult
    {
        public bool IsSuccess { get; set; }

        public bool IsFailure { get; set; }

        public string Message { get; set; }

        public TValue Value { get; set; }

        public static ServiceResult<TValue> Success(TValue value)
        {
            return new ServiceResult<TValue> { IsSuccess = true, Value = value };
        }

        public static ServiceResult<TValue> Failure(string message)
        {
            return new ServiceResult<TValue> { IsFailure = true, Message = message };
        }
    }

    public class ServiceResult<TValue1, TValue2> : IResult
    {
        public bool IsSuccess { get; set; }

        public bool IsFailure { get; set; }

        public string Message { get; set; }

        public TValue1 Value1 { get; set; }

        public TValue2 Value2 { get; set; }

        public static ServiceResult<TValue1, TValue2> Success(TValue1 value1, TValue2 value2)
        {
            return new ServiceResult<TValue1, TValue2> { IsSuccess = true, Value1 = value1, Value2 = value2 };
        }

        public static ServiceResult<TValue1, TValue2> Failure(string message)
        {
            return new ServiceResult<TValue1, TValue2> { IsFailure = true, Message = message };
        }
    }
}