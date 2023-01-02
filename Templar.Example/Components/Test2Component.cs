using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Templar.Attributes;

namespace Templar.Example.Components
{
    internal class Test2Component : TemplarComponent
    {
        public override string TemplatePath => "templates/test2.html";
        [Parameter]
        public string TestParam { get; set; }
    }
}
