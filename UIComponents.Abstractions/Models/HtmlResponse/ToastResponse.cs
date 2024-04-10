using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Models.HtmlResponse
{
    public class ToastResponse : IHtmlResponse
    {
        public string type => "ToastResponse";

        public object notification { get; set; }

        public object data { get; set; }
    }
}
