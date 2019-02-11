using SANSurveyWebAPI.DAL;
using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.Models.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static SANSurveyWebAPI.Constants;

namespace SANSurveyWebAPI.BLL
{
    public class JobService
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private ProfileService profileService;


        private HangfireScheduler _hangfireScheduler = new HangfireScheduler();

        public JobService()
        {
            this.profileService = new ProfileService();

        }

        private static double CalculateTimeToTrigger(DateTime shiftStartUserTime, double userOffsetFromUtc, double beforeOrAfter)
        {
            double serverLocationFromUtc = ((DateTime.Now - DateTime.UtcNow).TotalHours) * 60;
            DateTime adjShiftStartUtc = shiftStartUserTime.AddHours(-1 * userOffsetFromUtc);
            TimeSpan timeToShiftFromNowUtc = adjShiftStartUtc - DateTime.UtcNow;
            double shiftHoursFromNow = timeToShiftFromNowUtc.TotalHours;
            DateTime userShiftDStartDateTime = DateTime.UtcNow.AddHours(shiftHoursFromNow); //Servertime
            TimeSpan totalMin = userShiftDStartDateTime - DateTime.UtcNow;
            return totalMin.TotalMinutes + beforeOrAfter; //with repsect to server location
        }


        // Format the email reminder or the message for each job
        private object GetEmailDto(string clientInitials, string jobName, int profileId,
            string baseUrl, string link, CurrentProfile profile = null, ProfileRosterDto roster = null, double? timeToRun = null, Survey survey = null)
        {
            EmailDto obj = null;

            //added for jobName = "ShiftReminderEmail" 
            if (baseUrl == "" && jobName == "ShiftReminderEmail")
            {
                baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/App/";
            }
            //end of jobName = "ShiftReminderEmail"

            switch (jobName)
            {

                #region RecurrentSurveyReminders

                case "ExpiringSoonRecurrentSurveyNotCompletedEmail":

                    if (clientInitials.ToLower().ToString() == "jd")
                    {
                        ExpirinSoonNotCompletedSurveyReminderEmailDto ec1 = new ExpirinSoonNotCompletedSurveyReminderEmailDto();
                        ec1.ToEmail = profile.ProfileEmailAddress;
                        ec1.RecipientName = profile.ProfileName;

                        //ec1.Link = baseUrl + "MyDay/Index?id=" + survey.Uid.ToString();
                        ec1.Link = baseUrl + "Survey3/Index?id=" + survey.Uid.ToString();
                        //ec1.Link = baseUrl + "Account/Login";

                        ec1.Id = profile.ProfileId;
                        //ec1.RosterItemId = roster.Id;
                        ec1.SurveyWindowStartDateTime = survey.SurveyWindowStartDateTime.Value.ToString("dd-MMM-yyyy hh:mm tt");
                        ec1.SurveyWindowEndDateTime = survey.SurveyWindowEndDateTime.Value.ToString("dd-MMM-yyyy hh:mm tt");

                        ec1.SurveyWindowStartDate = survey.SurveyWindowStartDateTime.Value.ToString("dd-MMM-yyyy");
                        ec1.SurveyWindowEndDate = survey.SurveyWindowEndDateTime.Value.ToString("dd-MMM-yyyy");
                        ec1.SurveyWindowStartTime = survey.SurveyWindowStartDateTime.Value.ToString("hh:mm tt");
                        ec1.SurveyWindowEndTime = survey.SurveyWindowEndDateTime.Value.ToString("hh:mm tt");

                        obj = ec1;
                    }
                    else if (clientInitials.ToLower().ToString() == "wam")
                    {
                        ExpirinSoonNotCompletedWarrenSurveyReminderEmailDto expiringNotCompleted = new ExpirinSoonNotCompletedWarrenSurveyReminderEmailDto();
                        expiringNotCompleted.ToEmail = profile.ProfileEmailAddress;
                        expiringNotCompleted.RecipientName = profile.ProfileName;

                        //ec1.Link = baseUrl + "MyDay/Index?id=" + survey.Uid.ToString();
                        expiringNotCompleted.Link = baseUrl + "Survey3/Index?id=" + survey.Uid.ToString();
                        //ec1.Link = baseUrl + "Account/Login";

                        expiringNotCompleted.Id = profile.ProfileId;
                        //ec1.RosterItemId = roster.Id;
                        expiringNotCompleted.SurveyWindowStartDateTime = survey.SurveyWindowStartDateTime.Value.ToString("dd-MMM-yyyy hh:mm tt");
                        expiringNotCompleted.SurveyWindowEndDateTime = survey.SurveyWindowEndDateTime.Value.ToString("dd-MMM-yyyy hh:mm tt");

                        expiringNotCompleted.SurveyWindowStartDate = survey.SurveyWindowStartDateTime.Value.ToString("dd-MMM-yyyy");
                        expiringNotCompleted.SurveyWindowEndDate = survey.SurveyWindowEndDateTime.Value.ToString("dd-MMM-yyyy");
                        expiringNotCompleted.SurveyWindowStartTime = survey.SurveyWindowStartDateTime.Value.ToString("hh:mm tt");
                        expiringNotCompleted.SurveyWindowEndTime = survey.SurveyWindowEndDateTime.Value.ToString("hh:mm tt");

                        obj = expiringNotCompleted;
                    }
                    break;

                case "ExpiringSoonRecurrentSurveyNotStartedEmail":

                    if (clientInitials.ToLower().ToString() == "jd")
                    {
                        ExpiringSoonNotStartedSurveyReminderEmailDto es1 = new ExpiringSoonNotStartedSurveyReminderEmailDto();
                        es1.ToEmail = profile.ProfileEmailAddress;
                        es1.RecipientName = profile.ProfileName;
                        //es1.Link = baseUrl + "MyDay/Index?id=" + survey.Uid.ToString();
                        es1.Link = baseUrl + "Survey3/Index?id=" + survey.Uid.ToString();
                        //es1.Link = baseUrl + "Account/Login";
                        es1.Id = profile.ProfileId;
                        es1.RosterItemId = roster.Id;

                        es1.ScheduledDateTime = DateTime.Now.AddMinutes(timeToRun.Value).ToString("dd-MMM-yyyy hh:mm: tt");

                        es1.SurveyWindowStartDate = survey.SurveyWindowStartDateTime.Value.ToString("dd-MMM-yyyy");
                        es1.SurveyWindowEndDate = survey.SurveyWindowEndDateTime.Value.ToString("dd-MMM-yyyy");
                        es1.SurveyWindowStartTime = survey.SurveyWindowStartDateTime.Value.ToString("hh:mm tt");
                        es1.SurveyWindowEndTime = survey.SurveyWindowEndDateTime.Value.ToString("hh:mm tt");

                        obj = es1;
                    }
                    else if (clientInitials.ToLower().ToString() == "wam")
                    {
                        ExpiringSoonNotStartedWarrenSurveyReminderEmailDto expiringNotStarted = new ExpiringSoonNotStartedWarrenSurveyReminderEmailDto();
                        expiringNotStarted.ToEmail = profile.ProfileEmailAddress;
                        expiringNotStarted.RecipientName = profile.ProfileName;
                        //es1.Link = baseUrl + "MyDay/Index?id=" + survey.Uid.ToString();
                        expiringNotStarted.Link = baseUrl + "Survey3/Index?id=" + survey.Uid.ToString();
                        //es1.Link = baseUrl + "Account/Login";
                        expiringNotStarted.Id = profile.ProfileId;
                        expiringNotStarted.RosterItemId = roster.Id;

                        expiringNotStarted.ScheduledDateTime = DateTime.Now.AddMinutes(timeToRun.Value).ToString("dd-MMM-yyyy hh:mm: tt");

                        expiringNotStarted.SurveyWindowStartDate = survey.SurveyWindowStartDateTime.Value.ToString("dd-MMM-yyyy");
                        expiringNotStarted.SurveyWindowEndDate = survey.SurveyWindowEndDateTime.Value.ToString("dd-MMM-yyyy");
                        expiringNotStarted.SurveyWindowStartTime = survey.SurveyWindowStartDateTime.Value.ToString("hh:mm tt");
                        expiringNotStarted.SurveyWindowEndTime = survey.SurveyWindowEndDateTime.Value.ToString("hh:mm tt");

                        obj = expiringNotStarted;
                    }

                    break;

                case "CompleteRecurrentSurveyEmail":
                    if (clientInitials.ToLower().ToString() == "jd")
                    {
                        CompleteSurveyReminderEmailDto cs = new CompleteSurveyReminderEmailDto();
                        cs.ToEmail = profile.ProfileEmailAddress;
                        cs.RecipientName = profile.ProfileName;
                        //cs.Link = baseUrl + "MyDay/Index?id=" + survey.Uid.ToString();
                        cs.Link = baseUrl + "Survey3/Index?id=" + survey.Uid.ToString();
                        //cs.Link = baseUrl + "Account/Login";
                        cs.Id = profile.ProfileId;
                        cs.RosterItemId = roster.Id;

                        cs.ScheduledDateTime = DateTime.Now.AddMinutes(timeToRun.Value).ToString("dd-MMM-yyyy hh:mm: tt");

                        cs.SurveyWindowStartDate = survey.SurveyWindowStartDateTime.Value.ToString("dd-MMM-yyyy");
                        cs.SurveyWindowEndDate = survey.SurveyWindowEndDateTime.Value.ToString("dd-MMM-yyyy");
                        cs.SurveyWindowStartTime = survey.SurveyWindowStartDateTime.Value.ToString("hh:mm tt");
                        cs.SurveyWindowEndTime = survey.SurveyWindowEndDateTime.Value.ToString("hh:mm tt");

                        obj = cs;
                    }
                    else if (clientInitials.ToLower().ToString() == "wam")
                    {
                        //CompleteWarrenSurveyReminderEmailDto completeWarren = new CompleteWarrenSurveyReminderEmailDto();
                        //completeWarren.ToEmail = profile.ProfileEmailAddress;
                        //completeWarren.RecipientName = profile.ProfileName;
                        ////cs.Link = baseUrl + "MyDay/Index?id=" + survey.Uid.ToString();
                        //completeWarren.Link = baseUrl + "Survey3/Index?id=" + survey.Uid.ToString();
                        ////cs.Link = baseUrl + "Account/Login";
                        //completeWarren.Id = profile.ProfileId;
                        //completeWarren.RosterItemId = roster.Id;

                        //completeWarren.ScheduledDateTime = DateTime.Now.AddMinutes(timeToRun.Value).ToString("dd-MMM-yyyy hh:mm: tt");

                        //completeWarren.SurveyWindowStartDate = survey.SurveyWindowStartDateTime.Value.ToString("dd-MMM-yyyy");
                        //completeWarren.SurveyWindowEndDate = survey.SurveyWindowEndDateTime.Value.ToString("dd-MMM-yyyy");
                        //completeWarren.SurveyWindowStartTime = survey.SurveyWindowStartDateTime.Value.ToString("hh:mm tt");
                        //completeWarren.SurveyWindowEndTime = survey.SurveyWindowEndDateTime.Value.ToString("hh:mm tt");

                        //obj = completeWarren;

                        CompleteWarrenSurveyReminderEmailDto cs = new CompleteWarrenSurveyReminderEmailDto();
                        cs.ToEmail = profile.ProfileEmailAddress;
                        cs.RecipientName = profile.ProfileName;
                        //cs.Link = baseUrl + "MyDay/Index?id=" + survey.Uid.ToString();
                        cs.Link = baseUrl + "Survey3/Index?id=" + survey.Uid.ToString();
                        //cs.Link = baseUrl + "Account/Login";
                        cs.Id = profile.ProfileId;
                        cs.RosterItemId = roster.Id;

                        cs.ScheduledDateTime = DateTime.Now.AddMinutes(timeToRun.Value).ToString("dd-MMM-yyyy hh:mm: tt");

                        cs.SurveyWindowStartDate = survey.SurveyWindowStartDateTime.Value.ToString("dd-MMM-yyyy");
                        cs.SurveyWindowEndDate = survey.SurveyWindowEndDateTime.Value.ToString("dd-MMM-yyyy");
                        cs.SurveyWindowStartTime = survey.SurveyWindowStartDateTime.Value.ToString("hh:mm tt");
                        cs.SurveyWindowEndTime = survey.SurveyWindowEndDateTime.Value.ToString("hh:mm tt");

                        obj = cs;

                    }
                    break;

                case "StartRecurrentSurveyEmail":
                    if (clientInitials.ToLower().ToString() == "jd")
                    {
                        SurveyInvitationEmailDto startSurveyEmail = new SurveyInvitationEmailDto();
                        startSurveyEmail.ToEmail = profile.ProfileEmailAddress;
                        startSurveyEmail.Id = profile.ProfileId;
                        startSurveyEmail.SurveyId = survey.Id;
                        startSurveyEmail.RecipientName = profile.ProfileName;
                        //startSurveyEmail.Link = baseUrl + "MyDay/Index?id=" + survey.Uid.ToString();
                        startSurveyEmail.Link = baseUrl + "Survey3/Index?id=" + survey.Uid.ToString();
                        //startSurveyEmail.Link = baseUrl + "Account/Login";
                        startSurveyEmail.RosterItemId = roster.Id;

                        startSurveyEmail.ScheduledDateTime = DateTime.Now.AddMinutes(timeToRun.Value).ToString("dd-MMM-yyyy hh:mm tt");

                        startSurveyEmail.SurveyWindowStartDate = survey.SurveyWindowStartDateTime.Value.ToString("dd-MMM-yyyy");
                        startSurveyEmail.SurveyWindowEndDate = survey.SurveyWindowEndDateTime.Value.ToString("dd-MMM-yyyy");
                        startSurveyEmail.SurveyWindowStartTime = survey.SurveyWindowStartDateTime.Value.ToString("hh:mm tt");
                        startSurveyEmail.SurveyWindowEndTime = survey.SurveyWindowEndDateTime.Value.ToString("hh:mm tt");
                        obj = startSurveyEmail;
                    }
                    else if (clientInitials.ToLower().ToString() == "wam")
                    {
                        StartWarrenSurveyInvitationEmailDto startWarrenSurveyEmail = new StartWarrenSurveyInvitationEmailDto();
                        startWarrenSurveyEmail.ToEmail = profile.ProfileEmailAddress;
                        startWarrenSurveyEmail.Id = profile.ProfileId;
                        startWarrenSurveyEmail.SurveyId = survey.Id;
                        startWarrenSurveyEmail.RecipientName = profile.ProfileName;
                        //startSurveyEmail.Link = baseUrl + "MyDay/Index?id=" + survey.Uid.ToString();
                        startWarrenSurveyEmail.Link = baseUrl + "Survey3/Index?id=" + survey.Uid.ToString();
                        //startSurveyEmail.Link = baseUrl + "Account/Login";
                        startWarrenSurveyEmail.RosterItemId = roster.Id;

                        startWarrenSurveyEmail.ScheduledDateTime = DateTime.Now.AddMinutes(timeToRun.Value).ToString("dd-MMM-yyyy hh:mm tt");

                        startWarrenSurveyEmail.SurveyWindowStartDate = survey.SurveyWindowStartDateTime.Value.ToString("dd-MMM-yyyy");
                        startWarrenSurveyEmail.SurveyWindowEndDate = survey.SurveyWindowEndDateTime.Value.ToString("dd-MMM-yyyy");
                        startWarrenSurveyEmail.SurveyWindowStartTime = survey.SurveyWindowStartDateTime.Value.ToString("hh:mm tt");
                        startWarrenSurveyEmail.SurveyWindowEndTime = survey.SurveyWindowEndDateTime.Value.ToString("hh:mm tt");
                        obj = startWarrenSurveyEmail;
                    }
                    break;


                case "ShiftReminderEmail":
                    if (clientInitials.ToLower().ToString() == "jd")
                    {
                        ShiftStartReminderEmailDto shiftEmail = new ShiftStartReminderEmailDto();
                        shiftEmail.ToEmail = profile.ProfileEmailAddress;
                        shiftEmail.RecipientName = profile.ProfileName;
                        shiftEmail.Link = baseUrl + "Calendar/List"; //baseUrl + "Survey3/Index?id=" + survey.Uid.ToString();
                        shiftEmail.Id = profile.ProfileId;
                        shiftEmail.ScheduledDateTime = DateTime.Now.AddMinutes(timeToRun.Value).ToString("dd-MMM-yyyy hh:mm tt");
                        shiftEmail.ShiftStartDate = roster.Start.ToString("dd-MMM-yyyy");
                        shiftEmail.ShiftEndDate = roster.End.ToString("dd-MMM-yyyy");
                        shiftEmail.ShiftStartTime = roster.Start.ToString("hh:mm tt");
                        shiftEmail.ShiftEndTime = roster.End.ToString("hh:mm tt");

                        obj = shiftEmail;
                    }
                    else if (clientInitials.ToLower().ToString() == "wam")
                    {
                        ShiftReminderWarrenMahonyEmailDto shiftWarrenEmail = new ShiftReminderWarrenMahonyEmailDto();
                        shiftWarrenEmail.ToEmail = profile.ProfileEmailAddress;
                        shiftWarrenEmail.RecipientName = profile.ProfileName;
                        shiftWarrenEmail.Link = baseUrl + "Calendar/List"; //baseUrl + "Survey3/Index?id=" + survey.Uid.ToString();
                        shiftWarrenEmail.Id = profile.ProfileId;
                        shiftWarrenEmail.ScheduledDateTime = DateTime.Now.AddMinutes(timeToRun.Value).ToString("dd-MMM-yyyy hh:mm tt");
                        shiftWarrenEmail.ShiftStartDate = roster.Start.ToString("dd-MMM-yyyy");
                        shiftWarrenEmail.ShiftEndDate = roster.End.ToString("dd-MMM-yyyy");
                        shiftWarrenEmail.ShiftStartTime = roster.Start.ToString("hh:mm tt");
                        shiftWarrenEmail.ShiftEndTime = roster.End.ToString("hh:mm tt");

                        obj = shiftWarrenEmail;
                    }
                    break;

                #endregion


                #region Registration

                case "ExitSurveyEmail":
                    var k3 = _unitOfWork.ProfileRespository.GetByID(profileId);

                    Guid uid2 = Guid.NewGuid();

                    //profileService.ResetProfile(profileId, uid2, emailJobId: null, smsJobId: null);

                    ExitSurveyInvitationEmailDto exitEmail = new ExitSurveyInvitationEmailDto();
                    exitEmail.ToEmail = StringCipher.DecryptRfc2898(k3.LoginEmail);
                    exitEmail.RecipientName = k3.Name;
                    exitEmail.Link = baseUrl + "ExitSurvey/Index" + "/" + uid2;
                    obj = exitEmail;
                    break;


                case "RegisterBaselineSurveyEmail":
                    var k = _unitOfWork.ProfileRespository.GetByID(profileId);

                    Guid uid = Guid.NewGuid();
                    profileService.ResetProfile(profileId, uid, emailJobId: null, smsJobId: null);
                    RegistrationInvitationEmailDto regEmail = new RegistrationInvitationEmailDto();
                    regEmail.ToEmail = StringCipher.DecryptRfc2898(k.LoginEmail);
                    regEmail.RecipientName = k.Name;
                    regEmail.Link = baseUrl + "Register/Index" + "/" + uid;
                    obj = regEmail;
                    break;


                case "RegistrationCompletedEmail":
                    if (clientInitials.ToLower().ToString() == "jd")
                    {
                        var k2 = _unitOfWork.ProfileRespository.GetByID(profileId);
                        //SignupCompletedEmailDto completeRegEmail = new SignupCompletedEmailDto();
                        SignupCompletedEmailDto completeRegEmail = new WarrenMahonySignupCompletedEmailDto();
                        completeRegEmail.ToEmail = StringCipher.DecryptRfc2898(k2.LoginEmail);
                        completeRegEmail.RecipientName = k2.Name;
                        completeRegEmail.Link = baseUrl;
                        obj = completeRegEmail;

                    }
                    else if (clientInitials.ToLower().ToString() == "wam")
                    {
                        var k2 = _unitOfWork.ProfileRespository.GetByID(profileId);
                        //SignupCompletedEmailDto completeRegEmail = new SignupCompletedEmailDto();
                        WarrenMahonySignupCompletedEmailDto completeRegEmail = new WarrenMahonySignupCompletedEmailDto();
                        completeRegEmail.ToEmail = StringCipher.DecryptRfc2898(k2.LoginEmail);
                        completeRegEmail.RecipientName = k2.Name;
                        completeRegEmail.Link = baseUrl;
                        obj = completeRegEmail;
                    }

                    break;
                #endregion


                default:
                    break;
            }

            return obj;
        }

        private async Task<int?> CreateHangfireJob(string jobName, string jobType, string jobMethod, int profileId,
            string baseUrl, CurrentProfile profile = null, ProfileRosterDto roster = null, Survey recurrentSurvey = null)
        {
            //Chech which job and create the respective job
            int? hangfireJobId = null;
            double timeToRun = 0; //when the job should be run after NOW
            bool exitingAutoJob = false;
            string clientInitials = string.Empty;
            ProfileDetailsByClient pc = new ProfileDetailsByClient();

            if (profile == null || profile.ClientInitials == null || profile.ClientInitials.ToString() == "")
            {
                pc = profileService.GetClientDetailsByProfileId(profileId);
                clientInitials = pc.ClientInitials.ToString();
            }
            else
            {
                clientInitials = profile.ClientInitials.ToString();
            }

            switch (jobName)
            {

                case "ExpiringSoonRecurrentSurveyNotCompletedEmail":
                    NewSurvey newSurvey5 = new NewSurvey();
                    newSurvey5.ProfileId = profile.ProfileId;
                    newSurvey5.OffsetUtc = profile.OffsetFromUTC;
                    newSurvey5.SurveyId = recurrentSurvey.Id;
                    newSurvey5.RosterItemId = roster.Id;
                    //newSurvey5.ShiftStart = roster.Start;
                    //newSurvey5.ShiftEnd = roster.End;

                    if (jobMethod == JobMethod.Manual.ToString())
                    {
                        timeToRun = 0;
                        exitingAutoJob = true;
                    }
                    else
                    {
                        timeToRun = CalculateTimeToTrigger(recurrentSurvey.SurveyExpiryDateTime.Value, profile.OffsetFromUTC, -1 * REMINDER_BEFORE_EXPIRY_HRS * 60);
                        exitingAutoJob = true;
                    }

                    //Remove pre-exisint jobs, so that duplicate emails are not sent
                    if (exitingAutoJob)
                    {

                        var jobs = _unitOfWork.JobLogExpiringSoonSurveyNotCompletedReminderEmailRespository
                            .Get(x => x.ProfileRosterId == roster.Id);

                        foreach (var x in jobs)
                        {
                            if (x.HangfireJobId.HasValue)
                            {
                                await _hangfireScheduler.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                            }
                        }

                        //_unitOfWork.JobLogExpiringSoonSurveyNotCompletedReminderEmailRespository.RemoveRange(jobs);
                        //_unitOfWork.SaveChanges();
                    }

                    hangfireJobId = await ExpiringSoonRecurrentSurveyNotCompletedEmail(jobName, jobMethod, profileId, baseUrl, profile, roster, hangfireJobId, timeToRun, newSurvey5, recurrentSurvey);

                    break;

                case "ExpiringSoonRecurrentSurveyNotStartedEmail":
                    NewSurvey newSurvey4 = new NewSurvey();
                    newSurvey4.ProfileId = profile.ProfileId;
                    newSurvey4.OffsetUtc = profile.OffsetFromUTC;
                    newSurvey4.RosterItemId = roster.Id;
                    newSurvey4.ShiftStart = roster.Start;
                    newSurvey4.ShiftEnd = roster.End;
                    newSurvey4.SurveyId = recurrentSurvey.Id;

                    if (jobMethod == JobMethod.Manual.ToString())
                    {
                        timeToRun = 0;
                        exitingAutoJob = true;
                    }
                    else
                    {
                        timeToRun = CalculateTimeToTrigger(recurrentSurvey.SurveyExpiryDateTime.Value, profile.OffsetFromUTC, -1 * REMINDER_BEFORE_EXPIRY_HRS * 60);
                        exitingAutoJob = true;
                    }

                    //Remove pre-exisint jobs, so that duplicate emails are not sent
                    if (exitingAutoJob)
                    {

                        var jobs = _unitOfWork.JobLogExpiringSoonSurveyNotStartedReminderEmailRespository
                            .Get(x => x.ProfileRosterId == roster.Id);

                        foreach (var x in jobs)
                        {
                            if (x.HangfireJobId.HasValue)
                            {
                                await _hangfireScheduler.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                            }
                        }

                        //_unitOfWork.JobLogExpiringSoonSurveyNotStartedReminderEmailRespository.RemoveRange(jobs);
                        //_unitOfWork.SaveChanges();
                    }

                    hangfireJobId = await ExpiringSoonRecurrentSurveyNotStartedEmail(jobName, jobMethod, profileId, baseUrl, profile, roster, hangfireJobId, timeToRun, newSurvey4, recurrentSurvey);
                    break;

                case "CompleteRecurrentSurveyEmail":
                    if (clientInitials.ToLower().ToString() == "jd")
                    {
                        NewSurvey newSurvey3 = new NewSurvey();
                        newSurvey3.ProfileId = profile.ProfileId;
                        newSurvey3.OffsetUtc = profile.OffsetFromUTC;
                        newSurvey3.RosterItemId = roster.Id;
                        newSurvey3.ShiftStart = roster.Start;
                        newSurvey3.ShiftEnd = roster.End;
                        newSurvey3.SurveyId = recurrentSurvey.Id;

                        if (jobMethod == JobMethod.Manual.ToString())
                        {
                            timeToRun = 0;
                            exitingAutoJob = true;
                        }
                        else
                        {
                            timeToRun = CalculateTimeToTrigger(roster.End, profile.OffsetFromUTC, REMINDER_AFTER_SHIFT_END_HRS * 60);
                            exitingAutoJob = true;
                        }

                        //Remove pre-exisint jobs, so that duplicate emails are not sent
                        if (exitingAutoJob)
                        {

                            var jobs = _unitOfWork.JobLogCompleteSurveyReminderEmailRespository
                                .Get(x => x.ProfileRosterId == roster.Id);

                            foreach (var x in jobs)
                            {
                                if (x.HangfireJobId.HasValue)
                                {
                                    await _hangfireScheduler.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                                }
                            }

                            _unitOfWork.JobLogCompleteSurveyReminderEmailRespository.RemoveRange(jobs);
                            _unitOfWork.SaveChanges();
                        }

                        hangfireJobId = await CompleteRecurrentSurveyEmail(jobName, jobMethod, profileId, baseUrl, profile, roster, hangfireJobId, timeToRun, newSurvey3, recurrentSurvey);

                    }
                    else if (clientInitials.ToLower().ToString() == "wam")
                    {

                        NewSurvey newSurvey3 = new NewSurvey();
                        newSurvey3.ProfileId = profile.ProfileId;
                        newSurvey3.OffsetUtc = profile.OffsetFromUTC;
                        newSurvey3.RosterItemId = roster.Id;
                        newSurvey3.ShiftStart = roster.Start;
                        newSurvey3.ShiftEnd = roster.End;
                        newSurvey3.SurveyId = recurrentSurvey.Id;

                        if (jobMethod == JobMethod.Manual.ToString())
                        {
                            timeToRun = 0;
                            exitingAutoJob = true;
                        }
                        else
                        {
                            timeToRun = CalculateTimeToTrigger(roster.End, profile.OffsetFromUTC, REMINDER_AFTER_SHIFT_END_HRS * 60);
                            exitingAutoJob = true;
                        }

                        //Remove pre-exisint jobs, so that duplicate emails are not sent
                        if (exitingAutoJob)
                        {

                            var jobs = _unitOfWork.JobLogCompleteSurveyReminderEmailRespository
                                .Get(x => x.ProfileRosterId == roster.Id);

                            foreach (var x in jobs)
                            {
                                if (x.HangfireJobId.HasValue)
                                {
                                    await _hangfireScheduler.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                                }
                            }

                            _unitOfWork.JobLogCompleteSurveyReminderEmailRespository.RemoveRange(jobs);
                            _unitOfWork.SaveChanges();
                        }

                        hangfireJobId = await CompleteRecurrentSurveyEmail(jobName, jobMethod, profileId, baseUrl, profile, roster, hangfireJobId, timeToRun, newSurvey3, recurrentSurvey);


                    }
                    break;

                case "StartRecurrentSurveyEmail":

                    NewSurvey newSurvey2 = new NewSurvey();
                    newSurvey2.ProfileId = profile.ProfileId;
                    newSurvey2.OffsetUtc = profile.OffsetFromUTC;
                    newSurvey2.RosterItemId = roster.Id;
                    newSurvey2.ShiftStart = roster.Start;
                    newSurvey2.ShiftEnd = roster.End;
                    newSurvey2.SurveyId = recurrentSurvey.Id;

                    if (jobMethod == JobMethod.Manual.ToString())
                    {
                        timeToRun = 0;
                        exitingAutoJob = true;
                    }
                    else
                    {
                        timeToRun = CalculateTimeToTrigger(recurrentSurvey.SurveyWindowEndDateTime.Value, profile.OffsetFromUTC, 0);
                        exitingAutoJob = true;
                    }

                    //Remove pre-exisint jobs, so that duplicate emails are not sent
                    #region SurveyReminder
                    if (exitingAutoJob)
                    {

                        var startSurveyReminders = _unitOfWork.JobLogStartSurveyReminderEmailRespository
                            .Get(x => x.ProfileRosterId == roster.Id);

                        foreach (var x in startSurveyReminders)
                        {
                            if (x.HangfireJobId.HasValue)
                            {
                                await _hangfireScheduler.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                            }
                        }

                        //_unitOfWork.JobLogStartSurveyReminderEmailRespository.RemoveRange(startSurveyReminders);
                        //_unitOfWork.SaveChanges();
                    }
                    #endregion

                    #region StartSurveyEmailReminder Job
                    hangfireJobId = await StartRecurrentSurveyEmail(jobName, jobMethod, profileId, baseUrl, profile, roster, hangfireJobId, timeToRun, newSurvey2, recurrentSurvey);
                    #endregion


                    #region Update Survey Job
                    int? hangfireJobIdUpdateStatus2 = await UpdateSurveyStatus(jobMethod, profileId, roster, newSurvey2, timeToRun);
                    #endregion

                    break;

                case "CreateSurveyJob":
                    #region CreateSurveyJob
                    //Has to run at start of roster, trigger
                    timeToRun = CalculateTimeToTrigger(roster.Start, profile.OffsetFromUTC, 0);
                    //Create a New Survey
                    NewSurvey e = new NewSurvey();
                    e.ProfileId = profile.ProfileId;
                    e.OffsetUtc = profile.OffsetFromUTC;
                    e.RosterItemId = roster.Id;
                    e.ShiftStart = roster.Start;
                    e.ShiftEnd = roster.End;
                    e.SurveyId = 0;
                    //e.BaseUrl = baseUrl;
                    //e.EmailAddress = currProfile.ProfileEmailAddress;
                    //e.RecipientName = currProfile.ProfileName;

                    if (jobMethod == JobMethod.Manual.ToString())
                    {
                        timeToRun = 0;
                    }

                    //e.ScheduledDateTime = DateTime.Now.AddMinutes(timeToRun).ToString("dd/MM/yyyy HH:mm:ss");
                    Survey survey = SurveyService.CreateSurveyFromNewSurvey(e);

                    //TODO: send the actual survey object instead of new survey in the previous method

                    e.ScheduledDateTime = DateTime.Now.AddMinutes(timeToRun).ToString();
                    e.SurveyStartDateTime = survey.SurveyWindowStartDateTime.Value;
                    /* Manually create the survey instead of the scheduler*/
                    //hangfireJobId = await _hangfireScheduler.CreateSurveyJob(e, timeToRun);
                    //await SurveyService.CreateSurveyScheduled(e, string.Empty);

                    ApplicationDbContext db = new ApplicationDbContext();
                    db.Surveys.Add(survey); //SaveSurvey
                    int createSurveyJobId = await db.SaveChangesAsync();
                    //await db.SaveChangesAsync();
                    e.SurveyId = survey.Id;

                    JobLogCreateSurvey d4 = new JobLogCreateSurvey();
                    d4.HangfireJobId = hangfireJobId;
                    d4.ProfileId = profileId;
                    d4.ProfileRosterId = roster.Id;
                    d4.JobMethod = jobMethod;
                    d4.RunAfterMin = timeToRun;
                    d4.CreatedDateTimeUtc = DateTime.UtcNow;
                    d4.CreatedDateTimeServer = DateTime.Now;

                    if (e.SurveyId != null)
                    {
                        d4.SurveyId = e.SurveyId;
                    }
                    else
                    {
                        d4.SurveyId = 0;
                    }

                    _unitOfWork.JobLogCreateSurveyRespository.Insert(d4);
                    await _unitOfWork.SaveChangesAsync(); //Create Job
                    #endregion

                    #region StartSurveyEmailReminder Job
                    //Should be reminded just at SurveyEnd
                    jobName = "StartRecurrentSurveyEmail";
                    timeToRun = CalculateTimeToTrigger(survey.SurveyWindowEndDateTime.Value, profile.OffsetFromUTC, 0);
                    hangfireJobId = await StartRecurrentSurveyEmail(jobName, jobMethod, profileId, baseUrl, profile, roster, hangfireJobId, timeToRun, e, survey);
                    #endregion

                    #region Update Survey Job
                    int? hangfireJobIdUpdateStatus = await UpdateSurveyStatus(jobMethod, profileId, roster, e, timeToRun);
                    #endregion

                    #region CompleteSurveyReminderEmail Job
                    //Should be reminded 2 hours after shift ends
                    //if (clientInitials.ToLower().ToString() == "jd")
                    //{
                        jobName = "CompleteRecurrentSurveyEmail";
                        timeToRun = CalculateTimeToTrigger(roster.End, profile.OffsetFromUTC, REMINDER_AFTER_SHIFT_END_HRS * 60);
                        hangfireJobId = await CompleteRecurrentSurveyEmail(jobName, jobMethod, profileId, baseUrl, profile, roster, hangfireJobId, timeToRun, e, survey);
                    //}
                    //else if (clientInitials.ToLower().ToString() == "wam")
                    //{
                    //}
                    #endregion

                    #region ExpiringSoonNotStartedReminderEmail Job
                    //Should be reminded 5 hours before expiry
                    jobName = "ExpiringSoonRecurrentSurveyNotStartedEmail";
                    timeToRun = CalculateTimeToTrigger(survey.SurveyExpiryDateTime.Value, profile.OffsetFromUTC, -1 * REMINDER_BEFORE_EXPIRY_HRS * 60);
                    hangfireJobId = await ExpiringSoonRecurrentSurveyNotStartedEmail(jobName, jobMethod, profileId, baseUrl, profile, roster, hangfireJobId, timeToRun, e, survey);
                    #endregion

                    break;


                #region ShiftReminderEmail
                case "ShiftReminderEmail":

                    if (jobMethod == JobMethod.Manual.ToString())
                    {
                        timeToRun = 0;
                        exitingAutoJob = true;
                    }
                    else
                    {
                        timeToRun = CalculateTimeToTrigger(roster.Start, profile.OffsetFromUTC, -1 * Constants.REMINDER_SHIFT_START_BEFORE_HRS * 60);
                        exitingAutoJob = true;
                    }

                    //Remove pre-exisint jobs, so that duplicate emails are not sent
                    if (exitingAutoJob)
                    {

                        var reminders = _unitOfWork.JobLogShiftReminderEmailRespository
                            .Get(x => x.ProfileRosterId == roster.Id);

                        foreach (var x in reminders)
                        {
                            if (x.HangfireJobId.HasValue)
                            {
                                await _hangfireScheduler.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                            }
                        }

                        //_unitOfWork.JobLogShiftReminderEmailRespository.RemoveRange(reminders);
                        //_unitOfWork.SaveChanges();
                    }

                    if (clientInitials.ToLower().ToString() == "jd")
                    {
                        ShiftStartReminderEmailDto e2 = (ShiftStartReminderEmailDto)GetEmailDto(clientInitials, jobName, profileId, baseUrl, string.Empty, profile, roster, timeToRun);
                        e2.RosterItemId = roster.Id;
                        e2.ScheduledDateTime = DateTime.Now.AddMinutes(timeToRun).ToString();
                        //ec1.ScheduledDateTime = DateTime.Now.AddMinutes(timeToRun.Value).ToString("dd-MMM-yyyy hh:mm tt");

                        hangfireJobId = await _hangfireScheduler.ShiftReminderEmail(e2, timeToRun);
                    }
                    else if (clientInitials.ToLower().ToString() == "wam")
                    {
                        ShiftReminderWarrenMahonyEmailDto e2 = (ShiftReminderWarrenMahonyEmailDto)GetEmailDto(clientInitials, jobName, profileId, baseUrl, string.Empty, profile, roster, timeToRun);
                        e2.RosterItemId = roster.Id;
                        e2.ScheduledDateTime = DateTime.Now.AddMinutes(timeToRun).ToString();

                        hangfireJobId = await _hangfireScheduler.ShiftReminderWarrenEmail(e2, timeToRun);
                    }

                    JobLogShiftReminderEmail d3 = new JobLogShiftReminderEmail();
                    d3.HangfireJobId = hangfireJobId;
                    d3.ProfileId = profileId;
                    d3.ProfileRosterId = roster.Id;
                    d3.JobMethod = jobMethod;
                    d3.RunAfterMin = timeToRun;
                    d3.CreatedDateTimeUtc = DateTime.UtcNow;
                    d3.CreatedDateTimeServer = DateTime.Now;
                    //d.Email = e1; //TODO
                    _unitOfWork.JobLogShiftReminderEmailRespository.Insert(d3);
                    _unitOfWork.SaveChanges();
                    break;
                #endregion 



                #region Registration and Baseline

                case "ExitSurveyEmail":

                    timeToRun = 0; //Schedule now

                    ExitSurveyInvitationEmailDto emailExit = (ExitSurveyInvitationEmailDto)GetEmailDto(clientInitials, jobName, profileId, baseUrl, string.Empty);

                    if (jobMethod == JobMethod.Manual.ToString())
                    {
                        timeToRun = 0;
                    }

                    hangfireJobId = await _hangfireScheduler.ExitSurveyEmail(emailExit, timeToRun);

                    JobLogExitSurveyEmail exitJob = new JobLogExitSurveyEmail();
                    exitJob.HangfireJobId = hangfireJobId;
                    exitJob.ProfileId = profileId;
                    exitJob.JobMethod = jobMethod;
                    exitJob.RunAfterMin = timeToRun;
                    exitJob.CreatedDateTimeUtc = DateTime.UtcNow;
                    exitJob.CreatedDateTimeServer = DateTime.Now;
                    //d.Email = e1; //TODO
                    _unitOfWork.JobLogExitSurveyEmailRespository.Insert(exitJob);
                    _unitOfWork.SaveChanges(); //Create Job
                    break;



                case "RegisterBaselineSurveyEmail":

                    timeToRun = 0; //Schedule now

                    RegistrationInvitationEmailDto e1 = (RegistrationInvitationEmailDto)GetEmailDto(clientInitials, jobName, profileId, baseUrl, string.Empty);

                    if (jobMethod == JobMethod.Manual.ToString())
                    {
                        timeToRun = 0;
                    }

                    hangfireJobId = await _hangfireScheduler.RegisterBaselineSurveyEmail(e1, timeToRun);

                    JobLogBaselineSurveyEmail d = new JobLogBaselineSurveyEmail();
                    d.HangfireJobId = hangfireJobId;
                    d.ProfileId = profileId;
                    d.JobMethod = jobMethod;
                    d.RunAfterMin = timeToRun;
                    d.CreatedDateTimeUtc = DateTime.UtcNow;
                    d.CreatedDateTimeServer = DateTime.Now;
                    //d.Email = e1; //TODO
                    _unitOfWork.JobLogBaselineSurveyEmailRespository.Insert(d);
                    _unitOfWork.SaveChanges(); //Create Job
                    break;

                case "RegistrationCompletedEmail":

                    timeToRun = 0; //Schedule now

                    if (clientInitials.ToLower().ToString() == "jd")
                    {
                        SignupCompletedEmailDto e3 = (WarrenMahonySignupCompletedEmailDto)GetEmailDto(clientInitials, jobName, profileId, baseUrl, string.Empty);
                        hangfireJobId = await _hangfireScheduler.RegistrationCompletedEmail(e3, timeToRun);
                    }
                    else if (clientInitials.ToLower().ToString() == "wam")
                    {
                        WarrenMahonySignupCompletedEmailDto e3 = (WarrenMahonySignupCompletedEmailDto)GetEmailDto(clientInitials, jobName, profileId, baseUrl, string.Empty);
                        hangfireJobId = await _hangfireScheduler.WAMRegistrationCompletedEmail(e3, timeToRun);
                    }

                    if (jobMethod == JobMethod.Manual.ToString())
                    {
                        timeToRun = 0;
                    }

                    JobLogRegistrationCompletedEmail d2 = new JobLogRegistrationCompletedEmail();
                    d2.HangfireJobId = hangfireJobId;
                    d2.ProfileId = profileId;
                    d2.JobMethod = jobMethod;
                    d2.RunAfterMin = timeToRun;
                    d2.CreatedDateTimeUtc = DateTime.UtcNow;
                    d2.CreatedDateTimeServer = DateTime.Now;
                    //d.Email = e1; //TODO
                    _unitOfWork.JobLogRegistrationCompletedEmailRespository.Insert(d2);
                    _unitOfWork.SaveChanges(); //Create Job
                    break;

                #endregion



                default:
                    break;
            }

            return hangfireJobId;
        }


        private async Task<int?> UpdateSurveyStatus(string jobMethod, int profileId, ProfileRosterDto roster, NewSurvey e, double timeToRun)
        {
            int? hangfireJobId = await _hangfireScheduler.UpdateSurveyStatus(e, timeToRun);

            JobLogUpdateSurvey log = new JobLogUpdateSurvey();
            log.HangfireJobId = hangfireJobId;
            log.ProfileId = profileId;
            log.ProfileRosterId = roster.Id;
            log.JobMethod = jobMethod;
            log.RunAfterMin = timeToRun;
            log.CreatedDateTimeUtc = DateTime.UtcNow;
            log.CreatedDateTimeServer = DateTime.Now;
            if (e.SurveyId != null)
            {
                log.SurveyId = e.SurveyId;
            }
            else
            {
                log.SurveyId = 0;
            }

            _unitOfWork.JobLogUpdateSurveyRespository.Insert(log);
            _unitOfWork.SaveChanges();
            return hangfireJobId;
        }




        private async Task<int?> ExpiringSoonRecurrentSurveyNotCompletedEmail(string jobName, string jobMethod, int profileId,
            string baseUrl, CurrentProfile profile, ProfileRosterDto roster, int? hangfireJobId,
            double timeToRun, NewSurvey e, Survey survey)
        {
            if (profile.ClientInitials.ToLower().ToString() == "jd")
            {
                ExpirinSoonNotCompletedSurveyReminderEmailDto email = (ExpirinSoonNotCompletedSurveyReminderEmailDto)GetEmailDto(profile.ClientInitials, jobName, profileId, baseUrl, string.Empty, profile, roster, timeToRun, survey);
                email.SurveyId = e.SurveyId;
                email.RosterItemId = e.RosterItemId;

                hangfireJobId = await _hangfireScheduler.ExpiringSoonNotCompletedSurveyEmail(email, timeToRun);
            }
            else if (profile.ClientInitials.ToLower().ToString() == "wam")
            {
                ExpirinSoonNotCompletedWarrenSurveyReminderEmailDto email = (ExpirinSoonNotCompletedWarrenSurveyReminderEmailDto)GetEmailDto(profile.ClientInitials, jobName, profileId, baseUrl, string.Empty, profile, roster, timeToRun, survey);
                email.SurveyId = e.SurveyId;
                email.RosterItemId = e.RosterItemId;

                hangfireJobId = await _hangfireScheduler.ExpirinSoonNotCompletedWarrenSurveyReminderEmail(email, timeToRun);
            }

            JobLogExpiringSoonSurveyNotCompletedReminderEmail log = new JobLogExpiringSoonSurveyNotCompletedReminderEmail();
            log.HangfireJobId = hangfireJobId;
            log.ProfileId = profileId;
            log.ProfileRosterId = roster.Id;
            log.JobMethod = jobMethod;
            log.RunAfterMin = timeToRun;
            log.CreatedDateTimeUtc = DateTime.UtcNow;
            log.CreatedDateTimeServer = DateTime.Now;
            //d4.SurveyId = e.SurveyId;

            if (e.SurveyId != null)
            {
                log.SurveyId = e.SurveyId;
            }
            else
            {
                log.SurveyId = 0;
            }


            _unitOfWork.JobLogExpiringSoonSurveyNotCompletedReminderEmailRespository.Insert(log);
            _unitOfWork.SaveChanges();
            return hangfireJobId;
        }

        private async Task<int?> ExpiringSoonRecurrentSurveyNotStartedEmail(string jobName, string jobMethod, int profileId, string baseUrl, CurrentProfile profile, ProfileRosterDto roster, int? hangfireJobId, double timeToRun, NewSurvey e, Survey survey)
        {
            if (profile.ClientInitials.ToLower().ToString() == "jd")
            {
                ExpiringSoonNotStartedSurveyReminderEmailDto email = (ExpiringSoonNotStartedSurveyReminderEmailDto)GetEmailDto(profile.ClientInitials, jobName, profileId, baseUrl, string.Empty, profile, roster, timeToRun, survey);
                email.SurveyId = e.SurveyId;

                hangfireJobId = await _hangfireScheduler.ExpiringSoonNotStarteSurveyEmail(email, timeToRun);
            }
            else if (profile.ClientInitials.ToLower().ToString() == "wam")
            {
                ExpiringSoonNotStartedWarrenSurveyReminderEmailDto email = (ExpiringSoonNotStartedWarrenSurveyReminderEmailDto)GetEmailDto(
                                    profile.ClientInitials, jobName, profileId, baseUrl, string.Empty, profile, roster, timeToRun, survey);
                email.SurveyId = e.SurveyId;

                hangfireJobId = await _hangfireScheduler.ExpiringSoonNotStartedWarrenSurveyReminderEmail(email, timeToRun);
            }

            JobLogExpiringSoonSurveyNotStartedReminderEmail log = new JobLogExpiringSoonSurveyNotStartedReminderEmail();
            log.HangfireJobId = hangfireJobId;
            log.ProfileId = profileId;
            log.ProfileRosterId = roster.Id;
            log.JobMethod = jobMethod;
            log.RunAfterMin = timeToRun;
            log.CreatedDateTimeUtc = DateTime.UtcNow;
            log.CreatedDateTimeServer = DateTime.Now;
            //d4.SurveyId = e.SurveyId;

            if (e.SurveyId != null)
            {
                log.SurveyId = e.SurveyId;
            }
            else
            {
                log.SurveyId = 0;
            }

            _unitOfWork.JobLogExpiringSoonSurveyNotStartedReminderEmailRespository.Insert(log);
            _unitOfWork.SaveChanges(); //Create Job
            return hangfireJobId;
        }

        private async Task<int?> CompleteRecurrentSurveyEmail(string jobName, string jobMethod, int profileId, string baseUrl, CurrentProfile profile, ProfileRosterDto roster, int? hangfireJobId, double timeToRun, NewSurvey e, Survey survey)
        {
            //CompleteSurveyReminderEmailDto email = (CompleteSurveyReminderEmailDto)GetEmailDto(profile.ClientInitials, jobName, profileId, baseUrl, string.Empty, profile, roster, timeToRun, survey);
            //email.SurveyId = e.SurveyId;

            //hangfireJobId = await _hangfireScheduler.CompelteRecurrentSurveyEmail(email, timeToRun);


            if (profile.ClientInitials.ToLower().ToString() == "jd")
            {
                CompleteSurveyReminderEmailDto email = (CompleteSurveyReminderEmailDto)GetEmailDto(profile.ClientInitials, jobName, profileId, baseUrl, string.Empty, profile, roster, timeToRun, survey);
                email.SurveyId = e.SurveyId;
                hangfireJobId = await _hangfireScheduler.CompelteRecurrentSurveyEmail(email, timeToRun);
            }
            else if (profile.ClientInitials.ToLower().ToString() == "wam")
            {
                CompleteWarrenSurveyReminderEmailDto email = (CompleteWarrenSurveyReminderEmailDto) GetEmailDto(profile.ClientInitials, jobName, profileId, baseUrl, string.Empty, profile, roster, timeToRun, survey);
                email.SurveyId = e.SurveyId;
                hangfireJobId = await _hangfireScheduler.CompleteWarrenRecurrentSurveyEmail(email, timeToRun);
            }

            JobLogCompleteSurveyReminderEmail log = new JobLogCompleteSurveyReminderEmail();
            log.HangfireJobId = hangfireJobId;
            log.ProfileId = profileId;
            log.ProfileRosterId = roster.Id;
            log.JobMethod = jobMethod;
            log.RunAfterMin = timeToRun;
            log.CreatedDateTimeUtc = DateTime.UtcNow;
            log.CreatedDateTimeServer = DateTime.Now;

            if (e.SurveyId != null)
            {
                log.SurveyId = e.SurveyId;
            }
            else
            {
                log.SurveyId = 0;
            }

            _unitOfWork.JobLogCompleteSurveyReminderEmailRespository.Insert(log);
            _unitOfWork.SaveChanges();
            return hangfireJobId;
        }

        private async Task<int?> StartRecurrentSurveyEmail(string jobName, string jobMethod, int profileId,
            string baseUrl, CurrentProfile profile, ProfileRosterDto roster, int? hangfireJobId, double timeToRun, NewSurvey e, Survey survey)
        {

            if (profile.ClientInitials.ToLower().ToString() == "jd")
            {
                SurveyInvitationEmailDto email = (SurveyInvitationEmailDto)GetEmailDto(profile.ClientInitials, jobName, profileId, baseUrl, string.Empty, profile, roster, timeToRun, survey);
                email.SurveyId = e.SurveyId;
                //startSurveyEmail.ScheduledDateTime = DateTime.Now.AddMinutes(timeToRun).ToString();
                hangfireJobId = await _hangfireScheduler.StartRecurrentSurveyEmail(email, timeToRun);
            }
            else if (profile.ClientInitials.ToLower().ToString() == "wam")
            {
                StartWarrenSurveyInvitationEmailDto email = (StartWarrenSurveyInvitationEmailDto)GetEmailDto(profile.ClientInitials, jobName, profileId, baseUrl, string.Empty, profile, roster, timeToRun, survey);
                email.SurveyId = e.SurveyId;
                hangfireJobId = await _hangfireScheduler.StartWarrenSurveyInvitationEmail(email, timeToRun);
            }
            JobLogStartSurveyReminderEmail log = new JobLogStartSurveyReminderEmail();
            log.HangfireJobId = hangfireJobId;
            log.ProfileId = profileId;
            log.ProfileRosterId = roster.Id;
            log.JobMethod = jobMethod;
            log.RunAfterMin = timeToRun;
            log.CreatedDateTimeUtc = DateTime.UtcNow;
            log.CreatedDateTimeServer = DateTime.Now;

            if (e.SurveyId != null)
            {
                log.SurveyId = e.SurveyId;
            }
            else
            {
                log.SurveyId = 0;
            }

            _unitOfWork.JobLogStartSurveyReminderEmailRespository.Insert(log);
            _unitOfWork.SaveChanges();
            return hangfireJobId;
        }

        public async Task<int?> CreateJobAsync(string jobName, string jobType, int profileId, string jobMethod,
            string baseUrl, string link, CurrentProfile profile = null, ProfileRosterDto roster = null, Survey recurrentSurvey = null)
        {
            //Deal with both hangfire schedule and email repository
            //If job is an email and with no schedule, log job and then log email, and send
            //If job is an email with schedule, log job, log email, schedule job, get jobId and update with to emaillog
            //JobName = SendEmail, JobMethod = Auto,Manual, jobType = Email, TriggerMethod
            int? hangfireJobId = null;
            hangfireJobId = await CreateHangfireJob(jobName, jobType, jobMethod, profileId,
                baseUrl, profile, roster, recurrentSurvey);
            return null;
        }



        #region Remove JobLogs with Scheduled jobs

        public async Task RemoveShiftReminderJobs(int id)
        {
            var shiftReminderJobs = _unitOfWork.JobLogShiftReminderEmailRespository
                                .Get(x => x.ProfileRosterId == id);


            foreach (var x in shiftReminderJobs)
            {
                if (x.HangfireJobId.HasValue)
                {
                    await _hangfireScheduler.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                }
            }
            _unitOfWork.JobLogShiftReminderEmailRespository.RemoveRange(shiftReminderJobs);
            //_unitOfWork.SaveChanges();
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task RemoveCreateSurveyJobs(int id)
        {

            var createSurveyJobs = _unitOfWork.JobLogCreateSurveyRespository
                    .Get(x => x.ProfileRosterId == id);

            foreach (var x in createSurveyJobs)
            {
                if (x.HangfireJobId.HasValue)
                {
                    await _hangfireScheduler.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                }
            }
            _unitOfWork.JobLogCreateSurveyRespository.RemoveRange(createSurveyJobs);
            //_unitOfWork.SaveChanges();
            await _unitOfWork.SaveChangesAsync();

        }



        public async Task RemoveStartSurveyReminders(int id)
        {
            var startSurveyReminders = _unitOfWork.JobLogStartSurveyReminderEmailRespository
                  .Get(x => x.ProfileRosterId == id);

            foreach (var x in startSurveyReminders)
            {
                if (x.HangfireJobId.HasValue)
                {
                    await _hangfireScheduler.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                }
            }
            _unitOfWork.JobLogStartSurveyReminderEmailRespository.RemoveRange(startSurveyReminders);
            //_unitOfWork.SaveChanges();
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task RemoveCompleteSurveyReminders(int id)
        {
            var completeSurveyReminders = _unitOfWork.JobLogCompleteSurveyReminderEmailRespository
                 .Get(x => x.ProfileRosterId == id);

            foreach (var x in completeSurveyReminders)
            {
                if (x.HangfireJobId.HasValue)
                {
                    await _hangfireScheduler.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                }
            }
            _unitOfWork.JobLogCompleteSurveyReminderEmailRespository.RemoveRange(completeSurveyReminders);
            //_unitOfWork.SaveChanges();
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task RemoveExpiringSoonNotStartedReminder(int id)
        {
            var expiringSoonNotStartedReminders = _unitOfWork.JobLogExpiringSoonSurveyNotStartedReminderEmailRespository
              .Get(x => x.ProfileRosterId == id);

            foreach (var x in expiringSoonNotStartedReminders)
            {
                if (x.HangfireJobId.HasValue)
                {
                    await _hangfireScheduler.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                }
            }
            _unitOfWork.JobLogExpiringSoonSurveyNotStartedReminderEmailRespository.RemoveRange(expiringSoonNotStartedReminders);
            //_unitOfWork.SaveChanges();
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task RemoveExpiringSoonNotCompletedReminders(int id)
        {
            var expiringSoonNotCompletedReminders = _unitOfWork.JobLogExpiringSoonSurveyNotCompletedReminderEmailRespository
                     .Get(x => x.ProfileRosterId == id);

            foreach (var x in expiringSoonNotCompletedReminders)
            {
                if (x.HangfireJobId.HasValue)
                {
                    await _hangfireScheduler.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                }
            }
            _unitOfWork.JobLogExpiringSoonSurveyNotCompletedReminderEmailRespository.RemoveRange(expiringSoonNotCompletedReminders);
            //_unitOfWork.SaveChanges();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveUpdateSurveyJob(int id)
        {
            var updateSurveyJobs = _unitOfWork.JobLogUpdateSurveyRespository
                     .Get(x => x.ProfileRosterId == id);

            foreach (var x in updateSurveyJobs)
            {
                if (x.HangfireJobId.HasValue)
                {
                    await _hangfireScheduler.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                }
            }
            _unitOfWork.JobLogUpdateSurveyRespository.RemoveRange(updateSurveyJobs);
            //_unitOfWork.SaveChanges();
            await _unitOfWork.SaveChangesAsync();

        }



        #endregion

        public void Dispose()
        {
            _hangfireScheduler.Dispose();
            profileService.Dispose();

            _unitOfWork.Dispose();
        }

    }
}