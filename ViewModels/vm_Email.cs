using System;
using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class vm_Email
    {
        [Key]
        [Required]
        public int PK_ID { get; set; }

        [Display(Name = "Name")]
        [Required]
        public string Username { get; set; }

        [Display(Name = "User email")]
        [Required]
        public string UserEmail { get; set; }

        [Display(Name = "Manager")]
        [Required]
        public string Manager { get; set; }

        [Display(Name = "Manager email")]
        [Required]
        public string ManagerEmail { get; set; }

        [Display(Name = "Start date"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime StartDate { get; set; }

        [Display(Name = "End date"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required]
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

        [Display(Name = "Allowance entitlement")]
        [Required]
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
                return (int) (this.EndDate -  this.StartDate).TotalDays;
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
                return this.remainingDays;
            }
            set
            {
                value = this.Entitlement - this.BreakDuration;
                if (value < 0)
                    this.remainingDays = 0;
                else this.remainingDays = value;
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
    }
}
