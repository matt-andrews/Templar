using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Templar.Containers
{
    public class ParametersContainer : IParameters
    {
        private static readonly Regex _queryRegex = new(@"\""[^\"" ][^\""]*\""");
        private readonly Dictionary<string, string> _parameters;
        private readonly Dictionary<string, object> _references;
        private int _refCount;
        public ParametersContainer()
        {
            _parameters = new();
            _references = new();
        }
        public object? this[string key, Type propertyType, bool cascade = false]
        {
            get
            {
                if (_parameters.TryGetValue(key, out string? result))
                {
                    if (!cascade)
                        _parameters.Remove(key);
                    if (result.Contains("%%REF"))
                        return GetReference(result, cascade);
                    return ConvertTo(propertyType, result);
                }
                return null;
            }
        }
        public void Add(string key, string value)
        {
            key = key.ToLower();
            if (_parameters.ContainsKey(key))
                _parameters[key] = value;
            else
                _parameters.Add(key.ToLower(), value);
        }
        public string AddReference(object reference)
        {
            string key = $"%%REF{_refCount++}";
            _references.Add(key, reference);
            return key;
        }
        public void AddQuery(string query)
        {
            if (query.StartsWith(' '))
            {
                query = query.Trim();
                var matches = _queryRegex.Matches(query);
                List<(string, string)> values = new();
                int index = 0;
                foreach (var m in matches)
                {
                    string match = m.ToString() ?? "";
                    string key = $"{{{index++}}}";
                    values.Add((key, match));
                    query = query.Replace(match, key);
                }
                string[] strParameters = query.Replace("\"", "").Split(' ');
                foreach (var p in strParameters)
                {
                    var kvp = p.Split('=');
                    Add(kvp[0].Trim(), values.First(f => f.Item1 == kvp[1].Trim()).Item2.Replace("\"", ""));
                }
            }
        }
        private object? GetReference(string key, bool cascading)
        {
            if (_references.ContainsKey(key))
            {
                var reference = _references[key];
                if (!cascading)
                    _references.Remove(key);
                return reference;
            }
            return null;
        }
        private static object? ConvertTo(Type type, string value)
        {
            if (type == typeof(string))
                return value;
            try
            {
                if (Nullable.GetUnderlyingType(type) != null)
                {
                    return TypeDescriptor.GetConverter(type).ConvertFrom(value);
                }

                return Convert.ChangeType(value, type);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
    public interface IParameters
    {
        object? this[string key, Type propertyType, bool cascade = false] { get; }
        void Add(string key, string value);
        string AddReference(object reference);
        void AddQuery(string query);
    }
}
