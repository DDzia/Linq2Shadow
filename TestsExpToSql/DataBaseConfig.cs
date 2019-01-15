using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace TestsExpToSql
{
    static class DataBaseConfig
    {
        public const string DatabaseName = "ExpToSqlTests";

        public static readonly string ConnectionStringWithoutDb = $"Server=(localdb)\\mssqllocaldb;Trusted_Connection=True;MultipleActiveResultSets=true;Integrated Security=True;";
        public static readonly string ConnectionString = $"{ConnectionStringWithoutDb}Database={DatabaseName};";

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
            var sql = File.ReadAllText(filePath);
            conn.ExecuteScript(sql);
        }

        //public static void DropTableIfExists(this IDbConnection conn, string name, string schema = "dbo")
        //{
        //    var cmd = $"IF OBJECT_ID('{schema}.{name}', 'U') IS NOT NULL\n" +
        //                      $"\tDROP TABLE {schema}.{name}";
        //    conn.ExecuteScript(cmd);
        //}

        public static void ReCreateDatabase()
        {
            using (var conn = new SqlConnection(ConnectionStringWithoutDb))
            {
                conn.Open();

                var recreateDbPhysicaly = "USE master\n" +
                        $"IF EXISTS(select * from sys.databases where name='{DatabaseName}')\n" +
                        $"DROP DATABASE {DatabaseName}\n" +
                        $"CREATE DATABASE {DatabaseName}";
                conn.ExecuteScript(recreateDbPhysicaly);
            }

            using (var conn = CreateConnectionAndOpen())
            {
                conn.ExecuteScriptFromFile("scripts/create-users-table.sql");
                conn.ExecuteScriptFromFile("scripts/populate-users.sql");
                conn.ExecuteScriptFromFile("scripts/get-all-users-function.sql");
                conn.ExecuteScriptFromFile("scripts/return-allusers-sp.sql");
            }
        }
    }
}
