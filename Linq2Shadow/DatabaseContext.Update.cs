using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Linq2Shadow.QueryTranslators.Where;

namespace Linq2Shadow
{
    public partial class DatabaseContext
    {
        private static Dictionary<string, object> ExtractMapFromType<T>(T obj)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            var props = typeof(T).GetProperties(bindingFlags)
                .Where(x => x.GetMethod != null && x.GetMethod.IsPublic)
                .Select(x => new Tuple<string, object>(x.Name, x.GetMethod.Invoke(obj, new object[0])));

            var fields = typeof(T).GetFields(bindingFlags)
                .Where(x => x.IsPublic)
                .Select(x => new Tuple<string, object>(x.Name, x.GetValue(obj)));

            var result = new Dictionary<string, object>();
            foreach (var pKeyValue in props)
            {
                result[pKeyValue.Item1] = pKeyValue.Item2;
            }
            foreach (var fKeyValue in fields)
            {
                result[fKeyValue.Item1] = fKeyValue.Item2;
            }

            return result;
        }

        private static void ValidateUpdateArguments(string updateTarget,
            Dictionary<string, object> updateFields)
        {
            if (updateTarget is null)
                throw new ArgumentNullException(nameof(updateTarget));
            if (string.IsNullOrWhiteSpace(updateTarget))
                throw new ArgumentException("Whitespace is not available.", nameof(updateTarget));

            if (updateFields is null)
                throw new ArgumentNullException(nameof(updateFields));
            if (!updateFields.Any())
                throw new ArgumentException("Has no fields to update.", nameof(updateFields));
            if (updateFields.Keys.Any(x => string.IsNullOrWhiteSpace(x)))
                throw new ArgumentException("One of fields is invalid.", nameof(updateFields));
        }

        /// <summary>
        /// Update data of source.
        /// </summary>
        /// <param name="updateTarget">Data source to update.</param>
        /// <param name="updateFields">Map of members and new values.</param>
        /// <param name="predicate">Filter predicate to update.</param>
        /// <returns>Count of updated rows.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="updateTarget"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="updateTarget"/> is whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="updateFields"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="updateFields"/> is empty map.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="updateFields"/> has whitespace key.</exception>
        public int Update(string updateTarget,
            Dictionary<string, object> updateFields,
            Expression<Func<ShadowRow, bool>> predicate = null)
        {
            ValidateUpdateArguments(updateTarget, updateFields);

            return CreateUpdateCommand(updateTarget, updateFields, predicate)
                .ExecuteNonQuery();
        }

        /// <summary>
        /// Update data of source.
        /// </summary>
        /// <param name="updateTarget">Data source to update.</param>
        /// <param name="updateFields">Map of members and new values.</param>
        /// <param name="predicate">Filter predicate to update.</param>
        /// <returns>Count of updated rows.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="updateTarget"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="updateTarget"/> is whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="updateFields"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="updateFields"/> has no members.</exception>
        public int Update<T>(string updateTarget,
            T updateFields,
            Expression<Func<ShadowRow, bool>> predicate = null)
        where T: class
        {
            if (updateFields == null)
                throw new ArgumentNullException(nameof(updateFields));

            var memeberValueMap = ExtractMapFromType(updateFields);
            return Update(updateTarget, memeberValueMap, predicate);
        }

        /// <summary>
        /// Update data of source asynchronously.
        /// </summary>
        /// <param name="updateTarget">Data source to update.</param>
        /// <param name="updateFields">Map of members and new values.</param>
        /// <param name="predicate">Filter predicate to update.</param>
        /// <param name="cancellationToken">Operation cancellation.</param>
        /// <returns>Count of updated rows.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="updateTarget"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="updateTarget"/> is whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="updateFields"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="updateFields"/> is empty map.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="updateFields"/> has whitespace key.</exception>
        public Task<int> UpdateAsync(string updateTarget,
            Dictionary<string, object> updateFields,
            Expression<Func<ShadowRow, bool>> predicate = null,
            CancellationToken cancellationToken = default)
        {
            ValidateUpdateArguments(updateTarget, updateFields);

            return CreateUpdateCommand(updateTarget, updateFields, predicate)
                .ExecuteNonQueryAsync(cancellationToken);
        }

        /// <summary>
        /// Update data of source asynchronously.
        /// </summary>
        /// <param name="updateTarget">Data source to update.</param>
        /// <param name="updateFields">Map of members and new values.</param>
        /// <param name="predicate">Filter predicate to update.</param>
        /// <param name="cancellationToken">Operation cancellation.</param>
        /// <returns>Count of updated rows.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="updateTarget"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="updateTarget"/> is whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="updateFields"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="updateFields"/> has no members.</exception>
        public Task<int> UpdateAsync<T>(string updateTarget,
            T updateFields,
            Expression<Func<ShadowRow, bool>> predicate = null,
            CancellationToken cancellationToken = default)
        where T: class
        {
            if (updateFields is null)
                throw new ArgumentNullException(nameof(updateFields));

            var memeberValueMap = ExtractMapFromType(updateFields);
            return CreateUpdateCommand(updateTarget, memeberValueMap, predicate)
                .ExecuteNonQueryAsync(cancellationToken);
        }

        private DbCommand CreateUpdateCommand(string updateTarget,
            Dictionary<string, object> updateFields,
            Expression<Func<ShadowRow, bool>> predicate = null)
        {
            var sb = new StringBuilder()
                .Append("UPDATE ")
                .Append(updateTarget);

            var store = new QueryParametersStore();
            var fieldPNameMap = new Dictionary<string, string>();
            foreach (var updateField in updateFields)
            {
                fieldPNameMap[updateField.Key] = store.Append(updateField.Value);
            }


            {
                var setQueryBuilder = new StringBuilder();
                var loopCounter = 0;
                foreach (var field in fieldPNameMap.Keys)
                {
                    setQueryBuilder.Append($"{field}={fieldPNameMap[field]}");

                    var itIsLast = loopCounter == fieldPNameMap.Count - 1;
                    if (!itIsLast)
                    {
                        setQueryBuilder.Append(", ");
                    }

                    loopCounter++;
                }

                sb.Append(" SET ");
                sb.Append(setQueryBuilder);
            }

            if (predicate != null)
            {
                var w = new WhereQueryTranslator(store, new[] { predicate });
                var whereSql = w.TranslateToSql(Expression.Empty());

                if (!string.IsNullOrWhiteSpace(whereSql))
                {
                    sb.Append(" WHERE ");
                    sb.Append(whereSql);
                }
            }


            var cmd = Connection.Value.CreateCommand();
            cmd.CommandText = sb.ToString();
            foreach (var param in store.GetParams())
            {
                var p = cmd.CreateParameter();
                p.ParameterName = param.Key;
                p.Value = param.Value;
                cmd.Parameters.Add(p);
            }

            return cmd;
        }
    }
}
