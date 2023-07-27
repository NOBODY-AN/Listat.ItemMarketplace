using Newtonsoft.Json;

namespace Domain.Models
{
    public class ErrorResponse<T>
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("result")]
        public T? Result { get; set; }

        [JsonConstructor]
        public ErrorResponse(int code, string message, T result)
        {
            Code = code;
            Message = message;
            Result = result;
        }

        public ErrorResponse(T result)
        {
            Code = (int)Errors.OK;
            Message = Errors.OK.ToString();
            Result = result;
        }
    }

    public class ErrorResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonConstructor]
        public ErrorResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public ErrorResponse(Errors errors)
        {
            Code = (int)errors;
            Message = errors.ToString();
        }
    }
}
