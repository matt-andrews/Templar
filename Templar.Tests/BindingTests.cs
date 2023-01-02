using Templar.Containers;
using Templar.Tests.Mocks;

namespace Templar.Tests
{
    internal class BindingTests
    {
        [Test]
        public void TestSingleBinding()
        {
            var comp = new Test1Component()
            {
                P1 = "Hello, World"
            };
            var parameters = new ParametersContainer();
            var builder = new BindingsBuilder(comp, parameters);
            Assert.That(builder.Build("@*This is some commented out stuff*@<title>@P1</title>"),
                Is.EqualTo("<title>Hello, World</title>"));
        }
        [Test]
        public void TestMultiLineComment()
        {
            var comp = new Test1Component()
            {
                P1 = "Hello, World"
            };
            var parameters = new ParametersContainer();
            var builder = new BindingsBuilder(comp, parameters);
            Assert.That(builder.Build(
                "@*This is some commented\r\n" +
                "out stuff \r\n" +
                "<title>@P1</title>*@"
                ),
                Is.EqualTo(string.Empty));
        }
        [Test]
        public void TestMultipleBindings()
        {
            var comp = new Test1Component()
            {
                P1 = "Hello",
                P2 = 42,
                P3 = true,
                P4 = new TestNestedObject()
                {
                    Data = "World"
                }
            };
            var parameters = new ParametersContainer();
            var builder = new BindingsBuilder(comp, parameters);
            Assert.That(builder.Build("@P1@P2 \"@P3\" @P4.ToString()"), Is.EqualTo("Hello42 \"True\" World"));
        }
        [Test]
        public void TestNestedBindings()
        {
            var comp = new Test1Component()
            {
                P4 = new TestNestedObject()
                {
                    Data = "Hello"
                }
            };
            var parameters = new ParametersContainer();
            var builder = new BindingsBuilder(comp, parameters);
            Assert.Multiple(() =>
            {
                Assert.That(builder.Build("@P4.Data"), Is.EqualTo("Hello"));
                Assert.That(builder.Build("param=\"@P4.Data\""), Is.EqualTo("param=\"Hello\""));
            });
        }

        [Test]
        public void TestDoubleNestedBindings()
        {
            var comp = new Test1Component();
            var parameters = new ParametersContainer();
            var builder = new BindingsBuilder(comp, parameters);
            Assert.That(builder.Build("@P5.Data.Data"), Is.EqualTo("Hello"));
        }
        [Test]
        public void TestFileOffsetAfterBindings()
        {
            var comp = new Test1Component()
            {
                P1 = "I am super duper long"
            };
            var parameters = new ParametersContainer();
            var builder = new BindingsBuilder(comp, parameters);
            Assert.That(builder.Build("@P1 @P1"), Is.EqualTo("I am super duper long I am super duper long"));
        }
        [Test]
        public void TestNonPublicProperties()
        {
            var comp = new Test1Component();
            var parameters = new ParametersContainer();
            var builder = new BindingsBuilder(comp, parameters);
            Assert.That(builder.Build("@P6 @_p7 @P8"), Is.EqualTo("Protected Property @_p7 Private Property"));
        }
        private class Test1Component : MockComponent
        {
            public string? P1 { get; set; }
            public int P2 { get; set; }
            public bool P3 { get; set; }
            public TestNestedObject P4 { get; set; } = new();
            public TestDoubleNestedObject P5 { get; set; } = new();
            protected string P6 { get; set; } = "Protected Property";
            private string _p7 = "Private Field";
            private string P8 { get; set; } = "Private Property";
        }
        private class TestNestedObject
        {
            public string Data { get; set; } = "";
            public override string ToString()
            {
                return Data;
            }
        }
        private class TestDoubleNestedObject
        {
            public TestNestedObject Data { get; set; } = new TestNestedObject() { Data = "Hello" };
        }
    }
}
