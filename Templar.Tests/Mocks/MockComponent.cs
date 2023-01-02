using Templar.Containers;

namespace Templar.Tests.Mocks
{
    internal abstract class MockComponent : TemplarComponent
    {
        public override string TemplatePath => "";
        public new void SetParameters(IParameters parameters)
        {
            base.SetParameters(parameters);
        }
    }
}
