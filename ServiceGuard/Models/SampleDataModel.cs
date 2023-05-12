using System.ComponentModel.DataAnnotations;
using ServiceGuard.Exceptions;

namespace ServiceGuard.Models {

    public class SampleDataModel {

        public class Result : Exceptions.IResult {
            public int ResultCode { get; set; } = -1;
            public string ResultMsg { get; set; } = "";

            #region 僅作參考
            /*  **資料庫查詢結果所需的資料承載**
            *   todo: 自行添加需要的資料承載欄位，以下僅作參考：
            */
            [Key] // 請將此標簽標注於'主鍵'
            public string SessionKey { get; set; } = "";
            #endregion
        }

        public struct Query {
            #region 僅作參考
            /*  **資料庫查詢時所需要的必要資料欄位**
            *   todo: 自行添加查詢時需要資料欄位，以下僅作參考：
            */
            public string Id { get; set; }
            public string Password { get; set; }
            #endregion
        }

    }

}
