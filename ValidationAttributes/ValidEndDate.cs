using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CustomValidationAttribute.ValidationAttributeExtensions
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class GreaterDateAttribute : ValidationAttribute, IClientValidatable
    {
        public string EarlierDateField { get; set; }
        protected override ValidationResult IsValid(object value,  ValidationContext validationContext)
        {
            DateTime? date = value != null ? (DateTime?) value : null;
            var earlierDateValue = validationContext.ObjectType.GetProperty(EarlierDateField)
                .GetValue(validationContext.ObjectInstance, null);
            DateTime? earlierDate = earlierDateValue != null ? (DateTime?) earlierDateValue : null;

            if (date.HasValue && earlierDate.HasValue && date <= earlierDate)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "greaterdate"
            };
            rule.ValidationParameters["earlierdate"] = EarlierDateField;

            yield return rule;
        }
    }
}
