using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Linq2Shadow.QueryProviders;

namespace Linq2Shadow
{
    public class DatabaseContext: IDisposable
    {
        private readonly Func<IDbConnection> _connFactory;
        internal readonly Lazy<IDbConnection> Connection;

        public DatabaseContext(Func<IDbConnection> db)
        {
            _connFactory = db;
            Connection = new Lazy<IDbConnection>(() =>
            {
                var connCreated = _connFactory();
                if (connCreated.State == ConnectionState.Closed)
                {
                    connCreated.Open();
                }
                return connCreated;
            });
        }

        public virtual DatabaseContext Branch() => new DatabaseContext(_connFactory);

        public IQueryable<ShadowRow> FromTableFunction(string functionName, object[] parameters = null)
        {
            return new Query<ShadowRow>(new FunctionCallQueryProvider(this, functionName, parameters));
        }

        public IEnumerable<ShadowRow> QueryToStoredProcedure(string storedProcedureName, IDictionary<string, object> parameters = null)
        {
            return FromStoredProcedureInternal(storedProcedureName, parameters);
        }

        public IEnumerable<ShadowRow> QueryToStoredProcedure<T>(string storedProcedureName, T parameters = null)
            where T: class
        {
            var parametersMap = new Dictionary<string, object>();

            if (parameters != null)
            {
                var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var p in properties)
                {
                    parametersMap[p.Name] = p.GetValue(parameters);
                }

                var fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public);
                foreach (var f in fields)
                {
                    parametersMap[f.Name] = f.GetValue(parameters);
                }
            }

            return FromStoredProcedureInternal(storedProcedureName, parametersMap);
        }

        private IEnumerable<ShadowRow> FromStoredProcedureInternal(string storedProcedureName,
            IDictionary<string, object> parameters = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine("DECLARE @return_value int");
            sb.AppendLine($"EXEC @return_value = {storedProcedureName}");

            var cmd = Connection.Value.CreateCommand();
            if (parameters != null && parameters.Any())
            {
                var paramNames = new List<string>();
                foreach (var param in parameters)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = param.Key;
                    p.Value = param.Value;
                    cmd.Parameters.Add(p);

                    paramNames.Add($"\t@{param.Key}");
                }
                var paramsAllSql = string.Join(",\n", paramNames);
                sb.AppendLine(paramsAllSql);
            }

            sb.AppendLine("SELECT 'Return Value' = @return_value");

            using (cmd)
            {
                cmd.CommandText = sb.ToString();

                using (var reader = cmd.ExecuteReader())
                {
                    return reader.ReadAll();
                }
            }
        }

        public IQueryable<ShadowRow> FromSource(string sourceName)
        {
            return new Query<ShadowRow>(new FromSourceQueryProvider(this, sourceName));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Connection.IsValueCreated)
                {
                    Connection.Value.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
