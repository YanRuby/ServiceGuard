namespace ServiceGuard.Commons {

    public interface IResult {
        public int ResultCode { get; set; }
        public string ResultMsg { get; set; }
        public string? ToString();
    }

    public static class WebApiResult {

        public static void Build(ref IResult result, Code code, string message = "") {
            string? msg = Message(code, message);
            if (msg == null) {
                result.ResultCode = (int)Code.Undefine;
                result.ResultMsg = $"Undefine Message Code: {code}";
            } else {
                result.ResultCode = (int)code;
                result.ResultMsg = msg;
            }
        }

        public static IResult Build(IResult responseData, Code code, string message = "") {
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
        public static IResult BuildSuccessInfo(IResult responseData, string msg = "") => Build(responseData, Code.Success, msg);
        public static IResult BuildFailInfo(IResult responseData, string msg = "") => Build(responseData, Code.Fail, msg);
        public static IResult BuildExceptionInfo(IResult responseData, string msg = "") => Build(responseData, Code.Exception, msg);
        public static IResult BuildTestInfo(IResult responseData, string msg = "") => Build(responseData, Code.Test, msg);
        #endregion

        public enum Code {
            #region disable Formatting
            /************************************************************
            * Defualt Code 預設
            */
             Undefine = -1,               // Undefine Message Info
              Success = 0,                // 成功
                 Fail = 1,                // 失敗
            Exception = 3,                // 發生例外問題
            /************************************************************
            * CheckFailed 檢查失敗
            */
            //              = 100,              // 保留
             CheckFailed_ValidData = 101,       // 資料有效性檢查失敗 >> [dbgMsg: DataHead|DataBody 内容不符合檢查標準]
            CheckFailed_CorsPolicy = 102,       // 跨域檢查失敗 >> [dbgMsg: DataHead 中找不到對應的跨域資訊]
            /************************************************************
            * Linq 查詢結果
            */
            //              = 200,              // 保留
            Linq_NotFound = 201,
            /************************************************************
            * Customize Code 自定義
            */


            Test = 777,              // 測試
            #endregion
        }

        private static string AddMessage(string defaultMsg, string moreMsg = "") {
            return moreMsg == "" ? defaultMsg : $"{defaultMsg}: {moreMsg}";
        }

        public static string? Message(Code code, string msg = "") {
            return code switch {
                /************************************************************
                * Defualt Code 預設
                */
                Code.Undefine           /* -1   */ => AddMessage($"Undefine Message Info", msg),
                Code.Success            /*  0   */ => AddMessage($"Success", msg),
                Code.Fail               /*  1   */ => AddMessage($"Fail", msg),
                Code.Exception          /*  3   */ => AddMessage($"Exception", msg),
                /************************************************************
                * CheckFailed
                */
                //                    Hold        /*  100 */ => $"Null", // 保留
                Code.CheckFailed_ValidData        /*  101 */ => AddMessage($"Check Failed [Valid Data]", msg),
                Code.CheckFailed_CorsPolicy       /*  101 */ => AddMessage($"Check Failed [Cors Policy]", msg),


                Code.Test               /*  777 */ => AddMessage($"Test", msg),
                _ => null, //           /*  XXX */ => AddMessage($"Undefine Message Info", msg),
            };
        }

    }

}
