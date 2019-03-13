using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Linq2Shadow.Adapters
{
    internal class DbParamsCollectionImpl: DbParameterCollection
    {
        private readonly List<DbParameter> _params = new List<DbParameter>();

        public override int Add(object value)
        {
            Insert(_params.Count, value);
            return _params.Count - 1;
        }

        public override void Clear() => _params.Clear();

        public override bool Contains(object value)
        {
            ThrowIfNotDbParameter(value);
            var p = (DbParameter)value;
            return GetParameter(p.ParameterName) != null;
        }

        public override int IndexOf(object value)
        {
            ThrowIfNotDbParameter(value);
            return _params.IndexOf((DbParameter)value);
        }

        public override void Insert(int index, object value)
        {
            ThrowIfNotDbParameter(value);

            var p = (DbParameter)value;
            if (Contains(value))
            {
                throw new InvalidOperationException($"Parameter with name '{p.ParameterName}' is already exists.");
            }

            _params.Insert(index, p);
        }

        public override void Remove(object value)
        {
            ThrowIfNotDbParameter(value);
            _params.Remove((DbParameter)value);
        }

        public override void RemoveAt(int index) =>_params.RemoveAt(index);

        public override void RemoveAt(string parameterName)
        {
            var index = _params.FindIndex(x => x.ParameterName == parameterName);
            if (index >= 0)
            {
                RemoveAt(index);
            }
        }

        protected override void SetParameter(int index, DbParameter value)
        {
            Insert(index, value);
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            //var pFound = GetParameter(parameterName);
            //if(pFound is null)
            //    throw new InvalidOperationException($"Parameter with '{parameterName}' name is not found.");

            // value.
            // Add(value.cl)

            throw new NotSupportedException(nameof(SetParameter));
        }

        public override int Count => _params.Count;
        public override bool IsFixedSize => false;
        public override bool IsReadOnly => false;
        public override bool IsSynchronized => false;

        public override object SyncRoot { get; } = new object();

        public override int IndexOf(string parameterName) => _params.FindIndex(x => x.ParameterName == parameterName);

        public override bool Contains(string value) => GetParameter(value) != null;

        public override void CopyTo(Array array, int index)
        {
            for (var i = 0; i < _params.Count; i++)
            {
                ((object[])array)[i + _params.Count] = _params[i];
            }
        }

        public override IEnumerator GetEnumerator() => _params.GetEnumerator();

        protected override DbParameter GetParameter(int index) => _params.ElementAt(index);

        protected override DbParameter GetParameter(string parameterName) => _params.FirstOrDefault(x => x.ParameterName == parameterName);

        public override void AddRange(Array values)
        {
            if(values is null)
                throw new ArgumentNullException(nameof(values));

            foreach (var value in values)
            {
                Add(value);
            }
        }

        private void ThrowIfNotDbParameter(object o)
        {
            if (!(o is DbParameter))
                throw new ArgumentException(nameof(o));
        }
    }
}
