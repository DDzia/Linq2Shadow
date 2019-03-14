using System.Linq;
using System.Linq.Expressions;

namespace Linq2Shadow.QueryTranslators.SelectOnly
{
    class SelectOnlyTranslator: QueryTranslator
    {
        internal SelectOnlyTranslator()
            :base(new QueryParametersStore()) {}

        public override string TranslateToSql(Expression expr)
        {
            var ejector = new SelectOnlyEjector();
            ejector.Visit(expr);
            if (!ejector.FieldNames?.Any() ?? true)
            {
                return "*";
            }

            for (int i = 0; i < ejector.FieldNames.Length; i++)
            {
                _sb.Append(ejector.FieldNames[i]);

                var itLast = i == ejector.FieldNames.Length - 1;
                if (!itLast) _sb.Append(", ");
            }

            return _sb.ToString();
        }
    }
}
