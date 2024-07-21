using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Polliakov.ExtendedValidation.Attributes
{
    /// <summary>
    /// Extended required attribute. Dissalow empty enumerables.
    /// </summary>
    public class NotEmptyAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is null)
                return false;

            if (value is string stringValue)
                return !string.IsNullOrWhiteSpace(stringValue);

            if (value is IEnumerable enumerableValue)
                return enumerableValue.GetEnumerator().MoveNext();

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage ?? $"The field {name} is empty.";
        }
    }
}
