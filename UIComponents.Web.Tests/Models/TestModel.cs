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

        
        public bool Checkbox { get; set; }

        public bool? ThreeStateBool { get; set; }

        [UICSpan("TestSpan", "Dit is een spantekst")]
        public int Number { get; set; }

        public double Decimal { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan TimeSpan { get; set; }

        public TestEnum Enum { get; set; }

        public string Color { get; set; }
    }

    public enum TestEnum
    {
        None = 0,
        One = 1, 
        Two = 2,
    }
}
