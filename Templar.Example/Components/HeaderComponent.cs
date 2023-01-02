using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Templar.Example.Components
{
    internal class HeaderComponent : TemplarComponent
    {
        public override string TemplatePath => "templates/header.html";
        public string BaseUrl { get; private set; }
        protected override Task OnInitializedAsync()
        {
            BaseUrl = Request.Host.Value;
            return base.OnInitializedAsync();
        }
    }
}
