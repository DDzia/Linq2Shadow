using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Linq2Shadow.QueryProviders
{
    internal class FunctionCallQueryProvider: QueryProvider
    {
        private readonly string _functionName;
        private readonly object[] _functionParameters;

        private readonly DatabaseContext _db;

        internal FunctionCallQueryProvider(DatabaseContext db,
            string functionName,
            object[] functionParameters = null)
        {
            _db = db;
            _functionName = functionName;
            _functionParameters = functionParameters;
        }

        public override TResult Execute<TResult>(Expression expression)
        {
            return new FromSourceQueryProvider(_db, CreateSyntheticSourceName(), _queryParamsStore)
                .Execute<TResult>(expression);
        }

        public override IEnumerator<T> GetEnumerator<T>(Expression expression)
        {
            return new FromSourceQueryProvider(_db, CreateSyntheticSourceName(), _queryParamsStore)
                .GetEnumerator<T>(expression);
        }

        private string CreateSyntheticSourceName()
        {
            var sb = new StringBuilder();
            sb.Append(_functionName);

            sb.Append("(");
            if (_functionParameters != null)
            {
                var pNames = _functionParameters.Select(x => _queryParamsStore.Append(x));
                sb.Append(string.Join(", ", pNames));
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}
