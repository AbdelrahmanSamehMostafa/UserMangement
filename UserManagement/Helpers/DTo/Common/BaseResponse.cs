namespace com.gbg.modules.utility.Helpers.DTo.Common
{
    public class BaseResponse<T>
    {
        public bool isSuccess { get; set; }
        public int statusCode { get; set; }
        public Dictionary<string, List<string>> validationErrors { get; set; } = new Dictionary<string, List<string>>();
        public string message { get; set; } = "";
        public T data { get; set; }
    }
}