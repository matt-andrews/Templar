using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Templar.Attributes;
using Templar.Example.Services;

namespace Templar.Example.Components
{
    internal class IndexPage : TemplarComponent
    {
        public override string TemplatePath => "templates/index-body.html";
        [Inject]
        public TestService TestService { get; set; } = default!;
        public string Title { get; private set; }
        public string Bindings { get; private set; }
        protected override Task OnInitializedAsync()
        {
            Title = TestService.Data;
            Bindings = "Bindings to bind properties to your document";
            return base.OnInitializedAsync();
        }
    }
}
