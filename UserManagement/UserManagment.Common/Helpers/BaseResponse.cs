namespace UserManagment.Common.Helpers
{
   public class BaseResponse<T>
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public Dictionary<string, List<string>> ValidationErrors { get; set; } = new Dictionary<string, List<string>>();
        public string Message { get; set; }="";
        public T Data { get; set; }
    }
}