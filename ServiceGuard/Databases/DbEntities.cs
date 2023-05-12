using Microsoft.EntityFrameworkCore;
using ServiceGuard.AppLibs;

namespace ServiceGuard.Databases {

    public partial class DbEntities : DbContext {

        protected ILogger Logger { get; set; }

        public DbEntities() {
            Logger = null;
        }

        public DbEntities(ILogger<DbEntities> logger) {
            Logger = logger;
        }

        public DbEntities(DbContextOptions<DbEntities> options, ILogger<DbEntities> logger)
            : base(options) {
            Logger = logger;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            Console.WriteLine("Test");

            if (!optionsBuilder.IsConfigured) {
                var connStr = AppSettings.DbConnectionStr;

                // 兼容無資料庫
                if (connStr == "") {
                    Logger.LogInformation("No Database Mode");
                    return; 
                }

                optionsBuilder.UseNpgsql(connStr);
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess) {
            Logger.LogInformation("Saving changes to the database");
            int result = base.SaveChanges(acceptAllChangesOnSuccess);
            Logger.LogInformation("Changes saved to the database");
            return result;
        }

    }
}
