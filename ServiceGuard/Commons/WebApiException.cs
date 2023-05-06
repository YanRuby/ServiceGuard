

namespace ServiceGuard.Commons {

    public class WebApiException : Exception {

        public interface IResult {
            public int ResultCode { get; set; }
            public string ResultMsg { get; set; }
        }

        public ErrorCode ErrCode { get; set; }

        public WebApiException(string message) : base(message) {

        }

        /// <summary>
        /// 構建結果訊息
        /// </summary>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public static IResult BuildResultInfo(IResult responseData) {
            responseData.ResultCode = 0;
            responseData.ResultMsg = "Test";
            return responseData;
        }

        public void Log() {
            
        }

        public enum ErrorCode {

        }

    }
}
