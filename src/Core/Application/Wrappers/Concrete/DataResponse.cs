using Application.Wrappers.Abstract;
using JsonConstructorAttribute = Newtonsoft.Json.JsonConstructorAttribute;

namespace Application.Wrappers.Concrete
{
    public class DataResponse<T> : IDataResponse<T>
    {
        public bool Success { get; } = true;
        public T Data { get; }

        public int StatusCode { get; }
        public string Message { get; set; }

        [JsonConstructor]
        public DataResponse(T data, int statuscode)
        {
            Data = data;
            StatusCode = statuscode;
        }

        public DataResponse(T data, int statuscode, string message)
        {
            Data = data;
            StatusCode = statuscode;
            Message = message;
        }
    }
}