using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Inputs
{
    /// <summary>
    /// This is a input for all number value types
    /// </summary>
    public class UICInputNumber : UICInput<double?>
    {
        #region Fields
        public override bool HasClientSideValidation => ValidationRequired || ValidationMinValue.HasValue || ValidationMaxValue.HasValue;
        #endregion

        #region Ctor
        public UICInputNumber() : this("")
        {

        }
        public UICInputNumber(string propertyName) : base(propertyName)
        {
        }

        #endregion


        #region Properties

        public bool AllowDecimalValues { get; set; } = true;

        public bool ValidationRequired { get; set; }
        public int? ValidationMinValue { get; set; }
        public int? ValidationMaxValue { get; set; }

        #endregion
    }
}
