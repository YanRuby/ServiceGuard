using System.ComponentModel.DataAnnotations;

// 注意: 此命名空間為：參考範本，禁止使用範本空間 ( 即：ServiceGuard.Sample 開頭的命名空間 )
namespace ServiceGuard.Sample.Models {

    /*  範本背景假設 (參考範本實作時，請刪除此段背景假設)
    *   - 用途：用戶資訊 ( User )
    *   - 命名：calss UserDataModel
    */

    /// <summary>
    /// 用戶資料模型
    /// </summary>
    public class UserDataModel {

        public abstract class DefaultResult : Commons.IResult {
            public virtual int ResultCode { get; set; } = -1;
            public virtual string ResultMsg { get; set; } = "";
        }

        public class Login {
            // Language Integrated Query
            public struct Linq {
                #region 僅作參考
                /*  **資料庫查詢時所需要的必要資料欄位**
                *   todo: 自行添加查詢時需要資料欄位，以下僅作參考：
                */
                public string Id { get; set; }
                public string Password { get; set; }
                #endregion
            }
            public class Result : DefaultResult {
                #region 僅作參考
                /*  **資料庫查詢結果所需的資料承載**
                *   todo: 自行添加需要的資料承載欄位，以下僅作參考：
                */
                [Key] // 請將此標簽標注於'主鍵'
                public string SessionKey { get; set; } = "";
                #endregion
            }
        }

    }
}
