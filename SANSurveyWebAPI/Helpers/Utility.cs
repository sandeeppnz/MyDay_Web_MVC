using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SANSurveyWebAPI
{

    public static class EnumUtil
    {
        public static T ParseEnum<T>(string value)
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }
    }
    public static class Constants
    {
        public const int SURVEY_WINDOW_IN_HRS = 4;
        public const int SURVEY_EXPIRY_AFTER_HRS = 24;
        public const int SURVEY_EXPIRY_AFTER_MIN = 3; //for testing
        public const int REMINDER_AFTER_SHIFT_END_HRS = 2;
        public const int REMINDER_BEFORE_EXPIRY_HRS = 5;
        public const int ROSTER_EMPTY_PERIOD_TILL_DAYS = 14;
        public const int REMINDER_SHIFT_START_BEFORE_HRS = 12;
        public const int PAGES_PER_TASK_VARIANT = 2;
        public const int PAGE_ONE_PROGRESS_VARIANT = 10;
        public const decimal TASK_COMPLETION_PERCENTAGE_VARIANT = 80M;
        public const int PAGE_ONE_PROGRESS_ORIGINAL = 0;


        #region application setup
        public const string AppFooter = "MyDay. All Rights Reserved.";
        public const string AppHeader = "MyDay";
        public const string AppBrandName = "MyDay\u2122";
        public const string AppName = "MyDay";
        public const string AdminEmail = "surveymyday@gmail.com";
        //public const string AdminEmail = "myday@aut.ac.nz";
        public const string WebsiteEmail = "myday@aut.ac.nz";
        public const string AdminName = "Admin";
        public const string GlobalErrorNotficationEmail = "surveymyday@gmail.com";
        //public const string GlobalErrorNotficationEmail = "myday@aut.ac.nz";
        private static string baseUri = HttpContext.Current.Request.Url.Authority;
        private static string loginUrl = "http://" + baseUri + "/Account/Login";
        #endregion
        #region Noitificaiton Related
        public const string Notification_NewSurvey = "New survey";
        public const string Notification_IncompleteSurvey = "Incomplete Survey";
        public const string Notification_ExpiringSurvey = "Expiring Soon Survey";

        public const string Notification_UpdateRoster = "Please update your roster";
        #endregion
        #region Error Messages
        public const string ErrorLoginMsg = "Login attempt failed. Please try again.";
        public const string ErrorLoginMsgMainSite = "Login failed, try again.";
        //public const string ErrorSignupMsg = "We are unable to find your email address in our invitation list. <br /><br />Please ensure the email address entered is correct and it is the same email address which you received the invitation.";
        public const string ErrorSignupMsg = "Please ensure the email address you have entered is the email you have registered with HEE Wessex. <br><br/>If you have any difficulty please contact <a href='"+ "mailto:myday@aut.ac.nz'"+ ">myday@aut.ac.nz</a> quoting the unique ID from the top of your email invitation.";
        public const string ErrorSignupEmailExistsMsg = "Sorry this email address has been used previously to create an account. <br /><br />Please check your entry or please contact us.";
        public readonly static string ErrorRegistrationAlreadyCompletedMsg = "Your registration has been completed already. <br /><br /> Please click to <a href='" + loginUrl + "'>login</a>";
        public readonly static string InvalidSurveyMsg = "Unable to find the survey";
        #endregion
        #region survey related
        public const string NA_7Rating = "7";
        public const string NA_5Rating = "6";

        public const string QDisplay = "Time Spent";
        public const string QDisplayShort = "Time Spent";
        public const string QDB = "Q01";


        public const string Q1Display = "Impatient for it to end ";
        public const string Q1DisplayShort = "Impatient";
        public const string Q1DB = "Q02";


        public const string Q2Display = "Happy ";
        public const string Q2DisplayShort = "Happy";
        public const string Q2DB = "Q03";

        public const string Q3Display = "Frustrated/ annoyed ";
        public const string Q3DisplayShort = "Frustrated";
        public const string Q3DB = "Q04";


        public const string Q4Display = "Competent/ capable ";
        public const string Q4DisplayShort = "Competent";
        public const string Q4DB = "Q05";

        public const string Q5Display = "Hassled/ pushed around ";
        public const string Q5DisplayShort = "Hassled";
        public const string Q5DB = "Q06";

        public const string Q6Display = "Warm/ friendly ";
        public const string Q6DisplayShort = "Friendly ";
        public const string Q6DB = "Q07";

        public const string Q7Display = "Worried/ anxious ";
        public const string Q7DisplayShort = "Worried";
        public const string Q7DB = "Q08";

        public const string Q8Display = "Enjoying myself ";
        public const string Q8DisplayShort = "Enjoying";
        public const string Q8DB = "Q09";

        public const string Q9Display = "Criticised / put down";
        public const string Q9DisplayShort = "Criticised";
        public const string Q9DB = "Q10";

        public const string Q10Display = "Tired ";
        public const string Q10DisplayShort = "Tired";
        public const string Q10DB = "Q11";

        //For Kids
        public const string Q12Display = "Safe";
        public const string Q12DisplayShort = "Safe";
        public const string Q12DB = "Q12";

        #endregion


        public static string GetFullyMaskedText(string s)
        {
            //return Regex.Replace(s, @"[A-Z]", "*");
            return new string('*', s.Length);

        }

        public static string GetEmailMaskedText(string s)
        {
            //return Regex.Replace(s, @"[A-Z]", "*");
            return Regex.Replace(s, @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{1}@)", m => new string('*', m.Length));
        }


        //Enums
        public enum JobName
        {
            [Display(Name = "ShiftReminderEmail")]
            ShiftReminderEmail = 1,

            [Display(Name = "CreateSurveyJob")]
            CreateSurveyJob = 2,

            [Display(Name = "RegisterBaselineSurveyEmail")]
            RegisterBaselineSurveyEmail = 3,

            [Display(Name = "RegistrationCompletedEmail")]
            RegistrationCompletedEmail = 4,

            [Display(Name = "StartRecurrentSurveyEmail")]
            StartRecurrentSurveyEmail = 5,

            [Display(Name = "CompleteRecurrentSurveyEmail")]
            CompleteRecurrentSurveyEmail = 6,

            [Display(Name = "ExpiringSoonRecurrentSurveyNotStartedEmail")]
            ExpiringSoonRecurrentSurveyNotStartedEmail = 7,

            [Display(Name = "ExpiringSoonRecurrentSurveyNotCompletedEmail")]
            ExpiringSoonRecurrentSurveyNotCompletedEmail = 8,

            [Display(Name = "ExitSurveyEmail")]
            ExitSurveyEmail = 9,

            [Display(Name = "WarrenMahonySignupCompletedEmail")]
            WarrenMahonySignupCompletedEmail = 10,

            //WarrenMahonySignupCompletedEmailDto


        }

        public enum JobMethod
        {
            [Display(Name = "Manual")]
            Manual = 1,

            [Display(Name = "Auto")]
            Auto = 2,
        }

        public enum JobType
        {
            [Display(Name = "Email")]
            Email = 1,

            [Display(Name = "Method")]
            Method = 2,
        }


        public enum PageType
        {
            [Display(Name = "ENTER")]
            Enter = 1,

            [Display(Name = "EXIT")]
            Exit = 2,

            [Display(Name = "ERROR")]
            ERROR = 3,

            [Display(Name = "SURVEY_NOT_FOUND")]
            SURVEY_NOT_FOUND = 4,

            [Display(Name = "LOST_SESSION")]
            SESSION_ERROR = 5,

        }

        public enum PageAction
        {
            [Display(Name = "Get")]
            Get = 1,

            [Display(Name = "Post")]
            Post = 2,
        }


        public enum SurveyStatus
        {
            [Display(Name = "Completed")]
            Completed = 1,

            [Display(Name = "Expired")]
            Expired = 2,

            [Display(Name = "Expiring_Soon")]
            Expiring_soon = 3,
        }


        public enum PageName
        {


            [Display(Name = "Survey_Original_ViewResponse")]
            Survey_Original_ViewResponse = 970,



            [Display(Name = "Survey_Original_Refresh")]
            Survey_Original_Refresh = 999,


            [Display(Name = "Survey_Original_Resume")]
            Survey_Original_Resume = 998,

            [Display(Name = "Survey_Original_Expired")]
            Survey_Original_Expired = 997,


            [Display(Name = "Survey_Original_New")]
            Survey_Original_New = 996,



            [Display(Name = "Survey_Original_Completed_Already")]
            Survey_Original_Completed_Already = 996,



            [Display(Name = "Survey_Original_Start")]
            Survey_Original_Start = 20,

            [Display(Name = "Survey_Original_Shift")]
            Survey_Original_Shift = 21,

            [Display(Name = "Survey_Original_Task")]
            Survey_Original_Task = 23,

            [Display(Name = "Survey_Original_Time")]
            Survey_Original_Time = 25,

            [Display(Name = "Survey_Original_Rating1")]
            Survey_Original_Rating1 = 27,

            [Display(Name = "Survey_Original_Rating2")]
            Survey_Original_Rating2 = 29,

            [Display(Name = "Survey_Original_Summary")]
            Survey_Original_Summary = 31,

            [Display(Name = "Survey_Original_EditResponse")]
            Survey_Original_EditResponse = 33,


            [Display(Name = "Registration_Demographics")]
            Registration_Demographics = 1,

            [Display(Name = "Registration_Work")]
            Registration_Screening = 2,

            [Display(Name = "Registration_Wellbeing")]
            Registration_Wellbeing = 3,

            [Display(Name = "Registration_Task")]
            Registration_Task = 4,

            [Display(Name = "Registration_Roster")]
            Registration_Roster = 5,

            [Display(Name = "Registration_Completed")]
            Registration_Completed = 6,


            [Display(Name = "Registration_Consent")]
            Registration_Consent = 7,

            [Display(Name = "Registration_Signup")]
            Registration_Signup = 8,


            [Display(Name = "Login")]
            Login = 9,

            [Display(Name = "Registration_TaskTime")]
            Registration_TaskTime = 10,

            [Display(Name = "Registration_Placement")]
            Registration_Placement = 11,


            [Display(Name = "Registration_Contract")]
            Registration_Contract = 12,

            [Display(Name = "Registration_Specialty")]
            Registration_Specialty = 13,

            [Display(Name = "Registration_Training")]
            Registration_Training = 14,


            [Display(Name = "UserHome_Index")]
            UserHome_Index = 101,


            [Display(Name = "ExitSurvey_Page1")]
            ExitSurvey_Page1 = 501,

            [Display(Name = "ExitSurvey_Page2")]
            ExitSurvey_Page2 = 502,

            [Display(Name = "ExitSurvey_Page3")]
            ExitSurvey_Page3 = 503,

            [Display(Name = "ExitSurvey_Page4")]
            ExitSurvey_Page4 = 504,

            [Display(Name = "ExitSurvey_Page5")]
            ExitSurvey_Page5 = 505,

            [Display(Name = "ExitSurvey_Page6")]
            ExitSurvey_Page6 = 506,

            [Display(Name = "ExitSurvey_Page7")]
            ExitSurvey_Page7 = 507,

            [Display(Name = "ExitSurvey_Page8")]
            ExitSurvey_Page8 = 508,


            [Display(Name = "ExitSurvey_Page9")]
            ExitSurvey_Page9 = 509,

            [Display(Name = "ExitSurvey_Page10")]
            ExitSurvey_Page10 = 510,


            [Display(Name = "ExitSurvey_Page11")]
            ExitSurvey_Page11 = 511,

            [Display(Name = "ExitSurvey_Page12")]
            ExitSurvey_Page12 = 512,

            [Display(Name = "ExitSurvey_Page13")]
            ExitSurvey_Page13 = 513,

            [Display(Name = "ExitSurvey_Page14")]
            ExitSurvey_Page14 = 514,

            [Display(Name = "ExitSurvey_PageContinued5")]
            ExitSurvey_PageContinued5 = 515,

            [Display(Name = "ExitSurvey_PageContinued12")]
            ExitSurvey_PageContinued12 = 516,

            [Display(Name = "Feedback")]
            Feedback = 517,
        }



        public enum StatusExitSurveyProgress
        {
            [Display(Name = "Page1")]
            Page1 = 1,

            [Display(Name = "Page2")]
            Page2 = 2,

            [Display(Name = "Page3")]
            Page3 = 3,

            [Display(Name = "Page4")]
            Page4 = 4,

            [Display(Name = "Page5")]
            Page5 = 5,

            [Display(Name = "PageContinued5")]
            PageContinued5 = 6,

            [Display(Name = "Page6")]
            Page6 = 7,

            [Display(Name = "Page7")]
            Page7 = 8,

            [Display(Name = "Page8")]
            Page8 = 9,

            [Display(Name = "Page9")]
            Page9 = 10,

            [Display(Name = "Page10")]
            Page10 = 11,

            [Display(Name = "Page11")]
            Page11 = 12,

            [Display(Name = "Page12")]
            Page12 = 13,

            [Display(Name = "PageContinued12")]
            PageContinued12 = 14,

            [Display(Name = "Page13")]
            Page13 = 15,

            [Display(Name = "Page14")]
            Page14 = 16,

            [Display(Name = "Page15")]
            Page15 = 17,

            [Display(Name = "Completed")]
            Completed = 18,
        }



        public enum StatusSurveyProgress
        {
            [Display(Name = "New")]
            New = 1,

            [Display(Name = "Invited")]
            Invited = 2,

            [Display(Name = "ShiftTime")]
            ShiftTime = 3,

            [Display(Name = "TaskTime")]
            TaskTime = 4,

            [Display(Name = "TaskRating1")]
            TaskRating1 = 5,

            [Display(Name = "TaskRating2")]
            TaskRating2 = 6,

            [Display(Name = "Completed")]
            Completed = 7,

            [Display(Name = "Tasks")]
            Tasks = 8,

            [Display(Name = "AddShiftTime")]
            AddShiftTime = 9,

            [Display(Name = "AddTasks")]
            AddTasks = 10,

            [Display(Name = "AddTaskTime")]
            AddTaskTime = 11,

            [Display(Name = "AddTaskRating1")]
            AddTaskRating1 = 12,

            [Display(Name = "AddTaskRating2")]
            AddTaskRating2 = 13,


            [Display(Name = "WRTaskTime")]
            WRTaskTime = 14,
            [Display(Name = "WRTasks")]
            WRTasks = 15,
            [Display(Name = "WRTaskRating1")]
            WRTaskRating1 = 16,
            [Display(Name = "WRTaskRating2")]
            WRTaskRating2 = 17,
            [Display(Name = "WRTaskTimeInd")]
            WRTaskTimeInd = 18,

            [Display(Name = "WAMTasks")]
            WAMTasks = 19,
            [Display(Name = "WAMTaskTime")]
            WAMTaskTime = 20,
            [Display(Name = "WAMTaskRating1")]
            WAMTaskRating1 = 21,
            [Display(Name = "WAMTaskRating2")]
            WAMTaskRating2 = 22,
            [Display(Name = "WAMAddShiftTime")]
            WAMAddShiftTime = 23,
            [Display(Name = "WAMAddTaskTime")]
            WAMAddTaskTime = 24,
            [Display(Name = "WAMAddTasks")]
            WAMAddTasks = 25,
            [Display(Name = "WAMAddTaskRating1")]
            WAMAddTaskRating1 = 26,
            [Display(Name = "WAMAddTaskRating2")]
            WAMAddTaskRating2 = 27,
           
        }

        public enum CaseWorkersSurveyProgress
        {
            [Display(Name = "New")]
            New = 1,

            [Display(Name = "Invited")]
            Invited = 2,

            [Display(Name = "ShiftTime")]
            ShiftTime = 3,

            [Display(Name = "TaskTime")]
            TaskTime = 4,

            [Display(Name = "TaskRating1")]
            TaskRating1 = 5,

            [Display(Name = "TaskRating2")]
            TaskRating2 = 6,

            [Display(Name = "Completed")]
            Completed = 7,

            [Display(Name = "Tasks")]
            Tasks = 8,

            [Display(Name = "AddShiftTime")]
            AddShiftTime = 9,

            [Display(Name = "AddTasks")]
            AddTasks = 10,

            [Display(Name = "AddTaskTime")]
            AddTaskTime = 11,

            [Display(Name = "AddTaskRating1")]
            AddTaskRating1 = 12,

            [Display(Name = "AddTaskRating2")]
            AddTaskRating2 = 13,
        }
        public enum StatusSurvey
        {
            [Display(Name = "Active")]
            Active = 1,

            [Display(Name = "Deactivated")]
            Deactivated = 2,

            [Display(Name = "Expired")]
            Expired = 3,

        }

        public enum StatusProfile
        {
            [Display(Name = "Active")]
            Active = 1,

            [Display(Name = "Deactivated")]
            Deactivated = 2,
        }
        public enum StatusRegistrationProgress
        {
            [Display(Name = "Screening")]
            Screening = 2,

            [Display(Name = "Wellbeing")]
            Wellbeing = 3,


            [Display(Name = "Task")]
            Task = 4,

            [Display(Name = "Demographics")]
            Demographics = 1,


            [Display(Name = "Roster")]
            Roster = 5,

            [Display(Name = "Completed")]
            Completed = 6,

            [Display(Name = "Unparticipate")]
            DNP = 7,

            [Display(Name = "Signup")]
            Signup = 8,

            [Display(Name = "Invited")]
            Invited = 9,

            [Display(Name = "New")]
            New = 10,

            [Display(Name = "ScreenedOut")]
            ScreenedOut = 11,

            [Display(Name = "TaskTime")]
            TaskTime = 12,

            [Display(Name = "Placement")]
            Placement = 13,


            [Display(Name = "Contract")]
            Contract = 14,

            [Display(Name = "Specialty")]
            Specialty = 15,

            [Display(Name = "Training")]
            Training = 16,

            #region WAMDemo

            [Display(Name = "WAMWellbeing")]
            WAMWellbeing = 18,
            [Display(Name = "WAMProfileRole")]
            WAMProfileRole = 19,
            [Display(Name = "WAMTasks")]
            WAMTasks = 20,
            [Display(Name = "WAMDemographics")]
            WAMDemographics = 21,
            [Display(Name = "WAMIntentions")]
            WAMIntentions = 22,
            [Display(Name = "WAMRoster")]
            WAMRoster = 23,

            #endregion

        }       

        public enum ApplicationRole
        {
            Admin = 1,
            Researcher = 2,
            Support = 3,
            Registered_User = 4,
        }





        public static string GetInt5ScaleRating(string text)
        {
            switch (text)
            {
                case "Strongly Dissatisfied":
                    return "1";
                    break;
                case "Dissatisfied":
                    return "2";
                    break;
                case "Neutral":
                    return "3";
                    break;
                case "Satisifed":
                    return "4";
                    break;
                case "Strongly Satisifed":
                    return "5";
                case "Very Satisifed": //added on 2nd August 2017 --by Bharati
                    return "5";                
                default:
                    return NA_5Rating;
            }

        }
        public static string GetInt5LikelyScaleRating(string text)
        {
            switch (text)
            {
                case "Very Unlikely":
                    return "1";
                    break;
                case "Unlikely":
                    return "2";
                    break;
                case "Neutral":
                    return "3";
                    break;
                case "Likely":
                    return "4";
                    break;
                case "Very Likely":
                    return "5";                
                default:
                    return NA_5Rating;
            }

        }

        public static string GetText5ScaleRating(string val)
        {
            switch (val)
            {
                case "1":
                    return "Strongly Dissatisfied";
                    break;
                case "2":
                    return "Dissatisfied";
                    break;
                case "3":
                    return "Neutral";
                    break;
                case "4":
                    return "Satisifed";
                    break;
                case "5":
                    return "Strongly Satisifed";
                default:
                    return null;
            }


        }
        public static string GetText5LikelyScaleRating(string val)
        {
            switch (val)
            {
                case "1":
                    return "Very Unlikely";
                    break;
                case "2":
                    return "Unlikely";
                    break;
                case "3":
                    return "Neutral";
                    break;
                case "4":
                    return "Likely";
                    break;
                case "5":
                    return "VeryLikely";
                default:
                    return null;
            }


        }

        public static string GetText5ScaleRatingExitSurveyPage1(string val)
        {
            switch (val)
            {
                case "1":
                    return "Very Dissatisfied";
                    break;
                case "2":
                    return "Dissatisfied";
                    break;
                case "3":
                    return "Neutral";
                    break;
                case "4":
                    return "Satisifed";
                    break;
                case "5":
                    return "Very Satisifed";
                default:
                    return null;
            }


        }


        public static string GetInt7ScaleRating(string text)
        {
            switch (text.Trim())
            {
                case "Not at all":
                    return "0";
                    break;
                case "Slightly":
                    return "1";
                    break;
                case "Somewhat":
                    return "2";
                    break;
                case "Moderately":
                    return "3";
                    break;
                case "Strongly":
                    return "4";
                case "Very Strongly":
                    return "5";
                case "Extremely":
                    return "6";
                case "N/A":
                    return NA_7Rating;
                default:
                    return text;
            }

        }
        public static string GetText7ScaleRating(string val)
        {
            switch (val)
            {
                case "0":
                    return "Not at all";
                case "1":
                    return "Slightly";
                case "2":
                    return "Somewhat";
                case "3":
                    return "Moderately";
                case "4":
                    return "Strongly";
                case "5":
                    return "Very Strongly";
                case "6":
                    return "Extremely";
                case "7":
                    return "N/A";
                default:
                    return ConvertTotalMins(val);
            }


        }


        private static string ConvertTotalMins(string totalMins)
        {

            int totalM = int.Parse(totalMins);
            int h = totalM / 60;
            int m = totalM % 60;

            string hh = string.Empty;
            string mm = string.Empty;

            if (m != 0) { mm = string.Format(" {0} min", m); }

            //if ( h > 0 && (totalM >= 60 && totalM < 120) )
            if (h > 0 )
            {
                hh = string.Format("{0} hrs", h);
            }
           
            return hh + mm;
        }

        public enum KidsSurveyStatus
        {
            [Display(Name = "NewSurvey")]
            NewSurvey = 1,
            [Display(Name = "FromLocation")]
            Location = 2,
            [Display(Name = "TimeSpent")]
            TimeSpent = 3,
            [Display(Name = "ToLocation")]
            ToLocation = 4,
            [Display(Name = "Travel")]
            Travel = 5,
            [Display(Name = "LocationSummary")]
            LocationSummary = 6,
            [Display(Name = "Tasks1")]
            Tasks1 = 7,
            [Display(Name = "Tasks2")]
            Tasks2 = 8,
            [Display(Name = "TaskSummaries")]
            TaskSummaries = 9,
            [Display(Name = "EmotionalAffectStage1")]
            EmotionalAffectStage1 = 10,
            [Display(Name = "EmotionalAffectStage2")]
            EmotionalAffectStage2 = 11,
            [Display(Name = "Feedback")]
            Feedback = 12,
        }
        public enum ProgressBarValue
        {
            [Display(Name = "Zero")]
            Zero = 0,
            [Display(Name ="Ten")]
            Ten = 10,
            [Display(Name = "Twenty")]
            Twenty = 20,
            [Display(Name = "Thirty")]
            Thirty = 30,
            [Display(Name = "Fourty")]
            Fourty = 40,
            [Display(Name = "Fifty")]
            Fifty = 50,
            [Display(Name = "Sixty")]
            Sixty = 60,
            [Display(Name = "Seventy")]
            Seventy = 70,
            [Display(Name = "Eighty")]
            Eighty = 80,
            [Display(Name = "Ninety")]
            Ninety = 90,
            [Display(Name = "Hundred")]
            Hundred = 100
        }

        public enum ExitV2Status
        {
            [Display(Name = "NewSurvey")]
            NewSurvey = 0,
            [Display(Name = "WellBeing")]
            WellBeing = 1,
            [Display(Name = "FirstJob")]
            FirstJob = 2,
            [Display(Name = "SecondJob")]
            SecondJob = 3,           
            [Display(Name = "FirstWorkEnvironment")]
            FirstWorkEnvironment = 4,
            [Display(Name = "SecondWorkEnvironment")]
            SecondWorkEnvironment = 5,
            [Display(Name = "ThirdWorkEnvironment")]
            ThirdWorkEnvironment = 6,
            [Display(Name = "FourthWorkEnvironment")]
            FourthWorkEnvironment = 7,
            [Display(Name = "FifthWorkEnvironment")]
            FifthWorkEnvironment = 8,
            [Display(Name = "FirstTraining")]
            FirstTraining = 9,
            [Display(Name = "SecondTraining")]
            SecondTraining = 10,
            [Display(Name = "ThirdTraining")]
            ThirdTraining = 11,
            [Display(Name = "AboutYou")]
            AboutYou = 12,
        }

        #region "SOCIAL WORKERS"

        public enum CaseWorkersRegistrationStatus
        {
            [Display(Name = "New")]
             New = 0,
            //[Display(Name = "SignUpSocialWorkers")]
            //SignUpSocialWorkers = 2,
            [Display(Name = "SubjectiveWellBeing")]
            SubjectiveWellBeing = 1,
            [Display(Name = "RoleAtCurrentWorkPlace")]
            RoleAtCurrentWorkPlace = 2,
            [Display(Name = "RoleAtCurrentWorkPlaceContd")]
            RoleAtCurrentWorkPlaceContd = 3,
            [Display(Name = "CaseWorkersTask")]
            CaseWorkersTask = 4,
            [Display(Name = "CaseLoad")]
            CaseLoad = 5,
            [Display(Name = "TimeAllocation")]
            TimeAllocation = 6,
            [Display(Name = "DemoGraphics")]
            DemoGraphics = 7,
            [Display(Name = "EducationBackground")]
            EducationBackground = 8,
            [Display(Name = "JobIntentions")]
            JobIntentions = 9,
            [Display(Name = "SocialWorkersRoster")]
            SocialWorkersRoster = 10,
            [Display(Name = "SocialWorkersFeedback")]
            SocialWorkersFeedback = 11,
            [Display(Name = "SocialWorkersCompletion")]
            SocialWorkersCompletion = 12
        }

        #endregion
    }
}