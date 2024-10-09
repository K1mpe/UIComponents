using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Attributes;
using UIComponents.Abstractions.DataTypes;
using UIComponents.Abstractions.DataTypes.RecurringDates;
using UIComponents.Web.Tests.Models;
using UIComponents.Web.Tests.Validators;

namespace UIComponents.Web.Tests.Models
{
    public class TestModel
    {
        [UICTooltip("Dit is een test")]
        [UICTooltipIcon("fas fa-warning text-warning")]
        [MinLength(3)]
        public string TestString { get; set; } = "abc";

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

        public TestEnum Enum { get; set; } = TestEnum.Two;

        public string Color { get; set; }

        public RecurringDate RecurringDate { get; set; }

        public TestModel SubModel { get; set; }

        public List<string> IntList { get; set; }
        [ReadOnly(true)]
        public List<TestModel2> ObjectList { get; set; }


        [UICPropertyType(UICPropertyType.DateTime)]
        [UICPrecisionDate(UICDatetimeStep.Second)]
        public DateTime? DateTime { get; set; }

        [UICPropertyType(UICPropertyType.DateOnly)]
        public DateTime Date { get; set; }

        [UICPropertyType(UICPropertyType.TimeOnly)]
        [UICPrecisionTime(UICTimeonlyEnum.Second)]
        public DateTime? TimeOnly { get; set; }


        public UICReferenceValues<TestModel2> References { get; set; } = new UICReferenceValues<TestModel2>().AssignProperties(x=>x.TestModel2Bool);

        public TestModel2 SubClass { get; set; } = new();

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

        public bool TestModel2Bool { get; set; }
    }
}
