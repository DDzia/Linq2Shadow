using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Linq2ShadowTests
{
    internal static class DbConfig
    {
        private const string DatabaseName = "ExpToSqlTests";

        private static readonly string ConnectionStringWithoutDb = @"Server=(localdb)\mssqllocaldb;Trusted_Connection=True;MultipleActiveResultSets=true;Integrated Security=True;Pooling=false;";
        private static readonly string ConnectionString = $"{ConnectionStringWithoutDb}Database={DatabaseName};";

        public static IDbConnection CreateConnectionAndOpen()
        {
            var conn = new SqlConnection(ConnectionString);
            conn.Open();

            return conn;
        }

        public static void ExecuteScript(this IDbConnection conn, string cmdText)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = cmdText;
            cmd.ExecuteNonQuery();
        }

        public static void ExecuteScriptFromFile(this IDbConnection conn, string filePath)
        {
            var root = Path.GetDirectoryName(typeof(DbConfig).Assembly.Location);
            var absolutePath = Path.Combine(root, filePath);
            var sql = File.ReadAllText(absolutePath);
            conn.ExecuteScript(sql);
        }

        public static void ReInitDatabase()
        {
            DropDatabase();
            CreateDatabase();
            PopulateDatabaseData();
        }

        private static void DropDatabase()
        {
            using (var conn = new SqlConnection(ConnectionStringWithoutDb))
            {
                conn.Open();

                var recreateDbPhysicaly = "USE master\n" +
                                          $"IF EXISTS(select * from sys.databases where name='{DatabaseName}')\n" +
                                          $"DROP DATABASE {DatabaseName}";
                conn.ExecuteScript(recreateDbPhysicaly);
            }
        }

        private static void CreateDatabase()
        {
            using (var conn = new SqlConnection(ConnectionStringWithoutDb))
            {
                conn.Open();

                var recreateDbPhysicaly = "USE master\n" +
                                          $"CREATE DATABASE {DatabaseName}";
                conn.ExecuteScript(recreateDbPhysicaly);
            }
        }

        private static void PopulateDatabaseData()
        {
            using (var conn = CreateConnectionAndOpen())
            {
                conn.ExecuteScriptFromFile("scripts/tables/create-users-table.sql");
                conn.ExecuteScriptFromFile("scripts/tables/create-reports-table.sql");
                conn.ExecuteScriptFromFile("scripts/tables/create-alltypes.table.sql");

                conn.ExecuteScriptFromFile("scripts/tv-functions/get-all-users-function.sql");
                conn.ExecuteScriptFromFile("scripts/sp/return-allusers-sp.sql");

                conn.ExecuteScriptFromFile("scripts/population/populate-users.sql");
                conn.ExecuteScriptFromFile("scripts/population/populate-reports.sql");
                conn.ExecuteScriptFromFile("scripts/population/populate-alltypes.sql");
            }
        }

        public static class DbObjectNames
        {
            public const string UsersTable = "tUsers";
            public const string ReportsTable = "tReports";
            public const string TypesTable = "[dbo].[AllTypes]";

            public const string GetAllUsersFunction = "fGetAllUsers";

            public const string GetAllUsersSp = "spReturnAllUsers";
        }
    }
}
