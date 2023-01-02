using System.Text.RegularExpressions;
using Templar.Containers;

namespace Templar
{
    public class ComponentBuilder
    {
        private readonly string _file;
        private readonly IComponents _components;
        private readonly IParameters _parameters;
        private static readonly Regex _componentRegex = new(@"(?<=\{Component:).+?(?=\})");
        public ComponentBuilder(string file, IComponents components, IParameters parameters)
        {
            _file = file;
            _components = components;
            _parameters = parameters;
        }
        public IEnumerable<Result> GetComponents()
        {
            foreach (var component in _components)
            {
                var matches = _componentRegex.Matches(_file);
                foreach (Match match in matches.Cast<Match>())
                {
                    var query = match.Value;
                    if (!query.StartsWith(component.Key))
                        continue;
                    query = query[component.Key.Length..];
                    if (!string.IsNullOrWhiteSpace(query))
                        _parameters.AddQuery(query);
                    yield return new Result(component, "{Component:" + match.Value + "}");
                }
            }
        }
        public class Result
        {
            public IComponent Component { get; }
            public string Tag { get; }
            public Result(IComponent component, string tag)
            {
                Component = component;
                Tag = tag;
            }
        }
    }
}
