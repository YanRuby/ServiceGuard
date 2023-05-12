using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceGuard.AppLibs;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;

using Microsoft.AspNetCore.Http.Features;
using ServiceGuard.Exceptions;

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
        #endregion

        protected TResponseData ResponseData = new(); // { get; set; } // struct 是值類型，屬性取得的是複製項，非參考項.

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

        #region 前置檢查
        /// <summary>
        /// 前置檢查器: 前置檢查流程
        /// </summary>
        /// <returns>是否通過檢查</returns>
        protected abstract bool PreChecker();
        /// <summary>
        /// **前置檢查-資料頭**
        /// </summary>
        /// <param name="parameter"> 
        /// <br> 需要驗證的欄位名稱</br>
        ///     <br> 範例： parameter = @"A,B,C..."; 檢查請求正文是否擁有欄位 A, B, C... </br>
        ///     <br> 注意： parameter = @"A,B,C..."; 欄位之間不可以有空格 </br>
        /// </param>
        /// <returns>是否存在必要資料欄位</returns>
        protected virtual bool CheckValidDataHead(string parameter) => true;
        /// <summary>
        /// **前置檢查-資料體**
        /// </summary>
        /// <param name="parameter"> 
        /// <br> 需要驗證的欄位名稱</br>
        ///     <br> 範例： parameter = @"A,B,C..."; 檢查請求正文是否擁有欄位 A, B, C... </br>
        ///     <br> 注意： parameter = @"A,B,C..."; 欄位之間不可以有空格 </br>
        /// </param>
        /// <returns>是否存在必要資料欄位</returns>
        protected virtual bool CheckValidDataBody(string parameter) => true;
        /// <summary>
        /// **數據有效性驗證** 
        /// </summary>
        /// <remarks> <br> 説明：</br>
        /// <br> 檢驗請求正文是否擁有指定欄位。此方法僅支援内部檢查，并施加于該變數 >> <see cref="JObjRequestData"/> </br>
        /// </remarks>
        /// <param name="parameter">
        ///     <br> 需要驗證的欄位名稱 </br>
        ///     <br> 範例： parameter = @"A,B,C..."; 檢查請求正文是否擁有欄位 A, B, C... </br>
        ///     <br> 注意： parameter = @"A,B,C..."; 欄位之間不可以有空格 </br>
        /// </param>
        /// <param name="path">
        ///     <br> 説明：path 為 json 的嵌套路徑 </br>
        ///     <br> 範例：path = "A.B.C..."; </br>
        ///     <br> 預設值：paht = null 時，路徑為最頂層 </br>
        /// </param>
        /// <returns>是否存在必要資料欄位</returns>
        protected virtual bool CheckValidData(string parameter, string? path = null) {
            if (JObjRequestData == null) return false;

            if(path == null) {
                foreach (var item in parameter.Split(',')) {   // 以根路徑作爲主目錄
                    Logger.LogInformation($"Parameter: {item}");
                    if (JObjRequestData.SelectToken(item) == null) return false;
                }
            }
            else {
                var token = JObjRequestData.SelectToken(path); // 獲取指定路徑作爲主目錄
                if (token == null) return false;
                foreach (var item in parameter.Split(',')) {
                    Logger.LogInformation($"Parameter Token: {item}");
                    if (token.SelectToken(item) == null) return false;
                }
            }

            return true;
        }
        /// <summary>
        /// 跨域檢查
        /// </summary>
        /// <returns>是否携帶跨域來源</returns>
        protected virtual bool CheckHasCorsPolicy() => true;
        /// <summary>
        /// Redis高速資料庫-資料比對
        /// </summary>
        /// <returns>比對是否一致</returns>
        protected virtual bool ComparisonData() => true;
        /// <summary>
        /// 資料解密
        /// </summary>
        /// <returns>解密是否成功</returns>
        protected virtual bool DataDecription() => true;
        #endregion

        /// <summary>
        /// 處理請求
        /// </summary>
        protected abstract bool ProcessData();

        /// <summary>
        /// 執行 ( 資料處理流程 | 收包策略 )
        /// </summary>
        /// <returns>響應内容</returns>
        protected virtual async Task<object?> Run() {
            // 載入請求正文
            using (StreamReader reader = new (Request.Body)) {
                Request.Body.Seek(0, SeekOrigin.Begin); // 重置-讀寫頭位置

                // 獲取-請求正文内容
                var content = await reader.ReadToEndAsync();

                JObjRequestData = JsonConvert.DeserializeObject<JObject>(content);
                Logger.LogInformation(
                    $"\n>>>>> Request.ContentLen: {content.Length}" +
                    $"\n>>>>> Request.Body:\n{content}\n"
                );
            }
            return null;
        }

        /// <summary>
        /// 一般日志記錄
        /// </summary>
        /// <param name="message">所需記錄的内容</param>
        /// <param name="args">參數</param>
        [Obsolete("This method has BUG", false)]
        protected void LogInformation(string message, params object[] args) {
            var dt = TimeZoneInfo.ConvertTime(DateTime.UtcNow, Utils.TimeZoneInfo).ToString("yy-MM-dd HH:mm:ss.fff");
            var name = GetType().Name;
            var msg = string.Format(message, args);
            Logger.LogInformation($"dt:{dt}, api:{name}, {msg}");
        }

    }

}
