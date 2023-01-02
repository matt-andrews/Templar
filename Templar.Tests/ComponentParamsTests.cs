using Templar.Attributes;
using Templar.Containers;
using Templar.Tests.Mocks;

namespace Templar.Tests
{
    public class ComponentParamsTests
    {
        [TestCase(" hello=\"Hello, World\"", "hello:Hello, World")]
        [TestCase(" p1=\"Hello\" p2=\", \" p3=\"World\"", "p1:Hello", "p2:, ", "p3:World")]
        [TestCase(" p1=\"This is a string with a lot of spaces\"", "p1:This is a string with a lot of spaces")]
        [TestCase(" p1=\"This = this\"", "p1:This = this")]
        [TestCase(" p1=\"'Quote'\"", "p1:'Quote'")]
        //TODO: Nested double quotes
        public void TestComponentParamQueryString2(string query, params string[] answer)
        {
            var parameters = new ParametersContainer();
            parameters.AddQuery(query);
            foreach (var a in answer)
            {
                var split = a.Split(':');
                Assert.That(split[1], Is.EqualTo(parameters[split[0], typeof(string)]));
            }
            Assert.Pass();
        }
        [Test]
        public void TestRefParameters()
        {
            var baseComponent = new TestRefParamBase();
            var components = new MockComponents();
            components.Add<TestRefParamComponent>();
            var parameters = new ParametersContainer();
            var bindings = new BindingsBuilder(baseComponent, parameters);
            string file = bindings.Build("{Component:TestRefParam p1=\"@BindingRef\"}");
            var componentBuilder = new ComponentBuilder(file, components, parameters);
            foreach (var comp in componentBuilder.GetComponents())
            {
                if (comp.Component.Activate() is TestRefParamComponent test)
                {
                    test.SetParameters(parameters);
                    Assert.Multiple(() =>
                    {
                        Assert.That(test.P1 is not null);
                        Assert.That(test.P1?.Data, Is.EqualTo("Hello"));
                    });
                }
                else
                {
                    Assert.Fail();
                }
            }
        }
        [Test]
        public void TestEmptyParameters()
        {
            var baseComponent = new TestRefParamBase();
            var components = new MockComponents();
            components.Add<TestRefParamComponent>();
            var parameters = new ParametersContainer();
            var bindings = new BindingsBuilder(baseComponent, parameters);
            string file = bindings.Build("{Component:TestRefParam}");
            var componentBuilder = new ComponentBuilder(file, components, parameters);
            foreach (var comp in componentBuilder.GetComponents())
            {
                if (comp.Component.Activate() is TestRefParamComponent test)
                {
                    test.SetParameters(parameters);
                }
                else
                {
                    Assert.Fail();
                }
            }
        }

        private class TestRefParamBase : MockComponent
        {
            public TestRef BindingRef { get; set; } = new TestRef();
        }
        private class TestRefParamComponent : MockComponent
        {
            [Parameter]
            public TestRef P1 { get; set; } = default!;
        }
        private class TestRef
        {
            public string Data { get; set; } = "Hello";
        }
    }
}