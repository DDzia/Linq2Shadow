using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Linq2Shadow.Extensions;
using Linq2Shadow.QueryProviders;

[assembly: CLSCompliant(true)]

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

        public IQueryable<ShadowRow> QueryToTableValuedFunction(string functionName, object[] parameters = null)
        {
            return new Query<ShadowRow>(new FunctionCallQueryProvider(this, functionName, parameters));
        }

        public IEnumerable<ShadowRow> QueryToStoredProcedure(string storedProcedureName, IDictionary<string, object> parameters = null)
        {
            return QueryToStoredProcedureInternal(storedProcedureName, parameters);
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

            return QueryToStoredProcedureInternal(storedProcedureName, parametersMap);
        }

        private IEnumerable<ShadowRow> QueryToStoredProcedureInternal(string storedProcedureName,
            IDictionary<string, object> parameters = null)
        {
            var cmd = Connection.Value.CreateCommand();
            var paramsSql = string.Empty;
            if (parameters != null && parameters.Any())
            {
                var paramNames = new List<string>();
                foreach (var param in parameters)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = param.Key;
                    p.Value = param.Value;
                    cmd.Parameters.Add(p);

                    paramNames.Add($"@{param.Key}");
                }
                paramsSql = string.Join(", ", paramNames);
            }

            using (cmd)
            {
                cmd.CommandText = string.Format(SqlTemplates.sqlSp, storedProcedureName, paramsSql);
                return cmd.ReadAll();
            }
        }

        public IQueryable<ShadowRow> QueryToTable(string sourceName)
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
