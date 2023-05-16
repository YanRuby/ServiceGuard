using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using ServiceGuard.Commons;
using ServiceGuard.Sample.Databases;
using ServiceGuard.Sample.Models;

// 注意: 此命名空間為：參考範本，禁止使用範本空間 ( 即：ServiceGuard.Sample 開頭的命名空間 )
namespace ServiceGuard.Sample.Controllers {

    [ApiController]                 // 標記-此類作爲API
    [Route("api/[controller]")]     // 啓用-URL路由
    [EnableCors("CorsPolicy")]      // 啓用-跨域策略 (似情況開啓，請遵循安全策略)
    public partial class SampleController : WebApiTemplate
        <SampleController.RequestDataModel, SampleController.ResponseDataModel> {

        /// <summary>
        /// API入口 ( 當收到請求時執行 )
        /// </summary>
        /// <param name="value">請求内容</param>
        /// <returns>響應結果</returns>
        [HttpPost] // 請求數據在數據體
        public virtual async Task<object> Post([FromBody] RequestDataModel value) {
            RequestData = value;    // 收到-請求内容
            await BuildRequest();   // 解析-請求内容

            /*  **前置檢查-説明**
            *   - 1：檢查請求所携帶的請求正文是否符合此<資料模型(RequestDataModel)>
            *   - 2：檢查請求正文是否携帶必要欄位資訊以做校驗等
            *   - 原因：排除 | 過濾不符條件的請求，以提高整體系統效率
            *   - 注意：過多的檢查會降低整體效率!!! 如非必要請不要涵蓋所有欄位進行檢查，可空欄不予以檢查
            *   + 方法：PreChecker() 為前置檢查流程，檢查條件、流程如需調整可於此方法内調整
            */

            // 前置檢查
            if (
                // 資料頭(Head)
                CheckValidDataHead(@"time") == true
                // 資料體(Body)
                && CheckValidDataBody(@"sessionKey") == true
                // 跨域資訊(CorsPolicy)
                && CheckHasCorsPolicy() == true
                // 高速資料庫比對資料(Redis)
                && ComparisonData() == true
                // 數據解密(Decription)
                && DataDecription() == true
                )
            // 放行(Pass) 前置檢查全部通過
            {
                // 處理資料
                if (ProcessData() == true) {
                    BuildResult(WebApiResult.Code.Success);
                }
            }

            BuildResponse(); // 建立-響應
            Logger.LogInformation($"{ResponseData}\n"); // Debug log

            return ResponseData; // 回復請求結果
        }

        [HttpGet] // 請求數據附加在 URL
        public virtual async Task<object> Get() {
            //RequestData = value;    // 收到-請求内容
            await BuildRequest();   // 解析-請求内容

            /*  **前置檢查-説明**
            *   - 1：檢查請求所携帶的請求正文是否符合此<資料模型(RequestDataModel)>
            *   - 2：檢查請求正文是否携帶必要欄位資訊以做校驗等
            *   - 原因：排除 | 過濾不符條件的請求，以提高整體系統效率
            *   - 注意：過多的檢查會降低整體效率!!! 如非必要請不要涵蓋所有欄位進行檢查，可空欄不予以檢查
            *   + 方法：PreChecker() 為前置檢查流程，檢查條件、流程如需調整可於此方法内調整
            */

            // 前置檢查
            if (
                // 資料頭(Head)
                CheckValidDataHead(@"time") == true
                // 資料體(Body)
                && CheckValidDataBody(@"sessionKey") == true
                // 跨域資訊(CorsPolicy)
                && CheckHasCorsPolicy() == true
                // 高速資料庫比對資料(Redis)
                && ComparisonData() == true
                // 數據解密(Decription)
                && DataDecription() == true
                )
            // 放行(Pass) 前置檢查全部通過
            {
                // 處理資料
                if (ProcessData() == true) {
                    BuildResult(WebApiResult.Code.Success);
                }
            }

            BuildResponse(); // 建立-響應
            Logger.LogInformation($"{ResponseData}\n"); // Debug log

            return ResponseData; // 回復請求結果
        }
        
        [HttpDelete]
        public virtual async Task<object> Delete() {
            return ResponseData; // 回復請求結果
        }

        [HttpPut]
        public virtual async Task<object> Put() {
            return ResponseData; // 回復請求結果
        }

        [HttpPatch]
        public virtual async Task<object> Patch() {
            return ResponseData; // 回復請求結果
        }

        [HttpHead]
        public virtual async Task<object> Head() {
            return ResponseData; // 回復請求結果
        }

        [HttpOptions]
        public virtual async Task<object> Options() {
            return ResponseData; // 回復請求結果
        }

    }

    public partial class SampleController : WebApiTemplate
        <SampleController.RequestDataModel, SampleController.ResponseDataModel> {

        #region DataModel 資料模型
        public struct RequestDataModel {
            #region 僅作參考
            /*  **請求時需要的資料**
            *   todo: 自行添加請求時需要資料欄位，以下僅作參考：
            */
            public string Id { get; set; }
            public string Password { get; set; }
            #endregion
        }
        public struct ResponseDataModel : Commons.IResult {
            public int ResultCode { get; set; }     // 響應代碼 ( 操作成功時為 0 )
            public string ResultMsg { get; set; }   // 響應資訊 ( 錯誤時應記錄錯誤資訊 )

            #region 僅作參考
            /*  **請求時需要的資料**
            *   todo: 自行添加請求時需要資料欄位，以下僅作參考：
            */
            public string SessionKey { get; set; }
            #endregion

            public override string ToString() {
                return "\n" + base.ToString() + " {\n"
                    + $"  >> ResultCode: {ResultCode}\n"
                    + $"  >> ResultMsg: {ResultMsg}\n"
                    + "}\n";
            }
        }
        #endregion

        protected override ILogger Logger { get; set; }

        protected Npgsql_UserManagerDbCtx dbContext;

        /// <summary>
        /// Constructor 構建式
        /// </summary>
        /// <param name="logger">依賴注入: 日志</param>
        public SampleController(Npgsql_UserManagerDbCtx dbContext, ILogger<SampleController> logger) {
            Logger = logger;
            result = ResponseData;
            this.dbContext = dbContext;
        }

        #region ProcessData 資料處理
        /*  **資料處理-説明**
        *   - 
        */
        protected override bool ProcessData() {
            try {
                // 資料庫查詢時所需要的必要資料欄位


                UserDataModel.Login.Linq linq = new() {
                    Id = RequestData.Id,
                    Password = RequestData.Password,
                };

                // 呼叫-資料庫
                var isFound = dbContext.Login(linq, out UserDataModel.Login.Result? data);

                // 未查詢到結果
                if(isFound == false) {
                    BuildResult(WebApiResult.Code.CheckFailed_ValidData);
                    return false;
                }
                // else 查詢到結果

                // 資料不爲空
                if (data != null) {

                    // 寫入-響應正文
                    ResponseData.SessionKey = data.SessionKey;
                    // More...

                    return true;
                }

                // For Debug
                Logger.LogInformation("Linq result data is null!");
                BuildResult(WebApiResult.Code.CheckFailed_ValidData);
                return false;
            }

            // 捕獲例外狀況
            catch (Exception ex) {
#pragma warning disable CA2254
                Logger.LogError(ex.Message);
                Logger.LogError(ex.StackTrace);
#pragma warning restore CA2254

#if DEBUG
                // 暴露例外訊息不安全
                BuildResult(WebApiResult.Code.CheckFailed_ValidData);
#else
                ResponseData = (ResponseDataModel)Result.BuildExceptionInfo(ResponseData);
#endif
                return false;
            }

        }
        #endregion

        /// <summary>
        /// **建立-響應**
        /// </summary>
        protected override void BuildResponse() {
            ResponseData = (ResponseDataModel)result;
        }

    }
}
