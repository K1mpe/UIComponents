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
using UIComponents.Web.Tests.Validators;

namespace UIComponents.Web.Tests.Models
{
    public class TestModel
    {
        [UICTooltip("Dit is een test")]
        public string TestString { get; set; }

        [UICPropertyType(UICPropertyType.MultilineText)]
        [DisplayName("<i class='fas fa-user'></i><strong class='text-danger'>Description</strong>")]
        public string Description { get; set; }

        public bool Checkbox { get; set; }

        public bool? ThreeStateBool { get; set; }

        [UICSpan("<span class='text-warning'>Dit</span> <blub>is een <sub>spantekst</sub>  &|é@#\"'([§^è!ç{0})-¨[^$*]ù%´µ``<>\\")]
        [Range(10, 20)]
        public int Number { get; set; }

        public double Decimal { get; set; }

        [ReadOnly(true)]
        public TimeSpan TimeSpan { get; set; }

        public TestEnum Enum { get; set; }

        public string Color { get; set; }

        public RecurringDate RecurringDate { get; set; }

        public TestModel SubModel { get; set; }

        public List<string> IntList { get; set; }
        public List<TestModel2> ObjectList { get; set; }



        public DateTime DateTime { get; set; } = DateTime.Now;
        public TimeOnly TimeOnly { get; set; }
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
        public int Number { get; set; }

    }
}
