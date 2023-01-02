using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Templar.Example.Components
{
    internal class Test1Component : TemplarComponent
    {
        public override string TemplatePath => "";
        protected override string GetFile(IStaticSiteService siteService)
        {
            return "<li>You can use components to modularize your website.</li>";
        }

    }
}
