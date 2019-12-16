using System;

namespace ArchitectSample.Protocol.Model.Results
{
    public class ServiceResult : IResult
    {
        public ServiceResult(bool isSuccess, bool isFailure, int code, string message)
        {
            if (isSuccess && code != 0) throw new ArgumentException("Code must be zero when result is success.", nameof(code));
            if (isFailure && code != int.MinValue) throw new ArgumentException("Code must be minimum int value when result is failure.", nameof(code));

            this.IsSuccess = isSuccess;
            this.IsFailure = isFailure;
            this.Code = code;
            this.Message = message;
        }

        public bool IsSuccess { get; }

        public bool IsFailure { get; }

        public int Code { get; }

        public string Message { get; }

        public static ServiceResult Success()
        {
            return new ServiceResult(true, false, 0, string.Empty);
        }

        public static ServiceResult<TValue> Success<TValue>(TValue value)
        {
            return new ServiceResult<TValue>(true, false, 0, string.Empty, value);
        }

        public static ServiceResult<TValue1, TValue2> Success<TValue1, TValue2>(TValue1 value1, TValue2 value2)
        {
            return new ServiceResult<TValue1, TValue2>(true, false, 0, string.Empty, value1, value2);
        }

        public static ServiceResult<TValue1, TValue2, TValue3> Success<TValue1, TValue2, TValue3>(TValue1 value1, TValue2 value2, TValue3 value3)
        {
            return new ServiceResult<TValue1, TValue2, TValue3>(true, false, 0, string.Empty, value1, value2, value3);
        }

        public static ServiceResult Failure(string message)
        {
            return new ServiceResult(false, true, int.MinValue, message);
        }

        public static ServiceResult Abnormal(string message, int code)
        {
            return new ServiceResult(false, false, code, message);
        }
    }

    public class ServiceResult<TValue> : IResult
    {
        public ServiceResult(bool isSuccess, bool isFailure, int code, string message, TValue value)
        {
            if (isSuccess && code != 0) throw new ArgumentException("Code must be zero when result is success.", nameof(code));
            if (isFailure && code != int.MinValue) throw new ArgumentException("Code must be minimum int value when result is failure.", nameof(code));

            this.IsSuccess = isSuccess;
            this.IsFailure = isFailure;
            this.Code = code;
            this.Message = message;
            this.Value = value;
        }

        public bool IsSuccess { get; }

        public bool IsFailure { get; }

        public int Code { get; }

        public string Message { get; }

        public TValue Value { get; }

        public static implicit operator ServiceResult<TValue>(ServiceResult result)
        {
            return new ServiceResult<TValue>(result.IsSuccess, result.IsFailure, result.Code, result.Message, default(TValue));
        }

        public static ServiceResult<TValue> Failure(string message)
        {
            return new ServiceResult<TValue>(false, true, int.MinValue, message, default(TValue));
        }

        public void Deconstruct(out TValue value)
        {
            value = this.Value;
        }

        public void Deconstruct(out IResult result, out TValue value)
        {
            result = new ServiceResult(this.IsSuccess, this.IsFailure, this.Code, this.Message);
            value = this.Value;
        }
    }

    public class ServiceResult<TValue1, TValue2> : IResult
    {
        public ServiceResult(bool isSuccess, bool isFailure, int code, string message, TValue1 value1, TValue2 value2)
        {
            if (isSuccess && code != 0) throw new ArgumentException("Code must be zero when result is success.", nameof(code));
            if (isFailure && code != int.MinValue) throw new ArgumentException("Code must be minimum int value when result is failure.", nameof(code));

            this.IsSuccess = isSuccess;
            this.IsFailure = isFailure;
            this.Code = code;
            this.Message = message;
            this.Value1 = value1;
            this.Value2 = value2;
        }

        public bool IsSuccess { get; }

        public bool IsFailure { get; }

        public int Code { get; }

        public string Message { get; }

        public TValue1 Value1 { get; }

        public TValue2 Value2 { get; }

        public static implicit operator ServiceResult<TValue1, TValue2>(ServiceResult result)
        {
            return new ServiceResult<TValue1, TValue2>(
                result.IsSuccess,
                result.IsFailure,
                result.Code,
                result.Message,
                default(TValue1),
                default(TValue2));
        }

        public static ServiceResult<TValue1, TValue2> Failure(string message)
        {
            return new ServiceResult<TValue1, TValue2>(false, true, int.MinValue, message, default(TValue1), default(TValue2));
        }

        public void Deconstruct(out TValue1 value1, out TValue2 value2)
        {
            value1 = this.Value1;
            value2 = this.Value2;
        }

        public void Deconstruct(out IResult result, out TValue1 value1, out TValue2 value2)
        {
            result = new ServiceResult(this.IsSuccess, this.IsFailure, this.Code, this.Message);
            value1 = this.Value1;
            value2 = this.Value2;
        }
    }

    public class ServiceResult<TValue1, TValue2, TValue3> : IResult
    {
        public ServiceResult(bool isSuccess, bool isFailure, int code, string message, TValue1 value1, TValue2 value2, TValue3 value3)
        {
            if (isSuccess && code != 0) throw new ArgumentException("Code must be zero when result is success.", nameof(code));
            if (isFailure && code != int.MinValue) throw new ArgumentException("Code must be minimum int value when result is failure.", nameof(code));

            this.IsSuccess = isSuccess;
            this.IsFailure = isFailure;
            this.Code = code;
            this.Message = message;
            this.Value1 = value1;
            this.Value2 = value2;
            this.Value3 = value3;
        }

        public bool IsSuccess { get; }

        public bool IsFailure { get; }

        public int Code { get; }

        public string Message { get; }

        public TValue1 Value1 { get; }

        public TValue2 Value2 { get; }

        public TValue3 Value3 { get; }

        public static implicit operator ServiceResult<TValue1, TValue2, TValue3>(ServiceResult result)
        {
            return new ServiceResult<TValue1, TValue2, TValue3>(
                result.IsSuccess,
                result.IsFailure,
                result.Code,
                result.Message,
                default(TValue1),
                default(TValue2),
                default(TValue3));
        }

        public static ServiceResult<TValue1, TValue2, TValue3> Failure(string message)
        {
            return new ServiceResult<TValue1, TValue2, TValue3>(
                false,
                true,
                int.MinValue,
                message,
                default(TValue1),
                default(TValue2),
                default(TValue3));
        }

        public void Deconstruct(out TValue1 value1, out TValue2 value2, out TValue3 value3)
        {
            value1 = this.Value1;
            value2 = this.Value2;
            value3 = this.Value3;
        }

        public void Deconstruct(out IResult result, out TValue1 value1, out TValue2 value2, out TValue3 value3)
        {
            result = new ServiceResult(this.IsSuccess, this.IsFailure, this.Code, this.Message);
            value1 = this.Value1;
            value2 = this.Value2;
            value3 = this.Value3;
        }
    }
}