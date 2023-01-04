using Templar.Attributes;
using Templar.Containers;
using Templar.Tests.Mocks;

namespace Templar.Tests
{
    internal class ComponentBuildingTests
    {
        [Test]
        public void SingleComponentNoParams()
        {
            var components = new MockComponents();
            components.Add<Test3Component>();
            var parameters = new ParametersContainer();
            var componentBuilder = new ComponentBuilder("{Component:Test3}", components, parameters);

            foreach (var r in componentBuilder.GetComponents())
            {
                var component = r.Component.Activate();
                if (component is Test3Component test1)
                {
                    test1.SetParameters(parameters);
                }
                else
                {
                    Assert.Fail();
                }
            }
        }
        [Test]
        public void SingleComponent()
        {
            var components = new MockComponents();
            components.Add<Test1Component>();
            var parameters = new ParametersContainer();
            var componentBuilder = new ComponentBuilder("{Component:Test1 p1=\"Hello, World\"}", components, parameters);

            foreach (var r in componentBuilder.GetComponents())
            {
                var component = r.Component.Activate();
                if (component is Test1Component test1)
                {
                    test1.SetParameters(parameters);
                    Assert.That(test1.P1, Is.EqualTo("Hello, World"));
                }
                else
                {
                    Assert.Fail();
                }
            }
        }
        [Test]
        public void MultipleComponents()
        {
            var components = new MockComponents();
            components.Add<Test1Component>();
            components.Add<Test2Component>();
            var parameters = new ParametersContainer();
            var componentBuilder = new ComponentBuilder(
                "{Component:Test1 p1=\"The secret of\"}{Component:Test2 p1=\"the universe\" p2=\"42\" p3=\"true\"}",
                components, parameters);

            foreach (var r in componentBuilder.GetComponents())
            {
                var component = r.Component.Activate();
                if (component is Test1Component test1)
                {
                    test1.SetParameters(parameters);
                    Assert.That(test1.P1, Is.EqualTo("The secret of"));
                }
                else if (component is Test2Component test2)
                {
                    test2.SetParameters(parameters);
                    Assert.Multiple(() =>
                    {
                        Assert.That(test2.P1, Is.EqualTo("the universe"));
                        Assert.That(test2.P2, Is.EqualTo(42));
                        Assert.That(test2.P3, Is.EqualTo(true));
                    });
                }
                else
                {
                    Assert.Fail();
                }
            }
        }
        [Test]
        public void MultipleIdenticalComponents()
        {
            var components = new MockComponents();
            components.Add<Test2Component>();
            var parameters = new ParametersContainer();
            var componentBuilder = new ComponentBuilder(
                "{Component:Test2 p1=\"First Component\" p2=\"23\" p3=\"false\"}" +
                "{Component:Test2 p1=\"Second Component\" p2=\"42\" p3=\"true\"}",
                components, parameters);
            int index = 0;
            foreach (var r in componentBuilder.GetComponents())
            {
                var component = r.Component.Activate();
                if (component is Test2Component test2)
                {
                    test2.SetParameters(parameters);
                    if (index++ == 0)
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(test2.P1, Is.EqualTo("First Component"));
                            Assert.That(test2.P2, Is.EqualTo(23));
                            Assert.That(test2.P3, Is.EqualTo(false));
                        });
                    }
                    else
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(test2.P1, Is.EqualTo("Second Component"));
                            Assert.That(test2.P2, Is.EqualTo(42));
                            Assert.That(test2.P3, Is.EqualTo(true));
                        });
                    }
                }
                else
                {
                    Assert.Fail();
                }
            }
        }
        [Test]
        public void TestSimilarComponentIdentity()
        {
            var components = new MockComponents();
            components.Add<ButtonComponent>();
            components.Add<ButtonCheckboxComponent>();
            components.Add<ButtonCheckboxGroupComponent>();
            var parameters = new ParametersContainer();
            {
                var componentBuilder = new ComponentBuilder(
                    "{Component:Button} {Component:ButtonCheckbox} {Component:ButtonCheckboxGroup}",
                    components, parameters);
                bool grp = false;
                bool chk = false;
                bool btn = false;
                foreach (var c in componentBuilder.GetComponents())
                {
                    var component = c.Component.Activate();
                    if (component is ButtonCheckboxGroupComponent)
                    {
                        grp = true;
                    }
                    else if (component is ButtonCheckboxComponent)
                    {
                        chk = true;
                    }
                    else if (component is ButtonComponent)
                    {
                        btn = true;
                    }
                }

                Assert.Multiple(() =>
                {
                    Assert.That(grp, Is.True);
                    Assert.That(chk, Is.True);
                    Assert.That(btn, Is.True);
                });
            }

            {
                var componentBuilder = new ComponentBuilder(
                    "{Component:ButtonCheckboxGroup} {Component:ButtonCheckbox} {Component:Button}",
                    components, parameters);
                bool grp = false;
                bool chk = false;
                bool btn = false;
                foreach (var c in componentBuilder.GetComponents())
                {
                    var component = c.Component.Activate();
                    if (component is ButtonCheckboxGroupComponent)
                    {
                        grp = true;
                    }
                    else if (component is ButtonCheckboxComponent)
                    {
                        chk = true;
                    }
                    else if (component is ButtonComponent)
                    {
                        btn = true;
                    }
                }

                Assert.Multiple(() =>
                {
                    Assert.That(grp, Is.True);
                    Assert.That(chk, Is.True);
                    Assert.That(btn, Is.True);
                });
            }
            Assert.Pass();
        }
        private class Test1Component : MockComponent
        {
            [Parameter]
            public string? P1 { get; set; }
        }
        private class Test2Component : MockComponent
        {
            [Parameter]
            public string? P1 { get; set; }
            [Parameter]
            public int P2 { get; set; }
            [Parameter]
            public bool P3 { get; set; }
        }
        private class Test3Component : MockComponent
        { }
        private class ButtonComponent : MockComponent
        { }
        private class ButtonCheckboxComponent : MockComponent
        { }
        private class ButtonCheckboxGroupComponent : MockComponent
        { }
    }
}