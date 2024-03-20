using System.ComponentModel.DataAnnotations;
using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Inputs
{
    /// <summary>
    /// This is a input for a <see cref="DateTime"/>
    /// </summary>
    public class UICInputDatetime : UICInput<DateTime?>
    {
        #region Fields
        public override bool HasClientSideValidation => ValidationRequired || ValidationMinimumDate.HasValue || ValidationMaximumDate.HasValue;
        public override string RenderLocation => Defaults.Models.Inputs.UICInputDatetime.RenderLocation;
        #endregion

        #region ctor
        public UICInputDatetime() : this(null)
        {

        }
        public UICInputDatetime(string propertyName) : base(propertyName)
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Choose to show date only, minutes or seconds
        /// </summary>
        public UICDatetimeStep Precision { get; set; } = Defaults.Models.Inputs.UICInputDatetime.Precision;

        public bool ValidationRequired { get; set; } = Defaults.Models.Inputs.UICInputDatetime.ValidationRequired;

        public DateTime? ValidationMinimumDate { get; set; }

        public DateTime? ValidationMaximumDate { get; set; }

        #endregion
    }
}

namespace UIComponents.Defaults.Models.Inputs
{
    public static class UICInputDatetime
    {
        #region Fields
        public static string RenderLocation { get; set; } =  UIComponent.DefaultIdentifier(nameof(UICInputDatetime));
        #endregion


        #region Properties

        /// <summary>
        /// Choose to show date only, minutes or seconds
        /// </summary>
        public static UICDatetimeStep Precision { get; set; } = UICDatetimeStep.Minute;

        public static bool ValidationRequired { get; set; }

        #endregion
    }
}



