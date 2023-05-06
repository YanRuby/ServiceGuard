using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceGuard.AppLibs;

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
        protected TRequestData RequestData { get; private set; }
        protected TResponseData ResponseData = new(); // { get; set; } // struct 是值類型，屬性取得的是複製項，非參考項.
        protected JObject? JObjRequestData { get; private set; }
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
            //return ResponseData;
        }

        [HttpGet]
        public virtual object Get([FromBody] TRequestData value) {
            RequestData = value;
            Console.WriteLine("Get");
            return Run(); // Array ???
        }


        /// <summary>
        /// 參數驗證
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
        protected virtual object Run() {

            using (var reader = new StreamReader(Request.Body)) {
                Request.Body.Seek(0, SeekOrigin.Begin);
                JObjRequestData = JsonConvert.DeserializeObject<JObject>(reader.ReadToEnd());
            }

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
