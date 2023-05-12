using Microsoft.EntityFrameworkCore;
using Npgsql;
using ServiceGuard.Models;

namespace ServiceGuard.Databases {

    /*  DbEntities.Service
    *   説明：Db資料庫-關於服務查詢
    *   命名規範：
    *   
    *   + 返回值-説明：
    *   - Type：返回值類型，Type 必然為 class
    *   - List<Type>：返回多筆資料
    *   
    *   + 查詢-方法名稱-説明：
    *   -          Type GetData(); 取得 Data
    *   - List<Type> GetAllData(); 獲取所有 Data
    *   -          Type HasData(); 是否有 Data
    *   
    *   + 新增-方法名稱-説明：
    *   - Type InsertData(); 新增 Data
    *   
    *   + 更新-方法名稱-説明：
    *   - Type UpdateData(); 更新 Data
    *   
    *   + 刪除-方法名稱-説明：
    *   - Type DeleteData(); 刪除 Data
    *   
    *   + 特殊-方法名稱-説明：
    *   - Type UserLogin(); 用戶登入
    *   - Type AutoLogin(); 自動登入
    *   
    */

    public partial class DbEntities : DbContext {

        public DbSet<SampleDataModel.Result> SampleResultModel { get; set; }
        public static bool UserLogin(SampleDataModel.Query parameter, out SampleDataModel.Result result) {
            
            // 建立-查詢指令
            string cmd = "";
            NpgsqlParameter[] param = new NpgsqlParameter[] {
                new NpgsqlParameter("@id", parameter.Id),
                new NpgsqlParameter("@password", parameter.Password),
            };

            // 呼叫-資料庫
            using (var entities = new DbEntities()) {
                result = entities.SampleResultModel.FromSqlRaw(cmd, param).ToList().First();
            }

            return true;
        }

    }
}
