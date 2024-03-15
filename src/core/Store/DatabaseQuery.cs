using Swan.Core.Helper;

namespace Swan.Core.Store
{
    public class DatabaseQuery
    {
        private readonly Dictionary<string, object> _query;

        public DatabaseQuery()
        {
            _query = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public void Add(string key, object value)
        {
            if(_query.ContainsKey(key))
            {
                throw new Exception($"Query already contains key {key}.");
            }

            _query[StringHelper.Underscored(key)] = value;
        }

        public override string ToString()
        {
            if(!_query.Any())
            {
                return string.Empty;
            }

            return $"WHERE {string.Join(" AND ", _query.Select(x => $"{x.Key}='{x.Value}'"))}";
        }
    }
}
