using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace VenkataAllocationManagementSystem.CustomClass
{
    /// <summary>
    /// Custom validation attribute to ensure that the end date of an allocation is not later than the project's end date.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AllocationEndDateValidationAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public AllocationEndDateValidationAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            // System.Diagnostics.EventLog.WriteEntry("Application", $"Validating {validationContext.DisplayName} with value: {value}");   
            if (value == null || !DateOnly.TryParse(value.ToString(), out DateOnly dateToValidate))
            {
                // If the value is null or not a valid date, let other validators handle it (e.g., RequiredAttribute)
                return ValidationResult.Success!;
            }

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (property == null)
            {
                throw new ArgumentException("Property with this name not found.");
            }

            var comparisonValue = (DateOnly?)property.GetValue(validationContext.ObjectInstance);
            if (comparisonValue == null || !DateOnly.TryParse(comparisonValue.ToString(), out DateOnly comparisonDate))
            {
                // If the comparison property is null or not a valid date, let other validators handle it
                return ValidationResult.Success!;
            }

            // System.Diagnostics.EventLog.WriteEntry("Application", $"Validating {validationContext.DisplayName}: {dateToValidate}");
            // System.Diagnostics.EventLog.WriteEntry("Application", $"Validating Comparison : {comparisonDate}");
            // System.Diagnostics.EventLog.WriteEntry("Application", $"CompareTo: {dateToValidate.CompareTo(comparisonDate)}");

            // Implement your comparison logic (e.g., dateToValidate > comparisonDate)
            if (dateToValidate.CompareTo(comparisonDate) > 0)
            {
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} must be after {_comparisonProperty}.");;
            }
            return ValidationResult.Success!;
        }
    }    
}