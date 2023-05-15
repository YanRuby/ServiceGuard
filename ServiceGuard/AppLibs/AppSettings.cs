

namespace ServiceGuard.AppLibs {

    public static class AppSettings {

        // 跨域-許可清單
        public static List<string> Origins { get; set; } = new();

        // 資料庫-連綫口號
        public static string DbConnectionStr { get; set; } = "";

    }

}
