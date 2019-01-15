using System;
using System.Linq.Expressions;
using ExpToSql.Visitors;

namespace ExpToSql.QueryProviders
{
    class FunctionCallQueryProvider: QueryProvider
    {
        private readonly string _functionName;
        private readonly object[] _parameters;

        private readonly DatabaseContext _db;

        public FunctionCallQueryProvider(DatabaseContext db,
            string functionName,
            object[] parameters = null)
        {
            _db = db;
            _functionName = functionName;
            _parameters = parameters;
        }

        public override object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public override TResult Execute<TResult>(Expression expression)
        {
            var sql = new FunctionCallQueryTranslator(_functionName, _parameters)
                .Translate2Sql(expression);

            using (var cmd = _db._db.Value.CreateCommand())
            {
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    return (TResult)reader.ReadAll();
                }
            }
        }
    }
}
