using System.Linq;
using System.Web.Mvc;
using ViewModels;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using DAL.LeaveManagementSystem;
using DAL.ActiveDirectoryWarehouse;
using RazorEngine;
using RazorEngine.Templating;
using EmailUtilities;
using static ExtensionMethods.BooleanExtensions;

namespace Web.Controllers
{
    public partial class HomeController : Controller
    {
        public ActionResult Index()
        {
            ActiveDirectoryWarehouseContext db = new ActiveDirectoryWarehouseContext();
			
			// Get username by windows auth
            var Username = User.Identity.Name.Split('\\')[1];
            ViewBag.Username = db.UserProfiles.Where(w => w.PKUserName.Equals(Username)).FirstOrDefault().UserFullName;

            return View();
        }

        public ActionResult LMS_Read([DataSourceRequest]DataSourceRequest request)
        {
            var db = new LeaveManagementSystemContext();
            ActiveDirectoryWarehouseContext dbAD = new ActiveDirectoryWarehouseContext();

            var loggedInUser = User.Identity.Name.Split(separator: new char[] { '\\' })[1];

            var staffProfile = dbAD.UserProfiles
                .Where(w => w.PKUserName.Equals(loggedInUser))
                .Select(s => new vm_UserProfile()
                {
                    PK_UserName = s.PKUserName,
                    UserFirstName = s.UserFirstName,
                    UserSurname = s.UserSurname,
                    UserEmail = s.UserEmail,
                    UserTelephone = s.UserTelephone,
                    UserMobileNumber = s.UserMobileNumber,
                    UserJobTitle = s.UserJobTitle,
                    Office = s.Office,
                    Department = s.Department,
                    Manager = s.Manager,
                    Company = s.Company
                }).FirstOrDefault();

            var managerProfile = dbAD.UserProfiles
                              .Where(w => w.UserFullName.Equals(staffProfile.Manager))
                              .FirstOrDefault();

            dbAD.Dispose();

            //requests only visible by the user and his manager
            var model = db.UserManagements
                            .Where(w =>
                                w.AuthName.Equals(staffProfile.PK_UserName) ||
                                w.AuthName.Equals(managerProfile.PKUserName)
                            );

            DataSourceResult result = model.AsQueryable().ToDataSourceResult(request, ModelState, m => new vm_Grid()
            {
                PK_ID = m.PK_ID,
                AuthName = staffProfile.PK_UserName,
                StartDate = m.StartDate,
                EndDate = m.EndDate,
                Reason = m.Reason,
                IsApproved = m.IsApproved,
                Entitlement = m.Entitlement,
                IsEntitlementOvertaken = m.IsEntitlementOvertaken,
                BreakDuration = m.BreakDuration,
                RemainingDays = m.RemainingDays
            });

            db.Dispose();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LMS_Create([DataSourceRequest] DataSourceRequest request, vm_Grid viewModel)
        {
            var db = new LeaveManagementSystemContext();
            ActiveDirectoryWarehouseContext dbAD = new ActiveDirectoryWarehouseContext();

            var staffLoginID = User.Identity.Name.Split('\\')[1];

            var staffProfile = dbAD.UserProfiles
                .Where(w => w.PKUserName.Equals(staffLoginID))
                .FirstOrDefault();

            dbAD.Dispose();

            if (viewModel != null && ModelState.IsValid)
            {
                var model = new UserManagement();

                model.PK_ID = viewModel.PK_ID;
                model.AuthName = staffProfile.PKUserName;
                model.StartDate = viewModel.StartDate;
                model.EndDate = viewModel.EndDate;
                model.Reason = viewModel.Reason;
                model.IsApproved = viewModel.IsApproved;
                model.Entitlement = viewModel.Entitlement;
                model.BreakDuration = viewModel.BreakDuration;
                model.RemainingDays = viewModel.RemainingDays;
                model.IsEntitlementOvertaken = viewModel.IsEntitlementOvertaken;

                db.Add(model);
                db.SaveChanges();
                viewModel.PK_ID = model.PK_ID;
                SendMail(viewModel);
                db.Dispose();
            }
            return Json(new[] { viewModel }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult LMS_Update([DataSourceRequest] DataSourceRequest request, vm_Grid viewModel)
        {
            using (var db = new LeaveManagementSystemContext())
            {
                if (viewModel != null && ModelState.IsValid)
                {
                    ActiveDirectoryWarehouseContext dbAD = new ActiveDirectoryWarehouseContext();

                    var staffLoginID = User.Identity.Name.Split('\\')[1];

                    var staffProfile = dbAD.UserProfiles
                        .Where(w => w.PKUserName.Equals(staffLoginID))
                        .FirstOrDefault();
                    UserManagement model = db.UserManagements.Where(w => w.PK_ID.Equals(viewModel.PK_ID)).FirstOrDefault();

                    model.PK_ID = viewModel.PK_ID;
                    model.AuthName = staffProfile.PKUserName;
                    model.StartDate = viewModel.StartDate;
                    model.EndDate = viewModel.EndDate;
                    model.Reason = viewModel.Reason;
                    model.IsApproved = viewModel.IsApproved;
                    model.Entitlement = viewModel.Entitlement;
                    model.BreakDuration = viewModel.BreakDuration;
                    model.RemainingDays = viewModel.RemainingDays;
                    model.IsEntitlementOvertaken = viewModel.IsEntitlementOvertaken;

                    db.FlushChanges();
                    db.SaveChanges();
                    dbAD.Dispose();
                    viewModel.PK_ID = model.PK_ID;
                    SendMail(viewModel);
                }
                return Json(new[] { viewModel }.ToDataSourceResult(request, ModelState));
            }
        }

        [HttpPost]
        public ActionResult LMS_Delete([DataSourceRequest] DataSourceRequest request, vm_Grid viewModel)
        {

            if (viewModel != null && ModelState.IsValid)
            {
                using (var db = new LeaveManagementSystemContext())
                {
                    UserManagement model = db.UserManagements.Where(w => w.PK_ID.Equals(viewModel.PK_ID)).FirstOrDefault();
                    db.Delete(model);
                    db.SaveChanges();
                }
            }
            return Json(new[] { viewModel }.ToDataSourceResult(request, ModelState));
        }

        public void SendMail(vm_Grid viewModel)
        {
            LeaveManagementSystemContext dbLMS = new LeaveManagementSystemContext();
            ActiveDirectoryWarehouseContext dbAD = new ActiveDirectoryWarehouseContext();

            var loggedInUser = User.Identity.Name.Split(separator: new char[] { '\\' })[1];

            var staffProfile = dbAD.UserProfiles
                .Where(w => w.PKUserName.Equals(loggedInUser))
                .Select(s => new vm_UserProfile()
                {
                    PK_UserName = s.PKUserName,
                    UserFirstName = s.UserFirstName,
                    UserSurname = s.UserSurname,
                    UserEmail = s.UserEmail,
                    UserTelephone = s.UserTelephone,
                    UserMobileNumber = s.UserMobileNumber,
                    UserJobTitle = s.UserJobTitle,
                    Office = s.Office,
                    Department = s.Department,
                    Manager = s.Manager,
                    Company = s.Company
                }).FirstOrDefault();

            UserProfile managerProfile = dbAD.UserProfiles
                                          .Where(w => w.UserFullName.Equals(staffProfile.Manager))
                                          .FirstOrDefault();

            var request = dbLMS.UserManagements.Where(w => w.PK_ID.Equals(viewModel.PK_ID)).FirstOrDefault();

            dbAD.Dispose();
            dbLMS.Dispose();

            var approvedBool = request.IsApproved;
            var approvedConvert = boolConversion(approvedBool);
            var entitlementBool = request.IsEntitlementOvertaken;
            var entitlementConvert = boolConversion(entitlementBool);

            vm_Email emailTemplate = new vm_Email();
            emailTemplate.PK_ID = request.PK_ID;
            emailTemplate.Username = staffProfile.UserFullName;
            emailTemplate.UserEmail = staffProfile.UserEmail;
            emailTemplate.Manager = staffProfile.Manager;
            emailTemplate.ManagerEmail = managerProfile.UserEmail;
            emailTemplate.StartDate = request.StartDate;
            emailTemplate.EndDate = request.EndDate;
            emailTemplate.Reason = request.Reason;
            emailTemplate.Entitlement = request.Entitlement;
            emailTemplate.BreakDuration = request.BreakDuration;
            emailTemplate.RemainingDays = request.RemainingDays;
            emailTemplate.IsApproved = request.IsApproved;
            emailTemplate.Approved = approvedConvert;
            emailTemplate.IsEntitlementOvertaken = request.IsEntitlementOvertaken;
            emailTemplate.EntitlementOvertaken = entitlementConvert;

            var from = staffProfile.UserEmail;
            var to = managerProfile.UserEmail;

            var templatePath = System.IO.File.ReadAllText(Server.MapPath("~/Views/sendMail/sendMail.cshtml"));
            var template = Engine.Razor.RunCompile(templatePath, "key", typeof(vm_Email), emailTemplate);

            var templatePathConfirmation = System.IO.File.ReadAllText(Server.MapPath("~/Views/sendMail/sendMailConfirmation.cshtml"));
            var templateConfirmation = Engine.Razor.RunCompile(templatePathConfirmation, "confirmationkey", typeof(vm_Email), emailTemplate);

            //email to manager
            EmailHandler mail = new EmailHandler();
            mail.SendEmailNotification(from, to, "Leave Request", template);

            //email to the user for confirmation
            EmailHandler confirmationEmail = new EmailHandler();
            confirmationEmail.SendEmailNotification(from, from, "Leave request confirmation", templateConfirmation);
        }

        public partial class NotificationController : Controller
        {
            public ActionResult PopUpTemplate()
            {
                return View();
            }
        }
    }
}


