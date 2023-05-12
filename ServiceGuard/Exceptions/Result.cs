
namespace ServiceGuard.Exceptions {

    public interface IResult {
        public int ResultCode { get; set; }
        public string ResultMsg { get; set; }
        public string? ToString();
    }

    public static class Result {

        public static IResult BuildInfo(IResult responseData, Code code, string message = "") {
            string? msg = Message(code, message);
            if (msg == null) {
                responseData.ResultCode = (int)Code.Undefine;
                responseData.ResultMsg = $"Undefine Message Code: {code}";
            } else {
                responseData.ResultCode = (int)code;
                responseData.ResultMsg = msg;
            }
            return responseData;
        }

        #region 常用的資訊建立方法
        public static IResult BuildSuccessInfo(IResult responseData, string msg = "") => BuildInfo(responseData, Code.Success, msg);
        public static IResult BuildFailInfo(IResult responseData, string msg = "") => BuildInfo(responseData, Code.Fail, msg);
        public static IResult BuildExceptionInfo(IResult responseData, string msg = "") => BuildInfo(responseData, Code.Exception, msg);
        public static IResult BuildTestInfo(IResult responseData, string msg = "") => BuildInfo(responseData, Code.Test, msg);
        #endregion

        public enum Code {
               Undefine = -1,               // Undefine Message Info
                Success = 0,                // 成功
                   Fail = 1,                // 失敗
              Exception = 3,                // 發生例外問題
            SessionFail = 101,              // 通訊失敗(身份不匹配)
                   Test = 777,              // 測試
        }

        public static string? Message(Code code, string msg = "") {
            var undefMsg = $"Message Undefine Code: {code}";
            return code switch {
                Code.Undefine           /* -1   */ => $"Undefine Message Info",
                Code.Success            /*  0   */ => $"Success",
                Code.Fail               /*  1   */ => $"Fail",
                Code.Exception          /*  3   */ => $"Exception: {msg}",
                Code.SessionFail        /*  101 */ => $"Session Fail",
                Code.Test               /*  777 */ => $"Test",
                _ => null, //           /*  XXX */ => $"Undefine Message Code: {code}",
            };
        }

    }

}
