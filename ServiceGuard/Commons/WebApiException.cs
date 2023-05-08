

namespace ServiceGuard.Commons {

    public class WebApiException : Exception {

        public interface IResult {
            public int ResultCode { get; set; }
            public string ResultMsg { get; set; }

            public string ToString();
        }

        public Code ResultCode { get; set; }

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

        public static void BuildResultInfo(ref IResult responseData, Code code) {

            string? msg = ResultMsg(code);
            if(msg == null) {
                responseData.ResultCode = -1;
                responseData.ResultMsg = $"Undefine: {code}";
            } else {
                responseData.ResultCode = (int)code;
                responseData.ResultMsg = msg;
            }

        }

        public void Log() {
            
        }

        public enum Code {
            Success = 0,            // 請求成功
            SessionFail = 101,      // 通訊失敗(身份不匹配)
        }

        public static string? ResultMsg(Code code) {
            return code switch {
                Code.Success => "Success",
                Code.SessionFail => "Session Fail",
                _ => null,
            };
        }

    }
}
