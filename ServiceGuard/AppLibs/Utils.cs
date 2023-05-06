
namespace ServiceGuard.AppLibs {

    public static class Utils {

        /// <summary>
        /// 以 UTC 格式取得當前系統時區
        /// </summary>
        public static TimeZoneInfo TimeZoneInfo { get; } = TimeZoneInfo.FindSystemTimeZoneById("UTC");

    }

}
