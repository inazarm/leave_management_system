using System;
using System.ComponentModel.DataAnnotations;
using CustomValidationAttribute.ValidationAttributeExtensions;

namespace ViewModels
{
    public class vm_Grid
    {
        [Key]
        [Required]
        public int PK_ID { get; set; }
        public string AuthName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start date"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End date"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [GreaterDate(EarlierDateField = "StartDate", ErrorMessage = "End date should be after start date")]
        public DateTime EndDate { get; set; }

        private string reason;
        public string Reason
        {
            get
            {
                return this.reason;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    this.reason = "Not explicited";
                else this.reason = value;
            }
        }

        [Required]
        public bool IsApproved { get; set; }

        public string Approved
        {
            get
            {
                return this.IsApproved.Equals(true) ? "yes" : "no";
            }
            set
            {  
            }
        }

        [Required]
        [Display(Name = "Entitlement days")]
        public int? Entitlement { get; set; }

        public bool IsEntitlementOvertaken
        {
            get
            {
                return this.Entitlement - this.BreakDuration < 0 ? true : false;
            }
            set
            {
            }
        }

        public string EntitlementOvertaken
        {
            get
            {
                return this.IsEntitlementOvertaken.Equals(true) ? "yes" : "no";
            }

            set
            {
            }
        }

        [Display(Name = "Break Duration")]
        public int? BreakDuration
        {
            get
            {
                return (int?) (this.EndDate - this.StartDate).TotalDays;
            }

            set
            {
            }
        }

        private int? remainingDays;
        public int? RemainingDays
        {
            get
            {
                return this.Entitlement - this.BreakDuration;
            }
            set
            {
            }
        }
    }
}
