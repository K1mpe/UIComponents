using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Models.HtmlResponses
{
    public class UICHtmlResponseRedirect : IHtmlResponse
    {
        public string Type => "Redirect";
        public string Url { get; set; }
    }
}
