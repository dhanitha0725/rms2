using System.Text.Json.Serialization;

namespace Domain.Common
{
    public class Result<T> : Result
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] // This will prevent the value from being serialized if it is the default value
        public T Value { get; private set; }

        private Result(bool isSuccess, T value, Error error) : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(true, value, null);
        }

        public static new Result<T> Failure(Error error)
        {
            return new Result<T>(false, default, error);
        }
    }
}
