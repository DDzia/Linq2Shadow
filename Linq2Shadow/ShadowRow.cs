using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Linq2Shadow
{
    [DebuggerDisplay("Count = {DebuggerView}")]
    public class ShadowRow : DynamicObject, IReadOnlyDictionary<string, object>
    {
        public static implicit operator Dictionary<string, object>(ShadowRow a)
        {
            return a._dic.ToDictionary(x => x.Key, x => x.Value);
        }

        private readonly Dictionary<string, object> _dic = new Dictionary<string, object>();

        internal ShadowRow() { }

        #region dynamic

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _dic.Keys;
        }

        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            result = null;
            return false;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = null;
            return false;
        }

        public override bool TryCreateInstance(CreateInstanceBinder binder, object[] args, out object result)
        {
            result = null;
            return false;
        }

        public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
        {
            return false;
        }

        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            return false;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if(indexes.Length > 1)
                throw new InvalidOperationException("indexes count");
            if(indexes[0]?.GetType() != typeof(string))
                throw new InvalidOperationException("invalid index type");

            var memberName = (string)indexes[0];
            var found = _dic.ContainsKey(memberName);
            result = found ? _dic[memberName] : null;

            return found;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var found = _dic.ContainsKey(binder.Name);
            result = found ? _dic[binder.Name] : null;
            return found;
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            result = null;
            return false;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;
            return false;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return false;
        }

        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            result = null;
            return false;
        }

        #endregion dynamic

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _dic.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => _dic.Count;

        public bool ContainsKey(string key) => _dic.ContainsKey(key);

        public bool TryGetValue(string key, out object value) => _dic.TryGetValue(key, out value);

        public object this[string key] => _dic[key];

        public IEnumerable<string> Keys => _dic.Keys;
        public IEnumerable<object> Values => _dic.Values;

        internal void SetValue(string key, object value) => _dic[key] = value;

        private string DebuggerView
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append("{ ");

                bool IsLast(string k) => Keys.Last() == k;

                foreach (var key in Keys)
                {
                    sb.Append(key);
                    sb.Append(":");
                    sb.Append(this[key]);

                    if (!IsLast(key)) sb.Append(",");
                    sb.Append(" ");
                }

                sb.Append("}");
                return sb.ToString();
            }
        }
    }
}
