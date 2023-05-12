namespace ServiceGuard.Exceptions {

    public class WebApiException : Exception {

        public Result.Code ResultCode { get; set; }

        public WebApiException(string message) : base(message) { }

        public void Log() { }

    }
}
