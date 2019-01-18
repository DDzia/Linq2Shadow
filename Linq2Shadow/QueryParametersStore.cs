using System.Collections.Generic;
using System.Linq;

namespace Linq2Shadow
{
    internal class QueryParametersStore
    {
        private uint _paramsCreatedCount;

        private readonly Dictionary<string, object> _params = new Dictionary<string, object>();

        public string Append<T>(T value)
        {
            var name = $"@param{_paramsCreatedCount++}";
            _params[name] = value;
            return name;
        }

        public IEnumerable<KeyValuePair<string, object>> GetParams()
        {
            return _params.AsEnumerable();
        }
    }
}
