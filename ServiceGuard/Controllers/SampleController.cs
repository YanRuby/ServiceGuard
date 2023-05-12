#define DEBUG_RequestData

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ServiceGuard.Commons;
using ServiceGuard.Models;
using ServiceGuard.Databases;
using ServiceGuard.Exceptions;

namespace ServiceGuard.Controllers {

    [ApiController]                 // 標記-此類作爲API
    [Route("api/[controller]")]     // 啓用-URL路由
    [EnableCors("CorsPolicy")]      // 啓用-跨域策略 (似情況開啓，請遵循安全策略)
    public class SampleController : WebApiTemplate
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

        public struct ResponseDataModel : Exceptions.IResult {
            public int ResultCode { get; set; }     // 響應代碼 ( 操作成功時為 0 )
            public string ResultMsg { get; set; }   // 響應資訊 ( 錯誤時應記錄錯誤資訊 )

            #region 僅作參考
            /*  **請求時需要的資料**
            *   todo: 自行添加請求時需要資料欄位，以下僅作參考：
            */
            public string SessionKey { get; set; }
            #endregion

            public override string ToString() {
                return base.ToString() + " {\n"
                    + $"  >> ResultCode: {ResultCode}\n"
                    + $"  >> ResultMsg: {ResultMsg}\n"
                    + "}\n";
            }
        }
        #endregion

        protected override ILogger Logger { get; set; }

        public SampleController(ILogger<SampleController> logger) {
            Logger = logger;
        }

        #region PreChecking 前置檢查
        /*  **前置檢查-説明**
        *   - 1：檢查請求所携帶的請求正文是否符合此<資料模型(RequestDataModel)>
        *   - 2：檢查請求正文是否携帶必要欄位資訊以做校驗等
        *   - 原因：排除 | 過濾不符條件的請求，以提高整體系統效率
        *   - 注意：過多的檢查會降低整體效率!!! 如非必要請不要涵蓋所有欄位進行檢查，可空欄不予以檢查
        *   + 方法：PreChecker() 為前置檢查流程，檢查條件、流程如需調整可於此方法内調整
        */

        protected override bool PreChecker() {
            /*  **檢查流程**
            *   資料頭(Head) >> 資料體(Body) >> 跨域資訊(CorsPolicy) >> 高速資料庫比對身份(Redis) >> 數據解密(Decription) >> 放行(Pass)
            */

            // 資料頭(Head)
            if (CheckValidDataHead(@"time") == false) return false;
            // 資料體(Body)
            if (CheckValidDataBody(@"sessionKey") == false) return false;
            // 跨域資訊(CorsPolicy)
            if (CheckHasCorsPolicy() == false) return false;
            // 高速資料庫比對資料(Redis)
            if (ComparisonData() == false) return false;
            // 數據解密(Decription)
            if (DataDecription() == false) return false;

            // 放行(Pass) 前置檢查全部通過
            return true;
        }
        protected override bool CheckValidDataHead(string parameter) {
            if (CheckValidData(parameter, "header") == false) {
                ResponseData = (ResponseDataModel)Result.BuildInfo(ResponseData, Result.Code.Fail);
                return false;
            }
            return true;
        }
        protected override bool CheckValidDataBody(string parameter) {
            if(CheckValidData(parameter) == false) {
                ResponseData = (ResponseDataModel)Result.BuildInfo(ResponseData, Result.Code.Fail);
                return false;
            }
            return true;
        }
        protected override bool CheckHasCorsPolicy() {
            /*if (AppSettings.Origins.Contains(Request.Headers["Origin"].ToString()) == false) {
                OutputData.result_code = 103;
                logger.LogWarning("request is not valid! origin:{0} not in ", Request.Headers["Origin"].ToString(), AppSettings.Origins);
                return false;
            }*/
            return true;
        }
        protected override bool ComparisonData() {
            /*var machine = RedisHelper.SessionCustomer(InputData.session_key);
            if (machine.customerIdx == -1) {
                OutputData.result_code = 104;
                return false;
            }*/
            return true;
        }
        protected override bool DataDecription() {
            return base.DataDecription();
        }
        #endregion
        #region ProcessData 資料處理
        /*  **資料處理-説明**
        *   - 
        */

        protected override bool ProcessData() {
            try {
                // 資料庫查詢時所需要的必要資料欄位
                SampleDataModel.Query query = new() {
                    Id = RequestData.Id,
                    Password = RequestData.Password,
                };

                // 呼叫-資料庫
                if (DbEntities.UserLogin(query, out SampleDataModel.Result data) == false) {
                    ResponseData = (ResponseDataModel)Result.BuildInfo(ResponseData, Result.Code.Fail);
                    return false;
                }

                // 寫入-響應正文
                {
                    ResponseData.SessionKey = data.SessionKey;
                }

            }
            catch (Exception ex) {
#pragma warning disable CA2254
                Logger.LogError(ex.Message);
                Logger.LogError(ex.StackTrace);
#pragma warning restore CA2254

#if DEBUG
                // 暴露例外訊息不安全
                ResponseData = (ResponseDataModel)Result.BuildExceptionInfo(ResponseData, ex.Message);
#else
                ResponseData = (ResponseDataModel)Result.BuildExceptionInfo(ResponseData);
#endif
                return false;
            }
            return true;
        }
        #endregion

        protected override async Task<object?> Run() {
            await base.Run();

            // 前置檢查 & 處理請求
            if (PreChecker() == true) {
                // 處理資料
                if (ProcessData() == true) {
                    ResponseData = (ResponseDataModel)Result.BuildSuccessInfo(ResponseData);
                } // else >> 已於 ProcessData() 方法中定義錯誤資訊
            } // else >> 已於 PreChecker() 方法中定義錯誤資訊

            // 響應請求
            Logger.LogInformation(
                $"\n>>>>> ResponseData:\n{ResponseData}\n"
            );

            return ResponseData;
        }

    }
}
