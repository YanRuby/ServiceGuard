#define DEBUG_RequestData

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using ServiceGuard.Commons;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ServiceGuard.Controllers {

    [ApiController]                 // 標記-此類作爲API
    [Route("api/[controller]")]     // 啓用-URL路由
    [EnableCors("CorsPolicy")]      // 啓用-跨域策略 (似情況開啓，請遵循安全策略)
    public class SampleController : WebApiTemplate
        <SampleController.RequestDataModel, SampleController.ResponseDataModel> {

        public struct RequestDataModel {
            // todo: 請求時需要的資料
            // ex: public string Id { get; set; }
            // ...
            public string Id { get; set; }
            public string Password { get; set; }
        }

        public struct ResponseDataModel : WebApiException.IResult {
            public int ResultCode { get; set; }      // 響應代碼 ( 操作成功時為 0 )
            public string ResultMsg { get; set; }    // 響應資訊 ( 錯誤時應記錄錯誤資訊 )

            // todo: 響應時需要回傳的資料
            // ex: public string Name { get; set; }
            // ...

            string WebApiException.IResult.ToString() {
                return base.ToString() + " {\n"
                    + $"  >> ResultCode: {ResultCode}\n"
                    + $"  >> ResultMsg: {ResultMsg}\n"
                    + "}\n";
            }

        }

        protected override ILogger Logger { get; set; }

        public SampleController(ILogger<SampleController> logger) {
            Logger = logger;
        }

        protected override bool PreCheckValidData() {

            /*// 通訊鑰 (身份驗證)
            if (JObjRequestData == null) return false;
            var parameter = @"session_key".Split(',');
            foreach (var item in parameter) {
                if (JObjRequestData.SelectToken(item) == null) {
                    //ResponseData.ResultCode = 101;
                    WebApiException.BuildResultInfo(ResponseData);
                    Logger.LogError($"Input Error:{item}");
                    return false;
                }
            }*/
            var tmp = (WebApiException.IResult)ResponseData;
            var tip = PreCheckSessionKey(ref tmp);
            ResponseData = (ResponseDataModel)tmp;
            Console.WriteLine("Data: \n" + ResponseData.ToString() + " {\n"
                    + $"  >> ResultCode: {ResponseData.ResultCode}\n"
                    + $"  >> ResultMsg: {ResponseData.ResultMsg}\n"
                    + "}\n"
            );
            Console.WriteLine($"Test: {tip}");
            // todo: 如需額外的前置檢查，請在下方繼續設置驗證關卡
            // ...

            return true;
        }

        protected override void ProcessData() {
            //throw new NotImplementedException();
            /*
             
            try {

                if (AppSettings.Origins.Contains(Request.Headers["Origin"].ToString()) == false) {
                    OutputData.result_code = 103;
                    logger.LogWarning("request is not valid! origin:{0} not in ", Request.Headers["Origin"].ToString(), AppSettings.Origins);
                    return;
                }

                var machine = RedisHelper.SessionCustomer(InputData.session_key);
                if (machine.customerIdx == -1) {
                    OutputData.result_code = 104;
                    return;
                }

                OutputData.result_code = 0;
                var data = O2OEntities.guard_store_information(machine.lsid);

                OutputData.storeName = data.store_Name;
                OutputData.city = data.city;
                OutputData.state = data.state;
                OutputData.zip = data.zip;
                OutputData.phone = data.phone;
                OutputData.address1 = data.address1;
                OutputData.address2 = data.address2;

            } catch (Exception ex) {
                OutputData.result_code = 102;
                logger.LogError(ex.Message);
                logger.LogError(ex.StackTrace);
            }
             
             */

        }

        protected override object BuildResponseObject() {

            /*switch (ResponseData.ResultCode) {
                case 0:
                    ResponseData.ResultMsg = "Test";
                    break;
                default:
                    // Undefined The Code
                    break;
            }*/

            ResponseData = (ResponseDataModel)WebApiException.BuildResultInfo(ResponseData);
            return ResponseData;
        }

        /// <summary>
        /// 執行 ( 資料處理流程 | 收包策略 )
        /// </summary>
        /// <returns>響應内容</returns>
        protected override async Task<object> Run() {

#if DEBUG_RequestData
            // 測試流程：測試通訊封包是否暢通
            await base.Run();
            Logger.LogInformation($">>>> [FromBody]: {RequestData.Id}, {RequestData.Password}");
#else
            // 預設流程 | 預設策略
            base.Run();

            // todo: 自定義流程，請移除<預設流程>，并在下方實作即可
            // ...
#endif
            return ResponseData;
        }

    }
}
