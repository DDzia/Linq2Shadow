using System.Linq.Expressions;
using System.Text;

namespace Linq2Shadow.QueryTranslators
{
    internal abstract class QueryTranslator: ExpressionVisitor
    {
        protected readonly StringBuilder _sb = new StringBuilder();

        protected readonly QueryParametersStore _queryParamsStore;

        protected internal QueryTranslator(QueryParametersStore queryParamsStore)
        {
            _queryParamsStore = queryParamsStore;
        }

        public abstract string TranslateToSql(Expression expr);
    }
}
