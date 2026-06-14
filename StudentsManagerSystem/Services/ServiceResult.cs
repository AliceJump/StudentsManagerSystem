namespace StudentsManagerSystem.Services
{
    internal sealed class ServiceResult
    {
        private ServiceResult(bool succeeded, string message)
        {
            Succeeded = succeeded;
            Message = message;
        }

        public bool Succeeded { get; }

        public string Message { get; }

        public static ServiceResult Success(string message = "操作成功") => new(true, message);

        public static ServiceResult Failure(string message) => new(false, message);
    }

    internal sealed class ServiceResult<T>
    {
        private ServiceResult(bool succeeded, string message, T? data)
        {
            Succeeded = succeeded;
            Message = message;
            Data = data;
        }

        public bool Succeeded { get; }

        public string Message { get; }

        public T? Data { get; }

        public static ServiceResult<T> Success(T data, string message = "操作成功") => new(true, message, data);

        public static ServiceResult<T> Failure(string message) => new(false, message, default);
    }
}
