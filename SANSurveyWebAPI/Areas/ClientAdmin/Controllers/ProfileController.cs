using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.IO;
using Twilio;
using SANSurveyWebAPI.Models.Api;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.BLL;
using Kendo.Mvc.Extensions;
using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.Controllers;

namespace SANSurveyWebAPI.Areas.ClientAdmin.Controllers
{
    [Authorize(Roles = "ClientAdmin")]
    public class ProfileController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProfileAdminService profileAdminSvc;
        private ProfileService profileSvc;
        private HangfireScheduler schedulerSvc;

        private AdminService adminService;
        
        public ProfileController()
        {
            this.profileAdminSvc = new ProfileAdminService();
            this.profileSvc = new ProfileService();
            this.schedulerSvc = new HangfireScheduler();
            this.adminService = new AdminService();


        }

        protected override void Dispose(bool disposing)
        {
            profileAdminSvc.Dispose();
            profileSvc.Dispose();
            schedulerSvc.Dispose();


            adminService.Dispose();

            base.Dispose(disposing);
        }



        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                var userId = db.Users
                .Where(x => x.Email == Constants.AdminEmail.ToString())
                .SingleOrDefault().Id;

                // Verify that the user selected a file
                if (file != null && file.ContentLength > 0)
                {
                    // extract only the filename
                    var fileName = Path.GetFileName(file.FileName);
                    // store the file inside ~/App_Data/uploads folder
                    //var path = Path.Combine(Server.MapPath("~/App_Data/Uploads"), fileName);
                    var path = Path.Combine(Server.MapPath("~/Views/Emails"), fileName);
                    file.SaveAs(path);

                    //BEGINING OF IMPORT
                    FileInfo eFile = new FileInfo(path);
                    using (var excelPackage = new OfficeOpenXml.ExcelPackage(eFile))
                    {
                        if (!eFile.Name.EndsWith("xlsx"))//Return ModelState.AddModelError()
                        {
                            ModelState.AddModelError("", "Incompartible Excel Document. Please use MSExcel 2007 and Above!");
                        }
                        else
                        {
                            var worksheet = excelPackage.Workbook.Worksheets[1];
                            if (worksheet == null) { ModelState.AddModelError("", "Wrong Excel Format!"); }// return ImportResults.WrongFormat;

                            else
                            {
                                var lastRow = worksheet.Dimension.End.Row;
                                while (lastRow >= 1)
                                {
                                    //var range = worksheet.Cells[lastRow, 1, lastRow, 3];
                                    var range = worksheet.Cells[lastRow, 1, lastRow, 5];
                                    if (range.Any(c => c.Value != null))
                                    { break; }
                                    lastRow--;
                                }
                                //using (var db = new BlackBox_FinaleEntities())// var db = new BlackBox_FinaleEntities())
                                //{
                                for (var row = 2; row <= lastRow; row++)
                                {
                                    string ss = worksheet.Cells[row, 3].Value as string;

                                    Profile s = new Models.Profile();
                                    s.LoginEmail = StringCipher.EncryptRfc2898(worksheet.Cells[row, 1].Value.ToString());
                                    s.Name = worksheet.Cells[row, 2].Value.ToString();
                                    s.MobileNumber = ss;
                                    s.OffSetFromUTC = int.Parse(worksheet.Cells[row, 4].Value.ToString());
                                    s.RegistrationProgressNext = Constants.StatusRegistrationProgress.New.ToString();
                                    s.CreatedDateTimeUtc = DateTime.UtcNow;
                                    s.UserId = userId;
                                    s.Incentive = int.Parse(worksheet.Cells[row, 5].Value.ToString());

                                    db.Profiles.Add(s);

                                    try
                                    {
                                        db.SaveChanges();
                                    }

                                    catch (Exception ex)
                                    {

                                    }
                                }
                                //}
                            }

                        }

                    }//END OF IMPORT
                }

            }
            catch (Exception ex)
            { }
            return RedirectToAction("Index");
        }
        //public ActionResult Upload(HttpPostedFileBase file)
        //{
        //    var userId = db.Users.Where(x => x.Email == Constants.AdminEmail.ToString()).SingleOrDefault().Id;

        //    // Verify that the user selected a file
        //    if (file != null && file.ContentLength > 0)
        //    {
        //        // extract only the filename
        //        var fileName = Path.GetFileName(file.FileName);
        //        // store the file inside ~/App_Data/uploads folder
        //        var path = Path.Combine(Server.MapPath("~/App_Data/Uploads"), fileName);
        //        file.SaveAs(path);

        //        //BEGINING OF IMPORT
        //        FileInfo eFile = new FileInfo(path);
        //        using (var excelPackage = new OfficeOpenXml.ExcelPackage(eFile))
        //        {
        //            if (!eFile.Name.EndsWith("xlsx"))//Return ModelState.AddModelError()
        //            { ModelState.AddModelError("", "Incompartible Excel Document. Please use MSExcel 2007 and Above!"); }
        //            else
        //            {
        //                var worksheet = excelPackage.Workbook.Worksheets[1];
        //                if (worksheet == null) { ModelState.AddModelError("", "Wrong Excel Format!"); }// return ImportResults.WrongFormat;

        //                else
        //                {
        //                    var lastRow = worksheet.Dimension.End.Row;
        //                    while (lastRow >= 1)
        //                    {
        //                        var range = worksheet.Cells[lastRow, 1, lastRow, 3];
        //                        if (range.Any(c => c.Value != null))
        //                        { break; }
        //                        lastRow--;
        //                    }
        //                    //using (var db = new BlackBox_FinaleEntities())// var db = new BlackBox_FinaleEntities())
        //                    //{
        //                    for (var row = 2; row <= lastRow; row++)
        //                    {
        //                        var s = new Profile
        //                        {
        //                            LoginEmail = worksheet.Cells[row, 1].Value.ToString(),
        //                            Name = worksheet.Cells[row, 2].Value.ToString(),
        //                            MobileNumber = worksheet.Cells[row, 3].Value.ToString(),
        //                            OffSetFromUTC = int.Parse(worksheet.Cells[row, 4].Value.ToString()),
        //                            RegistrationProgressNext = Constants.StatusRegistrationProgress.New.ToString(),
        //                            CreatedDateTimeUtc = DateTime.UtcNow,
        //                            //Status = Constants.StatusProfile.Active.ToString(),
        //                            UserId = userId
        //                            //idSerial = worksheet.Cells[row, 3].Value.ToString(),
        //                            //fullName = worksheet.Cells[row, 4].Value.ToString(),
        //                            //dob = DateTime.Parse(worksheet.Cells[row, 5].Value.ToString()),
        //                            //gender = worksheet.Cells[row, 6].Value.ToString()
        //                        };
        //                        db.Profiles.Add(s);
        //                        try
        //                        {
        //                            db.SaveChanges();
        //                        }

        //                        catch (Exception ex)
        //                        {

        //                        }
        //                    }
        //                    //}
        //                }

        //            }

        //        }//END OF IMPORT
        //    }

        //    return RedirectToAction("Index");
        //}


        //public ActionResult Index()
        //{
        //    return View();
        //}


        //public ActionResult Profiles_Read([DataSourceRequest] DataSourceRequest request)
        //{
        //    return Json(svc.Read().ToDataSourceResult(request));
        //}

        public async Task<ActionResult> Index()
        {
            //var profiles = db.Profiles
            //    .Include(p => p.Speciality)
            //    .Include(p => p.User);
            ////foreach (Profile p in profiles)
            ////{
            ////    string name = p.Name;
            ////    p.Name = name;
            ////}

            //return View(await profiles.ToListAsync());

            var profiles = adminService.GetAllProfiles();

            return View(profiles);

        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = await db.Profiles.FindAsync(id);
            profile.LoginEmail = StringCipher.DecryptRfc2898(profile.LoginEmail.Trim());
            if (profile == null)
            {
                return HttpNotFound();
            }

            //string name = profile.Name;
            //profile.Name = name;

            return View(profile);
        }

        public ActionResult Create()
        {
            ViewBag.SpecialityId = new SelectList(db.Specialitys, "Id", "Name");
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email");
            return View();
        }

        
        // POST: Profiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "Name,MobileNumber,LoginEmail,OffSetFromUTC,Incentive")] Profile profile)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        //string name = profile.Name;
        //        //profile.Name = name;
        //        //profile.Status = Constants.StatusProfile.Active.ToString();
        //        profile.CreatedDateTimeUtc = DateTime.UtcNow;
        //        profile.RegistrationProgressNext = Constants.StatusRegistrationProgress.New.ToString();
        //        profile.UserId = db.Users.Where(x => x.Email == Constants.AdminEmail.ToString()).SingleOrDefault().Id;
        //        db.Profiles.Add(profile);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }

        //    //ViewBag.SpecialityId = new SelectList(db.Specialitys, "Id", "Name", profile.SpecialityId);
        //    ViewBag.UserId = new SelectList(db.Users, "Id", "Email", profile.UserId);
        //    return View(profile);
        //}

        public async Task<ActionResult> Create([Bind(Include = "Name,MobileNumber,LoginEmail,OffSetFromUTC,Incentive")] Profile profile)
        {
            if (ModelState.IsValid)
            {

                //Encryption
                profile.LoginEmail = StringCipher.EncryptRfc2898(profile.LoginEmail.Trim());

                profile.CreatedDateTimeUtc = DateTime.UtcNow;
                profile.RegistrationProgressNext = Constants.StatusRegistrationProgress.New.ToString();
                profile.UserId = db.Users.Where(x => x.Email == Constants.AdminEmail.ToString()).SingleOrDefault().Id;
                //profile.Incentive = int.Parse(profile.Incentive);
                db.Profiles.Add(profile);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            //ViewBag.SpecialityId = new SelectList(db.Specialitys, "Id", "Name", profile.SpecialityId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", profile.UserId);
            return View(profile);
        }

        // GET: Profiles/Edit/5
        //public async Task<ActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Profile profile = await db.Profiles.FindAsync(id);
        //    if (profile == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    //string name = profile.Name;
        //    //profile.Name = name;

        //    //ViewBag.SpecialityId = new SelectList(db.Specialitys, "Id", "Name", profile.SpecialityId);
        //    ViewBag.UserId = new SelectList(db.Users, "Id", "Email", profile.UserId);
        //    return View(profile);
        //}

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = await db.Profiles.FindAsync(id);

            //Edited for encryption on edit page --done by Bharati Koli on 25th Sep 2017
            //Encryption
            //profile.LoginEmail = StringCipher.DecryptRfc2898(profile.LoginEmail.Trim());

            profile.LoginEmail = Constants.GetEmailMaskedText(StringCipher.DecryptRfc2898(profile.LoginEmail.Trim()));

            //End of encryption changes

            if (profile == null)
            {
                return HttpNotFound();
            }

            //string name = profile.Name;
            //profile.Name = name;

            //ViewBag.SpecialityId = new SelectList(db.Specialitys, "Id", "Name", profile.SpecialityId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", profile.UserId);
            return View(profile);
        }

        // POST: Profiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,MobileNumber,LoginEmail,UserId,RegistrationProgressNext,OffSetFromUTC,Incentive")] Profile profile)
        {
            if (ModelState.IsValid)
            {
                //string name = profile.Name;
                //profile.Name = name;
                //profile.LastUpdatedDate = DateTime.Now;
                var p = db.Profiles.Where(x => x.Id == profile.Id).SingleOrDefault();
                p.Name = profile.Name;
                p.MobileNumber = profile.MobileNumber;
                p.Incentive = profile.Incentive;
                //p.LoginEmail = profile.LoginEmail;

                //Editing for email not allowed -- done by Bharati Koli 25h Sep 2017
                //Encryption
                //p.LoginEmail = StringCipher.EncryptRfc2898(profile.LoginEmail.Trim());
                //End of code

                p.OffSetFromUTC = int.Parse(profile.OffSetFromUTC.ToString());
                p.UserId = profile.UserId;
                //p.RegistrationProgressNext = profile.RegistrationProgressNext;
                db.Entry(p).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            //ViewBag.SpecialityId = new SelectList(db.Specialitys, "Id", "Name", profile.SpecialityId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", profile.UserId);
            return View(profile);
        }
        //public async Task<ActionResult> Edit([Bind(Include = "Id,Name,MobileNumber,LoginEmail,UserId,RegistrationProgressNext,OffSetFromUTC")] Profile profile)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //string name = profile.Name;
        //        //profile.Name = name;
        //        //profile.LastUpdatedDate = DateTime.Now;
        //        var p = db.Profiles.Where(x => x.Id == profile.Id).SingleOrDefault();

        //        p.Name = profile.Name;
        //        p.MobileNumber = profile.MobileNumber;
        //        p.LoginEmail = profile.LoginEmail;
        //        p.OffSetFromUTC = int.Parse(profile.OffSetFromUTC.ToString());
        //        p.UserId = profile.UserId;
        //        p.RegistrationProgressNext = profile.RegistrationProgressNext;

        //        db.Entry(p).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    //ViewBag.SpecialityId = new SelectList(db.Specialitys, "Id", "Name", profile.SpecialityId);
        //    ViewBag.UserId = new SelectList(db.Users, "Id", "Email", profile.UserId);
        //    return View(profile);
        //}

        // GET: Profiles/Delete/5
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Profile profile = await db.Profiles.FindAsync(id);
        //    if (profile == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    //string name = profile.Name;
        //    //profile.Name = name;

        //    return View(profile);
        //}
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = await db.Profiles.FindAsync(id);

            //Masking email Id -- done by Bharati Koli on 25th Sep 2017
            //profile.LoginEmail = StringCipher.DecryptRfc2898(profile.LoginEmail.Trim());
            profile.LoginEmail = Constants.GetEmailMaskedText(StringCipher.DecryptRfc2898(profile.LoginEmail.Trim()));
            //end of code

            if (profile == null)
            {
                return HttpNotFound();
            }

            return View(profile);
        }

        // POST: Profiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Profile profile = await db.Profiles.FindAsync(id);
            db.Profiles.Remove(profile);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #region Hangfire
        public async Task<ActionResult> MassEmail()
        {
            var emailIds = db.Profiles
                .Where(x => x.RegistrationProgressNext == Constants.StatusRegistrationProgress.New.ToString())
                .Select(x => new { x.Id, x.LoginEmail, x.Name })
                .ToList();

            foreach (var k in emailIds)
            {
                Guid uid = Guid.NewGuid();
                RegistrationInvitationEmailDto e = new RegistrationInvitationEmailDto();
                e.ToEmail = StringCipher.DecryptRfc2898(k.LoginEmail);
                e.RecipientName = k.Name;
                e.Link = GetBaseURL() + "Register/Index" + "/" + uid;

                int? jobId = await schedulerSvc.RegisterBaselineSurveyEmail(e, 2.0);

                if (jobId != 0)
                {
                    await profileSvc.ResetProfileAsync(k.Id, uid, emailJobId: jobId, smsJobId: null);
                }
                else
                {
                    throw new Exception("Profile Id:" + k.Id + ", Hangfire jobid is zero error!");
                }
            }
            return RedirectToAction("Index");
        }
        #endregion
        //#region Hangfire
        //public async Task<ActionResult> MassEmail()
        //{

        //    var emailIds = db.Profiles
        //        .Where(x => x.RegistrationProgressNext == Constants.StatusRegistrationProgress.New.ToString())
        //        .Select(x => new { x.Id, x.LoginEmail, x.Name })
        //        .ToList();

        //    foreach (var k in emailIds)
        //    {
        //        Guid uid = Guid.NewGuid();

        //        RegistrationInvitationEmailDto e = new RegistrationInvitationEmailDto();
        //        e.ToEmail = StringCipher.DecryptRfc2898(k.LoginEmail);
        //        e.RecipientName = k.Name;
        //        e.Link = GetBaseURL() + "Register/Index" + "/" + uid;

        //        int? jobId = await schedulerSvc.RegisterBaselineSurveyEmail(e, 2.0);

        //        if (jobId != 0)
        //        {
        //            await profileSvc.ResetProfileAsync(k.Id, uid, emailJobId: jobId, smsJobId: null);
        //        }
        //        else
        //        {
        //            throw new Exception("Profile Id:" + k.Id + ", Hangfire jobid is zero error!");
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}
        //#endregion

        #region Single Email No Hangfire
        public async Task<ActionResult> InviteEmail(int? id)
        {
            if (!id.HasValue)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var k = db.Profiles
                .Where(p => p.Id == id.Value)
                .SingleOrDefault();

            if (k != null)
            {
                Guid uid = Guid.NewGuid();

                RegistrationInvitationEmailDto e = new RegistrationInvitationEmailDto();
                e.ToEmail = StringCipher.DecryptRfc2898(k.LoginEmail);
                e.RecipientName = k.Name;
                e.Link = GetBaseURL() + "Register/Index" + "/" + uid;
                e.Incentive = k.Incentive;
                await profileSvc.ResetProfileAsync(k.Id, uid, emailJobId: null, smsJobId: null);

                //No Hangfire
                await Task.Run(() =>
                {
                    PostalEmail.RegisterAndBaseline(e);
                });
            }
            else
            {
                return HttpNotFound();
            }


            return RedirectToAction("Index");
        }
        #endregion

        //#region Single Email No Hangfire
        //public async Task<ActionResult> InviteEmail(int? id)
        //{


        //    if (!id.HasValue)
        //    {
        //        return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    var k = db.Profiles
        //        .Where(p => p.Id == id.Value)
        //        .SingleOrDefault();

        //    if (k != null)
        //    {
        //        Guid uid = Guid.NewGuid();

        //        RegistrationInvitationEmailDto e = new RegistrationInvitationEmailDto();
        //        e.ToEmail = StringCipher.DecryptRfc2898(k.LoginEmail);
        //        e.RecipientName = k.Name;
        //        e.Link = GetBaseURL() + "Register/Index" + "/" + uid;

        //        await profileSvc.ResetProfileAsync(k.Id, uid, emailJobId: null, smsJobId: null);

        //        //No Hangfire
        //        await Task.Run(() =>
        //        {
        //            PostalEmail.RegisterAndBaseline(e);
        //        });
        //    }
        //    else
        //    {
        //        return HttpNotFound();
        //    }


        //    return RedirectToAction("Index");
        //}
        //#endregion
        ////[AutomaticRetry(Attempts = 20)]
        //public async static void NotifySignupInvitee(Profile profile, string body, string GetBaseURL())
        //{
        //    //var heading = GetBaseURL() + "Images/logo-mini.png";
        //    //var heading = "Text Header";
        //    //string heading = string.Format("<img src='{0}Images/logo.png' height='42' width='42'></img>", GetBaseURL());

        //    // Prepare Postal classes to work outside of ASP.NET request
        //    var viewsPath = Path.GetFullPath(HostingEnvironment.MapPath(@"~/Views/Emails"));
        //    var engines = new ViewEngineCollection();
        //    engines.Add(new FileSystemRazorViewEngine(viewsPath));

        //    var emailService = new Postal.EmailService(engines);

        //    var email = new SigupInvitationEmail
        //    {
        //        To = profile.EmailAddress,
        //        AppName = SANSurveyWebAPI.Constants.AppName,
        //        Name = profile.Name,
        //        Message = body
        //    };

        //    await email.SendAsync();
        //}




        #region SMS   
        //public ActionResult InviteSMS(int? id)
        //{
        //    if (!id.HasValue)
        //    {
        //        return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    Profile profile = db.Profiles.Where(p => p.Id == id.Value).SingleOrDefault();

        //    Guid uid = Guid.NewGuid();
        //    profile.Uid = uid.ToString();
        //    profile.RegistrationProgressNext = Constants.StatusRegistrationProgress.Invited.ToString();

        //    db.Entry(profile).State = EntityState.Modified;
        //    db.SaveChanges();

        //    if (profile == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    var accountSid = "ACe16948352dbb7be01f8e2cacb2c5089b"; // Your Account SID from www.twilio.com/console
        //    var authToken = "f084b6461e8278d8ebfa51f71526ea7a";  // Your Auth Token from www.twilio.com/console

        //    var twilio = new TwilioRestClient(accountSid, authToken);
        //    var message = twilio.SendMessage(
        //        "+18559765791", // From (Replace with your Twilio number)
        //        "+64223249110", // To (Replace with your phone number)
        //        "Dear <Insert Name> You are invited to participate in a survey being conducted by the Auckland University of Technology and Health Education England about the work day experience of junior doctors.Please go to www.aut.ac.nz/Signup?uid=a419eddb-9f74-40d2-a270-46efb30c6e2a to participate.Thank you."
        //        );




        //    return RedirectToAction("Index");
        //}
        #endregion


        #region deprecated due to Postal
        private async Task SendInvitationEmailRegular(string destination, string uid)
        {
            await Task.Run(() =>
            {
                string subject = "Sign-up for Survey";

                

                string body = GetBaseURL() + "Account/Signup" + "?uid=" + uid;

                SmtpClient client = new SmtpClient();


                MailMessage m = new MailMessage();
                m.From = new MailAddress(m.From.ToString());

                m.To.Add(new MailAddress(destination));

                m.CC.Add(new MailAddress(m.From.ToString()));
                m.Subject = "SurveyApp - " + subject;
                m.Body = body;

                client.Send(m);
            });
        }
        #endregion





    }
}