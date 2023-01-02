using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Templar.Attributes;

namespace Templar.Example.Components
{
    internal class FooterComponent : TemplarComponent
    {
        public override string TemplatePath => "";
        [Parameter]
        public string Link { get; set; } = "";
        protected override string GetFile(IStaticSiteService siteService)
        {
            return
                @"<footer>
                    <span>This website was generated with <a href=""@Link"">Templar</a></span>
                </footer>";
        }
    }
}
