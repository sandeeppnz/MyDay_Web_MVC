using SANSurveyWebAPI.Models.Api;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System.Data.Entity;
using System.Web;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Owin.Security;
using SANSurveyWebAPI.Models;
using System.Data.Entity.Migrations;
using System;
using Microsoft.Owin.Security.Cookies;

namespace SANSurveyWebAPI
{

    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(
            IdentityFactoryOptions<ApplicationRoleManager> options,
            IOwinContext context)
        {
            return new ApplicationRoleManager(
                new ApplicationRoleStore(context.Get<ApplicationDbContext>()));
        }
    }

    //web based sign-in
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager) UserManager, DefaultAuthenticationTypes.ApplicationCookie);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    public class ApplicationUserManager : UserManager<ApplicationUser, string>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser, string> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(
                new UserStore<ApplicationUser, ApplicationRole, string,
                    ApplicationUserLogin, ApplicationUserRole,
                    ApplicationUserClaim>(context.Get<ApplicationDbContext>()));

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 3,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(
                        dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }


    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        public static void InitializeIdentityForEF(ApplicationDbContext db)
        {
            //var userManager = HttpContext.Current
            //        .GetOwinContext().GetUserManager<ApplicationUserManager>();

            //var roleManager = HttpContext.Current
            //    .GetOwinContext().Get<ApplicationRoleManager>();


            //const string userNameAdmin = "surveymyday@gmail.com";
            //const string passwordAdmin = "Admin@123";
            //const string roleNameAdmin = "Admin";
            //const string registeredRoleName = "Registered User";


            ////Create Role Admin if it does not exist
            //var roleAdminDB = roleManager.FindByName(roleNameAdmin);
            //if (roleAdminDB == null)
            //{
            //    roleAdminDB = new ApplicationRole(roleNameAdmin);
            //    var roleresult = roleManager.Create(roleAdminDB);
            //}


            //var roleUserDB = roleManager.FindByName(registeredRoleName);
            //if (roleUserDB == null)
            //{
            //    roleUserDB = new ApplicationRole(registeredRoleName);

            //    // Set the new custom property:
            //    var userRoleresult = roleManager.Create(roleUserDB);
            //}








            ////Admin Account creation
            //var userAdmin = userManager.FindByName(userNameAdmin);
            //if (userAdmin == null)
            //{
            //    userAdmin = new ApplicationUser { UserName = userNameAdmin, Email = userNameAdmin };
            //    var result = userManager.Create(userAdmin, passwordAdmin);
            //    result = userManager.SetLockoutEnabled(userAdmin.Id, false);


            //    Profile p = new Profile
            //    {
            //        Name = "System Admin",
            //        MobileNumber = "",
            //        LoginEmail = "surveymyday@gmail.com",
            //        CreatedDateTimeUtc = DateTime.UtcNow,
            //        CreatedDateTime = DateTime.Now,
            //        SpecialityId = null,
            //        UserId = userAdmin.Id,
            //        Status = Constants.StatusProfile.Active.ToString()

            //    };

            //    db.Profiles.Add(p);
            //    db.SaveChanges();
            //}


            //// Add user admin to Role Admin if not already added
            //var rolesForAdmin = userManager.GetRoles(userAdmin.Id);
            //if (!rolesForAdmin.Contains(roleAdminDB.Name))
            //{
            //    var result = userManager.AddToRole(userAdmin.Id, roleAdminDB.Name);
            //}





            //const string userName = "sandeep.perera@gmail.com";
            //const string password = "abc";



            //var user = userManager.FindByName(userName);
            //if (user == null)
            //{
            //    user = new ApplicationUser { UserName = userName, Email = userName };
            //    var result = userManager.Create(user, password);
            //    result = userManager.SetLockoutEnabled(user.Id, false);


            //    Profile p = new Profile
            //    {
            //        Name = "Gmail",
            //        MobileNumber = "7777",
            //        LoginEmail = "sandeep.perera@gmail.com",
            //        CreatedDateTimeUtc = DateTime.UtcNow,
            //        CreatedDateTime = DateTime.Now,
            //        OffSetFromUTC = 13,
            //        SpecialityId = null,
            //        UserId = user.Id,
            //        Status = Constants.StatusProfile.Active.ToString()

            //    };

            //    db.Profiles.Add(p);
            //    db.SaveChanges();
            //}

            //// Add user admin to Role Admin if not already added
            //var rolesForUser = userManager.GetRoles(user.Id);
            //if (!rolesForUser.Contains(roleUserDB.Name))
            //{
            //    var result = userManager.AddToRole(user.Id, roleUserDB.Name);
            //}




            //const string user2 = "sandeep.perera@live.com";
            //const string user2Password = "abc";

            //// Create Unregistered User:
            //var _user2 = userManager.FindByName(user2);
            //if (_user2 == null)
            //{
            //    _user2 = new ApplicationUser
            //    {
            //        UserName = user2,
            //        Email = user2
            //    };

            //    var result = userManager.Create(_user2, user2Password);
            //    result = userManager.SetLockoutEnabled(_user2.Id, false);


            //    //add a profile for this user
            //    Profile p = new Profile
            //    {
            //        Name = "Live",
            //        MobileNumber = "88888",
            //        LoginEmail = "sandeep.perera@live.com",
            //        CreatedDateTimeUtc = DateTime.UtcNow,
            //        CreatedDateTime = DateTime.Now,
            //        OffSetFromUTC = 0,
            //        SpecialityId = null,
            //        UserId = _user2.Id,
            //        Status = Constants.StatusProfile.Active.ToString()
            //    };

            //    db.Profiles.Add(p);
            //    db.SaveChanges();
            //}

            //// Add user to Role Users if not already added
            //var rolesForUser2 = userManager.GetRoles(_user2.Id);
            //if (!rolesForUser2.Contains(roleUserDB.Name))
            //{
            //    userManager.AddToRole(_user2.Id, roleUserDB.Name);
            //}

            //db.Specialitys.AddOrUpdate(x => x.Id,
            //     new Speciality() { Id = 1, Name = "Acute Medicine", Sequence = 1 },
            //     new Speciality() { Id = 2, Name = "Cardiology", Sequence = 2 },
            //     new Speciality() { Id = 3, Name = "Dematology", Sequence = 3 },
            //     new Speciality() { Id = 4, Name = "General Practice", Sequence = 4 },
            //     new Speciality() { Id = 5, Name = "General Surgery", Sequence = 5 },
            //     new Speciality() { Id = 6, Name = "Neurology", Sequence = 6 },
            //     new Speciality() { Id = 7, Name = "Orthopedics", Sequence = 7 },
            //     new Speciality() { Id = 8, Name = "Obstetrics and Gynaecology", Sequence = 8 },
            //     new Speciality() { Id = 9, Name = "Paediatrics", Sequence = 9 },
            //     new Speciality() { Id = 10, Name = "Psychiatry", Sequence = 10 },
            //     new Speciality() { Id = 11, Name = "Trauma and Orthopaedics", Sequence = 11 },
            //     new Speciality() { Id = 8, Name = "Undecided", Sequence = 12 },
            //     new Speciality() { Id = 9, Name = "Other", Sequence = 13 }
            //     );

            //db.SaveChanges();

            //            db.BirthYears.AddOrUpdate(x => x.Id,
            //new BirthYear() { Id = 1, Name = "1975", Sequence = 1 },
            //new BirthYear() { Id = 2, Name = "1976", Sequence = 2 },
            //new BirthYear() { Id = 3, Name = "1977", Sequence = 3 },
            //new BirthYear() { Id = 4, Name = "1978", Sequence = 4 },
            //new BirthYear() { Id = 5, Name = "1979", Sequence = 5 },
            //new BirthYear() { Id = 6, Name = "1980", Sequence = 6 },
            //new BirthYear() { Id = 7, Name = "1981", Sequence = 7 },
            //new BirthYear() { Id = 8, Name = "1982", Sequence = 8 },
            //new BirthYear() { Id = 9, Name = "1983", Sequence = 9 },
            //new BirthYear() { Id = 10, Name = "1984", Sequence = 10 },
            //new BirthYear() { Id = 11, Name = "1985", Sequence = 11 },
            //new BirthYear() { Id = 12, Name = "1986", Sequence = 12 }
            //);
            //            db.SaveChanges();

            //db.Ethinicitys.AddOrUpdate(x => x.Id,
            //    //new Ethinicity() { Id = 1, Name = "Asian", Sequence = 3 },
            //    //new Ethinicity() { Id = 3, Name = "English", Sequence = 1 },
            //    //new Ethinicity() { Id = 1, Name = "Asian", Sequence = 3 },
            //    new Ethinicity() { Id = 1, Name = "America", Sequence = 3 },
            //    new Ethinicity() { Id = 1, Name = "New Zealand", Sequence = 3 }
            //);

            // db.TaskItems.AddOrUpdate(x => x.Id,
            //    new TaskItem() { Id = 1, ShortName = "Discharge planning", LongName = "Preparation for moving a patient from one level of care to another within or outside the current health care agency", Status = "Active", CreatedDateTimeUtc = DateTime.UtcNow, Sequence = 1 },
            //    new TaskItem() { Id = 2, ShortName = "Writing prescriptions", LongName = "Generating and distributing prescriptions", Status = "Active", CreatedDateTimeUtc = DateTime.UtcNow, Sequence = 2 },
            //    new TaskItem() { Id = 3, ShortName = "Patient related discussion with colleagues", LongName = "e.g. clinical meetings, shift handover, ward rounds or consultations with other wards", Status = "Active", CreatedDateTimeUtc = DateTime.UtcNow, Sequence = 3 },
            //    new TaskItem() { Id = 4, ShortName = "Ordering / obtaining test results", LongName = "Ordering or obtaining results from laboratory or radiology tests", Status = "Active", CreatedDateTimeUtc = DateTime.UtcNow, Sequence = 4 },
            //    new TaskItem() { Id = 5, ShortName = "Preparing for surgery", LongName = "'Scrubbing up', putting on gown and gloves", Status = "Active", CreatedDateTimeUtc = DateTime.UtcNow, Sequence = 5 },
            //    new TaskItem() { Id = 6, ShortName = "Discussion with patient and/or relatives", LongName = "Discussion with patient and/or relatives", Status = "Active", CreatedDateTimeUtc = DateTime.UtcNow, Sequence = 6 },
            //    new TaskItem() { Id = 7, ShortName = "Initial history & physical examination", LongName = "Initial history & physical examination", Status = "Active", CreatedDateTimeUtc = DateTime.UtcNow, Sequence = 7 },
            //    new TaskItem() { Id = 8, ShortName = "Primary care team meetings", LongName = "Meeting with doctors, nurses and support staff", CreatedDateTimeUtc = DateTime.UtcNow, Status = "Active", Sequence = 8 }



            //    //new TaskItem() { Id = 9, ShortName = "Task9", LongName = "Undertaking managerial responsibilities such as planning the workload and staffing of the department, especially at more senior levels", Status = "Active", CreatedDate = DateTime.Now, Sequence = 10 },
            //    //new TaskItem() { Id = 10, ShortName = "Task10", LongName = "Teaching and supervising junior doctors and medical students", Status = "Active", CreatedDate = DateTime.Now, Sequence = 8 },

            //    //new TaskItem() { Id = 10, ShortName = "Task11", LongName = "Carrying out auditing and research", CreatedDate = DateTime.Now, Status = "Active", Sequence = 9 },
            //    //new TaskItem() { Id = 1, ShortName = "Task12", LongName = "Seconday Providing general care to patients", Status = "Active", CreatedDate = DateTime.Now, Sequence = 3 },
            //    //new TaskItem() { Id = 2, ShortName = "Task13", LongName = "Seconday Investigations and treatment", Status = "Active", CreatedDate = DateTime.Now, Sequence = 2 },
            //    //new TaskItem() { Id = 3, ShortName = "Task14", LongName = "Seconday Talking to patients to diagnose their medical condition", Status = "Active", CreatedDate = DateTime.Now, Sequence = 1 },
            //    //new TaskItem() { Id = 4, ShortName = "Task14", LongName = "Seconday Performing operations and specialist investigations", Status = "Active", CreatedDate = DateTime.Now, Sequence = 4 },
            //    //new TaskItem() { Id = 5, ShortName = "Task15", LongName = "Seconday Making notes and preparing paperwork, both as a legal record of treatment and for the benefit of other healthcare professionals", Status = "Active", CreatedDate = DateTime.Now, Sequence = 11 },
            //    //new TaskItem() { Id = 6, ShortName = "Task16", LongName = "Seconday Working with other doctors as part of a team, either in the same department or within other specialties", Status = "Active", CreatedDate = DateTime.Now, Sequence = 5 },
            //    //new TaskItem() { Id = 7, ShortName = "Task17", LongName = "Seconday Liaising with other medical and non-medical staff in the hospital to ensure quality treatment", Status = "Active", CreatedDate = DateTime.Now, Sequence = 6 },
            //    //new TaskItem() { Id = 8, ShortName = "Task18", LongName = "Seconday Promoting health education", CreatedDate = DateTime.Now, Status = "Active", Sequence = 7 },
            //    //new TaskItem() { Id = 9, ShortName = "Task19", LongName = "Seconday Undertaking managerial responsibilities such as planning the workload and staffing of the department, especially at more senior levels", Status = "Active", CreatedDate = DateTime.Now, Sequence = 10 },
            //    //new TaskItem() { Id = 9, ShortName = "Task20", LongName = "Seconday Teaching and supervising junior doctors and medical students", Status = "Active", CreatedDate = DateTime.Now, Sequence = 8 },

            //    //new TaskItem() { Id = 10, ShortName = "Task21", LongName = "Primary Carrying out auditing and research", CreatedDate = DateTime.Now, Status = "Active", Sequence = 9 },
            //    //new TaskItem() { Id = 1, ShortName = "Task22", LongName = "Primary Monitoring and providing general care to patients on hospital wards and in outpatient clinics", Status = "Active", CreatedDate = DateTime.Now, Sequence = 3 },
            //    //new TaskItem() { Id = 2, ShortName = "Task23", LongName = "Primary Admitting patients requiring special care, followed by investigations and treatment", Status = "Active", CreatedDate = DateTime.Now, Sequence = 2 },
            //    //new TaskItem() { Id = 3, ShortName = "Task24", LongName = "Primary Examining and talking to patients to diagnose their medical condition", Status = "Active", CreatedDate = DateTime.Now, Sequence = 1 },
            //    //new TaskItem() { Id = 4, ShortName = "Task25", LongName = "Primary Carrying out specific procedures, e.g. performing operations and specialist investigations", Status = "Active", CreatedDate = DateTime.Now, Sequence = 4 },
            //    //new TaskItem() { Id = 5, ShortName = "Task26", LongName = "Primary Making notes and preparing paperwork, both as a legal record of treatment and for the benefit of other healthcare professionals", Status = "Active", CreatedDate = DateTime.Now, Sequence = 11 },
            //    //new TaskItem() { Id = 6, ShortName = "Task27", LongName = "Primary Working with other doctors as part of a team, either in the same department or within other specialties", Status = "Active", CreatedDate = DateTime.Now, Sequence = 5 },
            //    //new TaskItem() { Id = 7, ShortName = "Task28", LongName = "Primary Liaising with other medical and non-medical staff in the hospital to ensure quality treatment", Status = "Active", CreatedDate = DateTime.Now, Sequence = 6 },
            //    //new TaskItem() { Id = 8, ShortName = "Task29", LongName = "Primary Promoting health education", CreatedDate = DateTime.Now, Status = "Active", Sequence = 7 },
            //    //new TaskItem() { Id = 9, ShortName = "Task30", LongName = "Primary Undertaking managerial responsibilities such as planning the workload and staffing of the department, especially at more senior levels", Status = "Active", CreatedDate = DateTime.Now, Sequence = 10 },

            //    //new TaskItem() { Id = 9, ShortName = "Task31", LongName = "Primary Teaching and supervising junior doctors and medical students", Status = "Active", CreatedDate = DateTime.Now, Sequence = 8 },
            //    //new TaskItem() { Id = 10, ShortName = "Task32", LongName = "Primary carrying out auditing and research", CreatedDate = DateTime.Now, Status = "Active", Sequence = 9 }
            //);


        }
    }
}
