using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Models.HtmlResponses
{
    public class UICHtmlResponseAccessDenied : IHtmlResponse
    {
        public string Type => "AccessDenied";
        public Translatable Message { get; set; }
    }
}
