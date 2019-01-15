using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExpToSql.Visitors
{
    abstract class QueryTranslator: ExpressionVisitor
    {
        protected readonly StringBuilder _sb = new StringBuilder();

        public abstract string Translate2Sql(Expression expr);
    }
}
