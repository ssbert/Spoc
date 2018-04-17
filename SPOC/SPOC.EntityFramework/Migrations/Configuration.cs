using System.Data.Entity.Migrations;

namespace SPOC.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<SPOC.EntityFramework.SPOCDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "SPOC";
            SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());//设置Sql生成器为Mysql的
        }

        protected override void Seed(SPOC.EntityFramework.SPOCDbContext context)
        {
            // This method will be called every time after migrating to the latest version.
            // You can add any seed data here...
        }
    }
}
