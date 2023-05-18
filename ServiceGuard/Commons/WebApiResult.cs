namespace ServiceGuard.Commons {

    /// <summary>
    /// 響應資訊界面
    /// </summary>
    public interface IResult {
        public int ResultCode { get; set; }
        public string ResultMsg { get; set; }
        public string? ToString();
    }

    // For 產生響應内容
    /// <summary>
    /// WebApi 響應資訊
    /// </summary>
    public static partial class WebApiResult {

        /// <summary>
        /// 響應資料模型
        /// </summary>
        /// <remarks>
        /// 用於沒有響應資訊實例，卻又需要響應資料承載進行實體創建的途徑。
        /// </remarks>
        public struct Result : IResult {
            int IResult.ResultCode { get; set; }
            string IResult.ResultMsg { get; set; }
            string IResult.ToString() {
                IResult result = this;
                return "\n" + base.ToString() + " {\n"
                    + $"  >> ResultCode: {result.ResultCode}\n"
                    + $"  >> ResultMsg: {result.ResultMsg}\n"
                    + "}\n";
            }
        }

        /// <summary>
        /// 消息格式
        /// </summary>
        public enum MsgFormatType {
            Preset = 0,     // 預設值
        }

        /// <summary>
        /// 創建 WebApi 響應資訊内容
        /// </summary>
        /// <param name="result">響應資訊實例載體</param>
        /// <param name="code">響應代碼</param>
        /// <param name="message">額外的響應消息</param>
        /// <param name="type">響應消息的格式</param>
        public static void Build(ref IResult result, Code code, string? message = null,
            MsgFormatType type = MsgFormatType.Preset) {

            // 取得預設消息内容
            string? msg = code.ToMessage();

            // 未定義的預設消息
            if(msg is null) {
                result.ResultCode = Code.Undefine.ToInt();
                result.ResultMsg = $"Undefine Message Code: {code}";
                return;
            }

            result.ResultCode = code.ToInt();       // 設定-響應代碼
            result.ResultMsg = (message is null) ?  // 設定-響應消息
                
                // 采用-預設格式
                msg

                // 采用-自定義格式
                : type switch {
                    MsgFormatType.Preset            => $"{msg}: {message}",
                    
                    // 未定義格式，直接使用預設格式
                    _ => msg,
                };

        }

    }

    // For 響應代碼定義
    public static partial class WebApiResult {
        public enum Code {
            #region disable Formatting
            /************************************************************
            * Defualt Code 預設
            */
            Undefine = -1,                  // Undefine Message Info
            Success = 0,                    // 成功
            Fail = 1,                       // 失敗
            Exception = 3,                  // 發生例外問題
            NotFound = 4,                   // 找不到
            InvalidData = 5,                // 無效資料
            /************************************************************
            * CheckFailed 檢查失敗
            */
            CheckFailed = 100,       // 
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

        public static int ToInt(this Code code) => (int)code;

        public static string? ToMessage(this Code code) {
            return code switch {
                /************************************************************
                * Defualt Code 預設
                */
                Code.Undefine           /* -1   */ => $"Undefine Message Info",
                Code.Success            /*  0   */ => $"Success",
                Code.Fail               /*  1   */ => $"Fail",
                Code.Exception          /*  3   */ => $"Exception",
                Code.NotFound           /*  4   */ => $"NotFound",
                Code.InvalidData        /*  5   */ => $"InvalidData",
                /************************************************************
                * CheckFailed
                */
                //                    Hold        /*  100 */ => $"Null", // 保留
                Code.CheckFailed_ValidData        /*  101 */ => $"Check Failed [Valid Data]",
                Code.CheckFailed_CorsPolicy       /*  101 */ => $"Check Failed [Cors Policy]",


                Code.Test               /*  777 */ => $"Test",
                _ => null, //           /*  XXX */ => $"Undefine Message Info",
            };
        }

    }

}
