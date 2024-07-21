using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Polliakov.ExtendedValidation.Core
{
    /// <summary>
    /// Base class for attribute based validation of IEnumerable and IDictionary entries.
    /// </summary>
    /// <remarks>
    /// Attributes don't truly applies to each entry, just uses for their validation functionality.
    /// Can't be used for applying attribute to chars in string.
    /// </remarks>
    /// <typeparam name="TAttribute">Type of validation attribute, that used for validating entries.</typeparam>
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, 
        AllowMultiple = true
    )]
    public abstract class EntriesHaveValidationAttribute<TAttribute> : ValidationAttribute
        where TAttribute : ValidationAttribute
    {
        /// <summary>
        /// Can be overridden to false for optimization if attribute no need context.
        /// </summary>
        public override bool RequiresValidationContext => true;

        /// <summary>
        /// IDictionary KeyValuePair selector.
        /// </summary>
        public EntriesType EntriesType { get; set; } = EntriesType.Values;

        /// <summary>
        /// Is need to apply attribute to nested enumerables recursively. By default false.
        /// </summary>
        /// <remarks>
        /// Not obvious behavior: applies attribute to all null collections.
        /// </remarks>
        public bool IsRecursive { get; init; } = false;

        /// <summary>
        /// For recursive apply is number of layer where need to stop, -1 by default for max available depth.
        /// Nested enumerables starts from depth 1, 0 is the actual enumerable.
        /// </summary>
        public int MaxApplyDepth { get; init; } = -1;

        /// <summary>
        /// Factory method for initializing attribute.
        /// </summary>
        protected abstract Func<TAttribute> AttributeFactory { get; }

        /// <summary>
        /// If true use one instance of attribute for each entry, else creating different instances. 
        /// By default false.
        /// </summary>
        /// <remarks>
        /// Should be true for optimization reasons if attribute don't has state and 
        /// (don't require validation context or 
        /// it's dependencies don't have state and separated validation contexts is no needed).
        /// </remarks>
        protected virtual bool UseOneInstanceForEachEntry => false;

        private readonly TAttribute _commonAttribute;

        public EntriesHaveValidationAttribute() : this(null) { }

        public EntriesHaveValidationAttribute(string errorMessage) : base(errorMessage)
        {
            _commonAttribute = UseOneInstanceForEachEntry ? AttributeFactory() : null;
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage ?? $"{name} has invalid values by reason {GetAttributeName()}";
        }

        protected override sealed ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var entries = IsRecursive ? GetEntriesRecursive(value) : GetEntries(value);
            if (entries is null)
                return ValidationResult.Success;

            var isValid = true;
            foreach (var entry in entries)
            {
                if (!IsEntryValid(entry, validationContext))
                {
                    isValid = false;
                    break;
                }
            }

            if (isValid)
                return ValidationResult.Success;

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        protected static string GetAttributeName()
        {
            var attributeName = typeof(TAttribute).Name;

            if (attributeName.EndsWith("Attribute"))
                return attributeName[0..^9];
            else
                return attributeName;
        }

        private IEnumerable GetEntries(object value)
        {
            if (value is IDictionary dictionary)
            {
                return EntriesType switch
                {
                    EntriesType.Keys => dictionary.Keys,
                    EntriesType.Values or _ => dictionary.Values
                };
            }
            else if (
                EntriesType == EntriesType.Values && 
                value is IEnumerable entries &&
                value is not string)
            {
                return entries;
            }
            else
            {
                return null;
            }
        }

        private IEnumerable GetEntriesRecursive(object value)
        {
            var entries = new List<object>();
            AddEntriesRecursive(value, ref entries);
            return entries.Any() ? entries : null;
        }

        private (object value, bool isValue) AddEntriesRecursive(
            object value, 
            ref List<object> entriesResult,
            int layer = 0)
        {
            var entries = GetEntries(value);
            if ((MaxApplyDepth != -1 && MaxApplyDepth == layer) || entries is null)
                return (value, true);

            foreach (var entry in entries)
            {
                var result = AddEntriesRecursive(entry, ref entriesResult);
                if (result.isValue)
                    entriesResult.Add(result.value);
            }
            return (null, false);
        }

        private bool IsEntryValid(object entry, ValidationContext commonContext)
        {
            var attribute = _commonAttribute ?? AttributeFactory();

            if (attribute.RequiresValidationContext)
            {
                var validationContext = new ValidationContext(entry, commonContext, commonContext.Items);
                var result = attribute.GetValidationResult(entry, validationContext);
                return result.ErrorMessage is null;
            }
            else
            {
                return attribute.IsValid(entry);
            }
        }
    }
}
