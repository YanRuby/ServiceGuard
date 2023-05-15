using ServiceGuard.Commons;

namespace ServiceGuard.Exceptions {

    public class WebApiException : Exception {

        public WebApiResult.Code ResultCode { get; set; }

        public WebApiException(string message) : base(message) { }

        public void Log() { }

    }
}
