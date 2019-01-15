using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ExpToSql.QueryTranslators;
using ExpToSql.QueryTranslators.Where;

namespace ExpToSql.Visitors
{
    sealed class FunctionCallQueryTranslator: QueryTranslator
    {
        private readonly WhereQueryTranslator whereTranslator = new WhereQueryTranslator();
        // public List<LambdaExpression> OrderByExpressions = new List<LambdaExpression>();

        private readonly string _functionName;
        private readonly object[] _parameters;

        private List<string> _selectMembers = new List<string>();

        public FunctionCallQueryTranslator(string functionName, object[] parameters = null)
        {
            _functionName = functionName;
            _parameters = parameters;
        }



        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            //var body = node.Arguments[1];
            //Visit(body);
            //// var node.Arguments[1]

            //return base.VisitMethodCall(node);


            if (node.Method.DeclaringType == typeof(Queryable))
            {
                var prevQuery = node.Arguments[0];
                Visit(prevQuery);

                //if (node.Method.Name == "OrderBy")
                //{
                //    var lambda = (node.Arguments[1] as UnaryExpression).Operand as LambdaExpression;
                //    if (lambda == null)
                //        throw new InvalidOperationException("Invalid order by lambda");
                //    OrderByExpressions.Add(lambda);
                //    //
                //    // Visit(node.Arguments[0]);
                //    //foreach (var nodeArgument in node.Arguments)
                //    //{
                //    //    Visit(nodeArgument);
                //    //}
                //}

                // sb.Append("SELECT * FROM (");


                // this.Visit(node.Arguments[0]);


                // sb.Append(") AS T WHERE ");


                // LambdaExpression lambda = (LambdaExpression)StripQuotes(node.Arguments[1]);


                // Visit(lambda.Body);
                return node;
            }

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", node.Method.Name));
        }

        public override string Translate2Sql(Expression expr)
        {
            var whereLine = whereTranslator.Translate2Sql(expr);

            _sb.Append("SELECT * FROM ");
            _sb.Append("_functionName");
            _sb.Append("()");

            if (!string.IsNullOrWhiteSpace(whereLine))
            {
                _sb.Append("WHERE ");
                _sb.Append(whereLine);
            }

            return _sb.ToString();
        }
    }
}
