using System.ComponentModel.DataAnnotations;
using Polliakov.ExtendedValidation.Core;

namespace Polliakov.ExtendedValidation.Attributes
{
    public class EntriesHaveRequiredAttribute : EntriesHaveValidationAttribute<RequiredAttribute>
    {
        public override bool RequiresValidationContext => false;
        protected override bool UseOneInstanceForEachEntry => true;
        protected override Func<RequiredAttribute> AttributeFactory => () => new RequiredAttribute();
    }
}
