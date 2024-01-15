using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Attributes;

namespace UIComponents.Web.Tests.Models
{
    public class TestModel
    {
        [UICTooltip("Test", "Dit is een test")]
        public string TestString { get; set; }  

    }
}
