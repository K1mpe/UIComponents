using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Attributes;
using UIComponents.Abstractions.Models.RecurringDates;
using UIComponents.Web.Tests.Models;

namespace UIComponents.Web.Tests.Models
{
    public class TestModel
    {
        [UICTooltip("Dit is een test")]
        public string TestString { get; set; }

        public bool Checkbox { get; set; }

        public bool? ThreeStateBool { get; set; }

        [UICSpan("Dit is een spantekst")]
        public int Number { get; set; }

        public double Decimal { get; set; }

        [DisplayName("blub")]
        public DateTime Date { get; set; }

        public TimeSpan TimeSpan { get; set; }

        public TestEnum Enum { get; set; }

        public string Color { get; set; }

        public RecurringDate RecurringDate { get; set; }


    }

    public enum TestEnum
    {
        None = 0,
        One = 1, 
        Two = 2,
    }

    [UICInherit(typeof(TestModel))]
    public class TestModel2
    {
        public string TestString { get; set; }

    }
}
