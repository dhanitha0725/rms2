namespace Domain.Common
{
    public class Error
    {
        public string Message { get; }

        public Error(string message)
        {
         
            Message = message;
        }

        public override string ToString() => $"{Message}";
    }
}
