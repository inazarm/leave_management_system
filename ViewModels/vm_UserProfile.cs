using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class vm_UserProfile
    {
        [Key]
        [Display(Name = "Login Name:")]
        public string PK_UserName { get; set; }

        [Display(Name = "OU:")]
        public string OULocation { get; set; }

        [Display(Name = "First Name:")]
        public string UserFirstName { get; set; }

        [Display(Name = "Full Name:")]
        public string UserFullName {
            get {
                return UserFirstName + " " + UserSurname;
            }
        }

        [Display(Name = "Surname:")]
        public string UserSurname { get; set; }

        [Display(Name = "Email:")]
        public string UserEmail { get; set; }

        [Display(Name = "Telephone Number:")]
        public string UserTelephone { get; set; }

        [Display(Name = "Mobile Number:")]
        public string UserMobileNumber { get; set; }

        [Display(Name = "Job Title:")]
        public string UserJobTitle { get; set; }

        [Display(Name = "Office:")]
        public string Office { get; set; }

        [Display(Name = "Department:")]
        public string Department { get; set; }

        [Display(Name = "Manager:")]
        public string Manager { get; set; }

        [Display(Name = "Company:")]
        public string Company { get; set; }

        [Display(Name = "Active User:")]
        public bool? IsEnabled { get; set; }
    }
}