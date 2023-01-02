using System.Collections;

namespace Templar.Containers
{
    internal class ComponentsContainer : IComponents
    {
        private readonly List<Component> _components = new();
        public TemplarComponent? this[string key]
        {
            get
            {
                return _components
                    .FirstOrDefault(f => f.Key == key)?
                    .Activate();
            }
        }
        public void Add<T>()
            where T : TemplarComponent
        {
            var type = typeof(T);
            _components.Add(new Component(type.Name.Replace("Component", ""), type));
        }

        public IEnumerator<IComponent> GetEnumerator()
        {
            return _components.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public class Component : IComponent
        {
            public string Key { get; }
            private readonly Type _type;
            public Component(string key, Type type)
            {
                Key = key;
                _type = type;
            }
            public TemplarComponent Activate()
            {
                if (Activator.CreateInstance(_type) is TemplarComponent comp)
                    return comp;
                throw new NullReferenceException($"Cannot activate type {Key} because it is not a TemplarComponent");
            }
        }
    }
    public interface IComponent
    {
        string Key { get; }
        TemplarComponent Activate();
    }
    public interface IComponents : IEnumerable<IComponent>
    {
        TemplarComponent? this[string key] { get; }
        void Add<T>() where T : TemplarComponent;
    }
}
