using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceGuard.AppLibs;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;

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
        protected JObject? JObjRequestData { get; private set; }
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
        protected virtual async Task<object> Run() {

            /*using (var memoryStream = new MemoryStream()) {
                Request.EnableBuffering();
                Request.Body.Seek(0, SeekOrigin.Begin);
                await Request.Body.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                // 在這裡對內容進行處理
                using (var reader = new StreamReader(memoryStream)) {
                    var content = await reader.ReadToEndAsync();
                    Console.WriteLine($"Request: {content}");
                }
            }
*/
            /*using (var memoryStream = new MemoryStream()) {
                Console.WriteLine($"Request ContentLen: {Request.ContentLength}");
                Console.WriteLine($"Request BodyLen: {Request.Body}");


                //Request.Body.Position = 0;
                //await Request.Body.SeekAsync(0, SeekOrigin.Begin);
                await Request.Body.CopyToAsync(memoryStream);
                Console.WriteLine($"MS: {memoryStream.Length}");

                memoryStream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(memoryStream)) {
                    
                    var tmp = await reader.ReadToEndAsync();
                    Console.WriteLine($"Test: {tmp}");

                    JObjRequestData = JsonConvert.DeserializeObject<JObject>(tmp);
                    
                }
            }*/

            //return BuildResponseObject();
            
            using (StreamReader reader = new StreamReader(Request.Body)) {
                Request.EnableBuffering();
                Request.Body.Seek(0, SeekOrigin.Begin);
                Request.Body.Position = 0;
                Console.WriteLine($"Length: {Request.ContentLength}, {Request.ContentType}");
                var content = await reader.ReadToEndAsync();

                JObjRequestData = JsonConvert.DeserializeObject<JObject>(content);
                Console.WriteLine($"Test: {content.Length}");
            }
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
