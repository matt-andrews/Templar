using System.Reflection;
using System.Text.RegularExpressions;
using Templar.Containers;

namespace Templar
{
    public class BindingsBuilder
    {
        private readonly TemplarComponent _component;
        private readonly IParameters _parameters;
        private readonly static Regex _commentRegex = new(@"\@\*([^)]*)\*\@");
        public BindingsBuilder(TemplarComponent component, IParameters parameters)
        {
            _component = component;
            _parameters = parameters;
        }
        public string Build(string file)
        {
            file = RemoveComments(file);
            var bindingProperties = _component.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance)
                .OrderByDescending(o => o.Name.Length);
            foreach (var prop in bindingProperties)
            {
                var matches = Regex.Matches(file, $@"@{prop.Name}([\.\(\)a-zA-Z0-9]+)?");
                int fileOffset = 0;
                foreach (Match match in matches.Cast<Match>())
                {
                    var obj = prop.GetValue(_component);
                    if (match.Groups.Count > 1 && !string.IsNullOrWhiteSpace(match.Groups[1].Value))
                    {
                        if (match.Groups[1].Value.StartsWith('.'))
                        {
                            var callings = match.Groups[1].Value.TrimStart('.').Split('.');
                            foreach (var caller in callings)
                            {
                                var callingProp = obj?.GetType().GetProperty(caller);
                                if (callingProp is null)
                                {
                                    if (caller == "ToString()")
                                        obj = obj?.ToString();
                                    break;
                                }
                                obj = callingProp.GetValue(obj);
                            }
                        }
                    }
                    if (obj is null || typeof(IConvertible).IsAssignableFrom(obj.GetType()))
                    {
                        var replacement = ReplaceSingle(file, match, obj?.ToString() ?? string.Empty, fileOffset);
                        file = replacement.File;
                        fileOffset = replacement.Offset;
                    }
                    else
                    {
                        var replacement = ReplaceSingle(file, match, _parameters.AddReference(obj), fileOffset);
                        file = replacement.File;
                        fileOffset = replacement.Offset;
                    }
                }
            }
            return file;
        }
        private static string RemoveComments(string file)
        {
            return _commentRegex.Replace(file, "");
        }
        private static (string File, int Offset) ReplaceSingle(string file, Match match, string replacement, int offset)
        {
            string first = file[..(match.Index + offset)];
            string last = file[(offset + match.Index + match.Length)..];
            return (first + replacement + last, replacement.Length - match.Length + offset);
        }
    }
}
