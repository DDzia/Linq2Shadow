using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using ExpToSql.QueryProviders;

namespace ExpToSql
{
    interface IFromSpWithDictionary
    {
        IEnumerable<DynamicRow> FromStoredProcedure(string storedProcedureName,
            IDictionary<string, object> parameters = null);
    }

    public class DatabaseContext: IFromSpWithDictionary, IDisposable
    {
        internal readonly Lazy<IDbConnection> _db;

        public DatabaseContext(Lazy<IDbConnection> db)
        {
            _db = new Lazy<IDbConnection>(() =>
            {
                if (db.Value.State == ConnectionState.Closed)
                {
                    db.Value.Open();
                }

                return db.Value;
            });
        }

        public IQueryable<DynamicRow> FromTableFunction(string functionName, object[] parameters = null)
        {
            //var p = Expression.Parameter(typeof(DynamicRow), "item");
            //var label = Expression.Label(typeof(DynamicRow));
            //var returnExpr = Expression.Return(label, p, typeof(DynamicRow));
            //var tExpr = Expression.Lambda<Func<DynamicRow, DynamicRow>>(p);
            //var tDelegate = tExpr.Compile();
            // Expression.Return()

            // Expression.Dynamic()

            // Expression.


            return new Query<DynamicRow>(new FunctionCallQueryProvider(this, functionName, parameters));
        }

        public IEnumerable<DynamicRow> FromStoredProcedure(string storedProcedureName, IDictionary<string, object> parameters = null)
        {
            var cmd = _db.Value.CreateCommand();

            var sb = new StringBuilder();
            sb.AppendLine("DECLARE @return_value int");
            sb.AppendLine($"EXEC @return_value = {storedProcedureName}");

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
            cmd.CommandText = sb.ToString();

            using (var reader = cmd.ExecuteReader())
            {
                return reader.ReadAll();
            }
        }

        public IEnumerable<DynamicRow> FromStoredProcedure<T>(string storedProcedureName, T parameters = null)
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

            return (this as IFromSpWithDictionary).FromStoredProcedure(storedProcedureName, parametersMap);
        }

        public IQueryable<dynamic> FromSource(string sourceName)
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_db.IsValueCreated) _db.Value.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
