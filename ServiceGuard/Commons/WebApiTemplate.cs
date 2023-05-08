using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceGuard.AppLibs;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;

using Microsoft.AspNetCore.Http.Features;

namespace ServiceGuard.Commons {

    /// <summary>
    /// WebApi 模板 ( 以 Json 進行交互 )
    /// </summary>
    /// <typeparam name="TRequestData">請求内容</typeparam>
    /// <typeparam name="TResponseData">響應内容</typeparam>
    public abstract class WebApiTemplate<TRequestData, TResponseData> : Controller
        where TRequestData : struct
        where TResponseData : struct {

        #region Properties
        protected abstract ILogger Logger { get; set; }
        protected JObject? JObjRequestData { get; private set; } // 用於身份檢查
        protected TRequestData RequestData { get; private set; }
        protected TResponseData ResponseData = new(); // { get; set; } // struct 是值類型，屬性取得的是複製項，非參考項.
        #endregion

        /// <summary>
        /// API入口 ( 當收到請求時執行 )
        /// </summary>
        /// <param name="value">請求内容</param>
        /// <returns>響應結果</returns>
        [HttpPost]
        public virtual object Post([FromBody] TRequestData value) {
            RequestData = value;
            return Run();
        }

        [HttpGet]
        public virtual object Get() {
            Console.WriteLine("Get");
            return Run();
        }

        /// <summary>
        /// 通訊身份驗證
        /// </summary>
        /// <returns></returns>
        protected virtual bool PreCheckSessionKey(ref WebApiException.IResult result) {
            WebApiException.BuildResultInfo(ref result, WebApiException.Code.SessionFail);
            Console.WriteLine("PreCheckSKey: \n" + result.ToString());

            /*if (JObjRequestData == null) return false;
            var parameter = @"sessionKey".Split(',');
            foreach (var item in parameter) {
                if (JObjRequestData.SelectToken(item) == null) {
                    WebApiException.BuildResultInfo(ref result, WebApiException.Code.SessionFail);
                    return false;
                }
            }*/
            return true;
        }

        /// <summary>
        /// 數據有效性驗證
        /// </summary>
        /// <returns>驗證是否通過</returns>
        protected virtual bool PreCheckValidData() => true;

        /// <summary>
        /// 資料處理
        /// </summary>
        protected abstract void ProcessData();

        /// <summary>
        /// 創建響應内容
        /// </summary>
        /// <returns>響應内容</returns>
        protected abstract object BuildResponseObject();

        /// <summary>
        /// 執行 ( 資料處理流程 | 收包策略 )
        /// </summary>
        /// <returns>響應内容</returns>
        protected virtual async Task<object> Run() {

            //Request.EnableBuffering();    // 啓用-緩存
            //Request.Body.Position = 0;    // 重置-讀寫頭位置
            using (StreamReader reader = new (Request.Body)) {
                Request.Body.Seek(0, SeekOrigin.Begin); // 重置-讀寫頭位置

                // 獲取-請求正文内容
                var content = await reader.ReadToEndAsync();

                JObjRequestData = JsonConvert.DeserializeObject<JObject>(content);
                Logger.LogInformation($">>>> Request.Body: {content.Length}\n{content}");
            }

            PreCheckValidData();
            return BuildResponseObject();

            if (PreCheckValidData()) {
                LogInformation($"Data_Request: {JsonConvert.SerializeObject(RequestData)}");
                ProcessData();
            }

            LogInformation($"response_data:{JsonConvert.SerializeObject(ResponseData)}");
            return BuildResponseObject();
        }

        /// <summary>
        /// 一般日志記錄
        /// </summary>
        /// <param name="message">所需記錄的内容</param>
        /// <param name="args">參數</param>
        protected void LogInformation(string message, params object[] args) {
            var dt = TimeZoneInfo.ConvertTime(DateTime.UtcNow, Utils.TimeZoneInfo).ToString("yy-MM-dd HH:mm:ss.fff");
            var name = GetType().Name;
            var msg = string.Format(message, args);
            Logger.LogInformation($"dt:{dt}, api:{name}, {msg}");
        }

    }

}
