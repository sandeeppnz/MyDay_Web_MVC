using System.Collections.Generic;
using SANSurveyWebAPI.Models;
using System;
using System.Linq;

namespace SANSurveyWebAPI.ViewModels.Web
{
    public abstract class KidsModelBaseClass
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileEmail { get; set; }
        public int ProfileOffset { get; set; }
        public int MaxStepKidsSurvey { get; set; }
        public string ProgressNext { get; set; }
        public DateTime? LastUpdatedDateTimeUtc { get; set; }        
    }
    [Serializable]
    public class KidsProfileUpdatesVM : KidsModelBaseClass
    {

    }
    [Serializable]
    public class KidsSurveyVM
    {
        public string Uid { get; set; }
        public int ProfileId { get; set; }
        public string CurrentDate { get; set; }
    }
    [Serializable]
    public class KidsSurveyTimelineVM : ViewModelBase
    {
        public string PageQuestion { get; set; }
        public string QuestionHint { get; set; }
        public string ErrorMessage { get; set; }
        public List<KidsTasks> AllKidsTasksObj { get; set; }
        public string TaskName { get; set; } //What you did
        public string Venue { get; set; } //Where you did
        public string InOutLocation { get; set; }
        public string Travel { get; set; } //How did you reach
        public string People { get; set; }//Who you were with
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public IList<PeopleSelectionOptions> PeopleOptions { get; set; }
        public string HiddenPeopleIds { get; set; }
        public string QuestionTask { get; set; }
        public string QuestionLocation { get; set; }
        public string QuestionInOut { get; set; }
        public string QuestionTravel { get; set; }
        public string QuestionPeople { get; set; }
        public string QuestionStartTime { get; set; }
        public string QuestionEndTime { get; set; }
        public KidsSurveyTimelineVM()
        {
            PageQuestion = "You can start adding your daily activities here.";
            QuestionHint = "";
            ErrorMessage = "Please add at least one task to continue.";

            QuestionTask = "What were you doing?";
            QuestionLocation = "Where is this place?";
            QuestionInOut = "Where were you?";
            QuestionTravel = "How did you reach there?";
            QuestionPeople = "Who were you with?";
            QuestionStartTime = "What time did you do the task?";
            QuestionEndTime = "When did you end the task?";
        }
    }
    public class PeopleSelectionOptions
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string Ans { get; set; }
    }

    [Serializable]
    public class KidsResponsesOneVM : ViewModelBase
    {
        public string PageQuestion { get; set; }
        public string PageQuestionSurveyVariant { get; set; }
        public DateTime ResponseStartDateTimeUtc { get; set; }
        public DateTime ResponseEndDateTimeUtc { get; set; }
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public bool IsExist { get; set; }
        public string Q1Display { get; set; }
        public string Q1DisplayShort { get; set; }
        public string Q1DB { get; set; }
        public string Q2Display { get; set; }
        public string Q2DisplayShort { get; set; }
        public string Q2DB { get; set; }
        public string Q3Display { get; set; }
        public string Q3DisplayShort { get; set; }
        public string Q3DB { get; set; }
        public string Q4Display { get; set; }
        public string Q4DisplayShort { get; set; }
        public string Q4DB { get; set; }
        public string Q5Display { get; set; }
        public string Q5DisplayShort { get; set; }
        public string Q5DB { get; set; }
        public string Q1Ans { get; set; }
        public string Q2Ans { get; set; }
        public string Q3Ans { get; set; }
        public string Q4Ans { get; set; }
        public string Q5Ans { get; set; }
        public string DisplayPara1 { get; set; }
        public string DisplayPara2 { get; set; }
        public string NextTaskName { get; set; }
        public int TaskRound { get; set; }
        public string EAPageQuestion1 { get; set; }
        public string EAPageQuestion2 { get; set; }
        public string QuestionHint { get; set; }
        public KidsResponsesOneVM()
        {
            IsExist = false;

            Q1Display = Constants.Q1Display;
            Q1DisplayShort = Constants.Q1DisplayShort;
            Q1DB = Constants.Q1DB;

            Q2Display = Constants.Q2Display;
            Q2DisplayShort = Constants.Q2DisplayShort;
            Q2DB = Constants.Q2DB;

            Q3Display = Constants.Q3Display;
            Q3DisplayShort = Constants.Q3DisplayShort;
            Q3DB = Constants.Q3DB;

            Q4Display = Constants.Q4Display;
            Q4DisplayShort = Constants.Q4DisplayShort;
            Q4DB = Constants.Q4DB;

            Q5Display = Constants.Q5Display;
            Q5DisplayShort = Constants.Q5DisplayShort;
            Q5DB = Constants.Q5DB;

            PageQuestion = "Now, we would like to learn in more detail about how you felt during ";
            PageQuestionSurveyVariant = "Now, we would like to learn in more detail about how you felt during ";
            ResponseStartDateTimeUtc = DateTime.UtcNow;

            EAPageQuestion1 = "We would like to know how you felt, on average, during the ";
            EAPageQuestion2 = "minutes you spent on ";
            QuestionHint = "Please rate each feeling on the scale given. A rating of 0 means that you did not experience that feeling at all. A rating of 6 means that this feeling was a very important part of the experience. Please select the number between 0 and 6 that best describes how you felt.";
        }

    }

    [Serializable]
    public class KidsResponsesTwoVM : ViewModelBase
    {
        public string PageQuestion { get; set; }
        public string PageQuestionSurveyVariant { get; set; }
        public DateTime ResponseStartDateTimeUtc { get; set; }
        public DateTime ResponseEndDateTimeUtc { get; set; }
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public bool IsExist { get; set; }
        public string Q6Display { get; set; }
        public string Q6DisplayShort { get; set; }
        public string Q6DB { get; set; }
        public string Q7Display { get; set; }
        public string Q7DisplayShort { get; set; }
        public string Q7DB { get; set; }
        public string Q8Display { get; set; }
        public string Q8DisplayShort { get; set; }
        public string Q8DB { get; set; }
        public string Q9Display { get; set; }
        public string Q9DisplayShort { get; set; }
        public string Q9DB { get; set; }
        public string Q10Display { get; set; }
        public string Q10DisplayShort { get; set; }
        public string Q10DB { get; set; }
        public string Q6Ans { get; set; }
        public string Q7Ans { get; set; }
        public string Q8Ans { get; set; }
        public string Q9Ans { get; set; }
        public string Q10Ans { get; set; }
        public string EAPageQuestion1 { get; set; }
        public string EAPageQuestion2 { get; set; }
        public string QuestionHint { get; set; }
        public KidsResponsesTwoVM()
        {
            IsExist = false;

            Q6Display = Constants.Q6Display;
            Q6DisplayShort = Constants.Q6DisplayShort;
            Q6DB = Constants.Q6DB;

            Q7Display = Constants.Q7Display;
            Q7DisplayShort = Constants.Q7DisplayShort;
            Q7DB = Constants.Q7DB;

            Q8Display = Constants.Q8Display;
            Q8DisplayShort = Constants.Q8DisplayShort;
            Q8DB = Constants.Q8DB;

            Q9Display = Constants.Q9Display;
            Q9DisplayShort = Constants.Q9DisplayShort;
            Q9DB = Constants.Q9DB;

            Q10Display = Constants.Q10Display;
            Q10DisplayShort = Constants.Q10DisplayShort;
            Q10DB = Constants.Q10DB;

            PageQuestion = "Now, we would like to learn in more detail about how you felt during ";
            PageQuestionSurveyVariant = "Now, we would like to learn in more detail about how you felt during ";
            ResponseStartDateTimeUtc = DateTime.UtcNow;

            EAPageQuestion1 = "We would like to know how you felt, on average, during the ";
            EAPageQuestion2 = "minutes you spent on ";
            QuestionHint = "Please rate each feeling on the scale given. A rating of 0 means that you did not experience that feeling at all. A rating of 6 means that this feeling was a very important part of the experience. Please select the number between 0 and 6 that best describes how you felt.";
        }
    }

    [Serializable]
    public class KidsFeedbackVM : ViewModelBase
    {
        public string Comment { get; set; }
    }
    public class KidsResponsesVM
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string TaskDuration { get; set; }
        public string TaskTimeSpan { get; set; }
        public string TaskDescription { get; set; }
        public DateTime TaskStartDateTime { get; set; }
        public string TaskStartDate { get; set; }
        public string TaskStartTime { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string RatingString { get; set; }

    }
    public class KidsSummaryVM : ViewModelBase
    {
        public Lookup<string, KidsResponsesVM> FullResponseList { get; set; }

        public string Comment;

        public string LocationName { get; set; }
        public string TaskPerformed { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

    }
    [Serializable]
    public class KidsSurveyTaskDurationVM : ViewModelBase
    {
        public List<KidsTasks> kidsTasksObj { get; set; }
    }
    [Serializable]
    public class KidsEditResponseVM : ViewModelBase
    {
        public string PageQuestion { get; set; }
        public string PageQuestionSurveyVariant { get; set; }
        public DateTime PageStartDateTimeUtc { get; set; }
        public DateTime PageEndTime { get; set; }
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public DateTime TaskStartDateTime { get; set; }
        public bool IsExist { get; set; }
        public string Q1Display { get; set; }
        public string Q1DB { get; set; }
        public string Q2Display { get; set; }
        public string Q2DB { get; set; }
        public string Q3Display { get; set; }
        public string Q3DB { get; set; }
        public string Q4Display { get; set; }
        public string Q4DB { get; set; }
        public string Q5Display { get; set; }
        public string Q5DB { get; set; }
        public string Q6Display { get; set; }
        public string Q6DB { get; set; }
        public string Q7Display { get; set; }
        public string Q7DB { get; set; }
        public string Q8Display { get; set; }
        public string Q8DB { get; set; }
        public string Q9Display { get; set; }
        public string Q9DB { get; set; }
        public string Q10Display { get; set; }
        public string Q10DB { get; set; }
        public string Q1Ans { get; set; }
        public string Q2Ans { get; set; }
        public string Q3Ans { get; set; }
        public string Q4Ans { get; set; }
        public string Q5Ans { get; set; }
        public string Q6Ans { get; set; }
        public string Q7Ans { get; set; }
        public string Q8Ans { get; set; }
        public string Q9Ans { get; set; }
        public string Q10Ans { get; set; }

        //http://stackoverflow.com/questions/14473321/generating-random-unique-values-c-sharp
        //http://codereview.stackexchange.com/questions/61338/generate-random-numbers-without-repetitions     

        public KidsEditResponseVM()
        {
            IsExist = false;
            Q1Display = Constants.Q1Display;
            Q1DB = Constants.Q1DB;
            Q2Display = Constants.Q2Display;
            Q2DB = Constants.Q2DB;
            Q3Display = Constants.Q3Display;
            Q3DB = Constants.Q3DB;
            Q4Display = Constants.Q4Display;
            Q4DB = Constants.Q4DB;
            Q5Display = Constants.Q5Display;
            Q5DB = Constants.Q5DB;
            Q6Display = Constants.Q6Display;
            Q6DB = Constants.Q6DB;
            Q7Display = Constants.Q7Display;
            Q7DB = Constants.Q7DB;
            Q8Display = Constants.Q8Display;
            Q8DB = Constants.Q8DB;
            Q9Display = Constants.Q9Display;
            Q9DB = Constants.Q9DB;
            Q10Display = Constants.Q10Display;
            Q10DB = Constants.Q10DB;

            PageQuestion = "Edit your response on how you felt during ";
            PageQuestionSurveyVariant = "Edit your response on how you felt during ";
            PageStartDateTimeUtc = DateTime.UtcNow;
        }
    }
    [Serializable]
    public class KidsTaskTimeVM : ViewModelBase
    {
        public string PageQuestion { get; set; }
        public string QuestionHint { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime SurveyForDay { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public KidsTaskTimeVM()
        {
            PageQuestion = "Total Task Time ";
            ErrorMessage = "Please select at least one option to proceed.";
        }
    }
    [Serializable]
    public class KidsTaskLocationVM : ViewModelBase
    {
        public string PageQuestion { get; set; }
        public string QuestionHint { get; set; }
        public string ErrorMessage { get; set; }
        public string TaskLocation { get; set; }
        public string WhereAreYou { get; set; }
        public int TotalHours { get; set; }
        public int TotalMins { get; set; }
        public IList<IdValueOptions> YesNoOptionList { get; set; } //For: In the Neighbourhood 
        public IList<IdValueOptions> InOutOptionList { get; set; } //For: Inside or Outside
        public IList<IdValueOptions> TravelOptionList { get; set; } //For: Travel details
        public KidsTaskLocationVM()
        {
            PageQuestion = "Task Location ";
            ErrorMessage = "Please select at least one option to proceed.";
        }
    }
    public class IdValueOptions
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string LongName { get; set; }
    }
    [Serializable]
    public class KidsLocationVM : KidsModelBaseClass
    {
        public string PageQuestion { get; set; }
        public string QuestionHint { get; set; }
        public string ErrorMessage { get; set; }
        public string selectedLocation { get; set; }
        public string OtherLocation { get; set; }
        public int TimeSpentInHours { get; set; }
        public int TimeSpentInMins { get; set; }
        public IList<IdValueOptions> LocationOptionList { get; set; }
        public KidsLocationVM()
        {
            //PageQuestion = "Thinking back to 3pm, where were you?";
            ErrorMessage = "Please select the location to proceed further.";
        }
    }
    [Serializable]
    public class KidsNextLocationVM : ViewModelBase
    {
        public string PageQuestion { get; set; }
        public string QuestionHint { get; set; }
        public string ErrorMessage { get; set; }
        public string selectedLocation { get; set; }
        public string OtherLocation { get; set; }
        public int TimeSpentInHours { get; set; }
        public int TimeSpentInMins { get; set; }
        public IList<IdValueOptions> LocationOptionList { get; set; }
        public KidsNextLocationVM()
        {
            PageQuestion = "At 4pm, where did you go next?";
        }
    }
    [Serializable]
    public class KidsTimeSpentVM : KidsModelBaseClass
    {
        public string PageQuestion { get; set; }
        public string OtherHalfQuestion { get; set; }
        public string QuestionHint { get; set; }
        public string ErrorMessage { get; set; }
        public string Location { get; set; }
        public string OtherLocation { get; set; }
        public int TotalHours { get; set; }
        public int TotalMins { get; set; }
        public KidsTimeSpentVM()
        {
            PageQuestion = "How long were you at a ";
            OtherHalfQuestion = " after 3pm?";
            ErrorMessage = "Please select the time spent on this location to proceed further.";
        }
    }
    [Serializable]
    public class KidsTravelVM : ViewModelBase
    {
        public string PageQuestion1 { get; set; }
        public string PageQuestion2 { get; set; }
        public string ToInString { get; set; }
        public string QuestionMark { get; set; }
        public string QuestionHint { get; set; }
        public string MOTErrorMessage { get; set; }
        public string TimeSpentErrorMessage { get; set; }
        public string FromLocation { get; set; }
        public string OtherFromLocation { get; set; }
        public int HiddFromLocationId { get; set; }
        public string ToLocation { get; set; }
        public string OtherToLocation { get; set; }
        public int HiddToLocationId { get; set; }
        public IList<IdValueOptions> TransportOptionList { get; set; }
        public string HiddenTransport { get; set; }
        public string OtherTransport { get; set; }
        public int TravelInHours { get; set; }
        public int TravelInMins { get; set; }

        public KidsTravelVM()
        {
            PageQuestion1 = "How did you get from ";
            ToInString = "to";
            QuestionMark = "?";
            PageQuestion2 = "How long did it take to get from a ";
            MOTErrorMessage = "Please select the mode of transport to proceed further.";
            TimeSpentErrorMessage = "Please select the time spent during travel to proceed further.";
        }
    }
    [Serializable]
    public class KidsLocationSummaryVM : KidsModelBaseClass
    {
        //public List<KidsLocationAndTravel> AllKidsLocationListObj { get; set; }
        public List<KidsLocation> AllKidsLocationListObj { get; set; }
        public List<KidsTravel> AllKidsTravelListObj { get; set; }
        public List<KidsTasksOnLocation> AllKidsTasksOnLocationListObj { get; set; }
        public IList<TasksByLocation> AllKidsTasksByLocationObj { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string SurveyDate { get; set; }
        public KidsLocationSummaryVM()
        { }
    }
    public class TasksByLocation
    {
        public int TaskLocId { get; set; }
        public string TaskName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
    }
    public class KidsLocationAndTravel
    {
        public int KidsLocationId { get; set; }
        public int ProfileId { get; set; }
        public int KidsSurveyId { get; set; }
        public string Location { get; set; }
        public string OtherLocation { get; set; }
        public int TimeSpentInHours { get; set; }
        public int TimeSpentInMins { get; set; }
        public int LocationSequence { get; set; }
        public string StartedAt { get; set; } // Time at the location
        public string EndedAt { get; set; } //was on the same location till how long

        public int KidsTravelId { get; set; }
        public int FromLocationId { get; set; }
        public int ToLocationId { get; set; }
        public string ModeOfTransport { get; set; }
        public int TravelTimeInHours { get; set; }
        public int TravelTimeInMins { get; set; }
    }
    [Serializable]
    public class KidsTasksLocationVM : KidsModelBaseClass
    {
        public string PageQ1 { get; set; }
        public string QBetween { get; set; }
        public string QAnd { get; set; }
        public string PageQ2 { get; set; }
        public string QuestionHint { get; set; }
        public string ErrorMessage { get; set; }
        public int QLocationId { get; set; }
        public string QLocation { get; set; }
        public string QOtherLocation { get; set; }
        public string SelectedTasks { get; set; }
        public string OtherTasks { get; set; }
        public string SpentStartTime { get; set; }
        public string SpentEndTime { get; set; }
        public IList<IdValueOptions> TaskOptionList { get; set; }
        public KidsTasksLocationVM()
        {
            PageQ1 = "When you were at a ";
            QBetween = "between ";
            QAnd = "and ";
            PageQ2 = ", what did you do?";
            QuestionHint = "Select all the activities/tasks that you did during this time.";
            ErrorMessage = "Please select the tasks performed on this location during the specified time span.";
        }
    }
    [Serializable]
    public class KidsReactOVM : ViewModelBase
    {
        public string PageQuestion { get; set; }
        public string QTasks { get; set; }
        public string QLocation { get; set; }
        public string QStartedAt { get; set; }
        public string QEndedAt { get; set; }
        public string PageQContd { get; set; }

        public IList<KidsEmoStageTracked> KidsEMoTrackListObj { get; set; }
        
        public int? KidsLocationId { get; set; }
        public int? KidsTravelId { get; set; }
        public string SurveyDate { get; set; }
        public int? TaskId { get; set; }
        public string TaskName { get; set; }

        public string LocationTaskLongName { get; set; }

        public DateTime ResponseStartTime { get; set; }
        public DateTime ResponseEndTime { get; set; }

        public int KidsEmoTrackId { get; set; }

        public bool IsExist { get; set; }

        public string Q1Display { get; set; }
        public string Q1DisplayShort { get; set; }
        public string Q1DB { get; set; }
        public string Q2Display { get; set; }
        public string Q2DisplayShort { get; set; }
        public string Q2DB { get; set; }
        public string Q3Display { get; set; }
        public string Q3DisplayShort { get; set; }
        public string Q3DB { get; set; }
        public string Q4Display { get; set; }
        public string Q4DisplayShort { get; set; }
        public string Q4DB { get; set; }
        public string Q5Display { get; set; }
        public string Q5DisplayShort { get; set; }
        public string Q5DB { get; set; }
        public string Q1Ans { get; set; }
        public string Q2Ans { get; set; }
        public string Q3Ans { get; set; }
        public string Q4Ans { get; set; }
        public string Q5Ans { get; set; }

        public string QuestionHint { get; set; }

        public string LongNameModeOfTransport { get; set; }
        public string LongFromLocation { get; set; }
        public string LongToLocation { get; set; }
        public string LongTaskWhile { get; set; }
        public string LongTravelDetails { get; set; }
        public string LongTaskActivity { get; set; }
        public string IfOtherMOT { get; set; }

        public KidsReactOVM()
        {
            IsExist = false;

            Q1Display = Constants.Q1Display;
            Q1DisplayShort = Constants.Q1DisplayShort;
            Q1DB = Constants.Q1DB;

            Q2Display = Constants.Q2Display;
            Q2DisplayShort = Constants.Q2DisplayShort;
            Q2DB = Constants.Q2DB;

            Q3Display = Constants.Q3Display;
            Q3DisplayShort = Constants.Q3DisplayShort;
            Q3DB = Constants.Q3DB;

            Q4Display = Constants.Q4Display;
            Q4DisplayShort = Constants.Q4DisplayShort;
            Q4DB = Constants.Q4DB;

            Q5Display = Constants.Q5Display;
            Q5DisplayShort = Constants.Q5DisplayShort;
            Q5DB = Constants.Q5DB;

            PageQuestion = "Thinking about when you were ";
            PageQContd = "how did you feel?";
            //PageQuestionSurveyVariant = "Now, we would like to learn in more detail about how you felt during ";
            ResponseStartTime = DateTime.Now;

            //EAPageQuestion1 = "We would like to know how you felt, on average, during the ";
            //EAPageQuestion2 = "minutes you spent on ";
            QuestionHint = "Please rate each feeling on the scale given. A rating of 0 means that you did not experience that feeling at all. A rating of 6 means that this feeling was a very important part of the experience. Please select the number between 0 and 6 that best describes how you felt.";
        }

    }
    [Serializable]
    public class KidsReactTVM : ViewModelBase
    {
        public string PageQuestion { get; set; }
        public string QTasks { get; set; }
        public string QLocation { get; set; }
        public string QStartedAt { get; set; }
        public string QEndedAt { get; set; }
        public string PageQContd { get; set; }

        public IList<KidsEmoStageTracked> KidsEMoTrackListObj { get; set; }

        public int? KidsLocationId { get; set; }
        public int? KidsTravelId { get; set; }
        public string SurveyDate { get; set; }
        public int? TaskId { get; set; }
        public string TaskName { get; set; }

        public int KidsEmoTrackId { get; set; }

        public DateTime ResponseStartTime { get; set; }
        public DateTime ResponseEndTime { get; set; }

        public bool IsExist { get; set; }

        public string Q6Display { get; set; }
        public string Q6DisplayShort { get; set; }
        public string Q6DB { get; set; }
        public string Q7Display { get; set; }
        public string Q7DisplayShort { get; set; }
        public string Q7DB { get; set; }
        public string Q8Display { get; set; }
        public string Q8DisplayShort { get; set; }
        public string Q8DB { get; set; }
        public string Q9Display { get; set; }
        public string Q9DisplayShort { get; set; }
        public string Q9DB { get; set; }
        public string Q10Display { get; set; }
        public string Q10DisplayShort { get; set; }
        public string Q10DB { get; set; }
        public string Q6Ans { get; set; }
        public string Q7Ans { get; set; }
        public string Q8Ans { get; set; }
        public string Q9Ans { get; set; }
        public string Q10Ans { get; set; }

        public string Q12DB { get; set; }
        public string Q12Display { get; set; }
        public string Q12DisplayShort { get; set; }
        public string Q12Ans { get; set; }

        public string QuestionHint { get; set; }

        public string LongNameModeOfTransport { get; set; }
        public string LongFromLocation { get; set; }
        public string LongToLocation { get; set; }
        public string LongTaskWhile { get; set; }
        public string LongTravelDetails { get; set; }
        public string LongTaskActivity { get; set; }
        public string IfOtherMOT { get; set; }

        public KidsReactTVM()
        {
            IsExist = false;

            Q6Display = Constants.Q6Display;
            Q6DisplayShort = Constants.Q6DisplayShort;
            Q6DB = Constants.Q6DB;

            Q7Display = Constants.Q7Display;
            Q7DisplayShort = Constants.Q7DisplayShort;
            Q7DB = Constants.Q7DB;

            Q8Display = Constants.Q8Display;
            Q8DisplayShort = Constants.Q8DisplayShort;
            Q8DB = Constants.Q8DB;

            Q9Display = Constants.Q9Display;
            Q9DisplayShort = Constants.Q9DisplayShort;
            Q9DB = Constants.Q9DB;

            Q10Display = Constants.Q10Display;
            Q10DisplayShort = Constants.Q10DisplayShort;
            Q10DB = Constants.Q10DB;

            Q12Display = Constants.Q12Display;
            Q12DisplayShort = Constants.Q12DisplayShort;
            Q12DB = Constants.Q12DB;

            PageQuestion = "Thinking about when you were ";
            PageQContd = "how did you feel?";
            ResponseStartTime = DateTime.Now;

            QuestionHint = "Please rate each feeling on the scale given. A rating of 0 means that you did not experience that feeling at all. A rating of 6 means that this feeling was a very important part of the experience. Please select the number between 0 and 6 that best describes how you felt.";
        }
    }
    public class KidsReactionVM
    {
        public int Id { get; set; }
        public string TaskPerformed { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string SurveyDate { get; set; }
        public string Location { get; set; }
        public string OtherLocation { get; set; }

        public string Question { get; set; }
        public string Answer { get; set; }
        public string RatingString { get; set; }

        public int EmoId { get; set; }
        public string OtherModeOfTransport { get; set; }
        public int? KidsTravelId { get; set; }
        public string ModeOfTransport { get; set; }
        public string OtherTasks { get; set; }
        public int? KidsLocationId { get; set; }
    }

    public class AllSummarziedVM : ViewModelBase
        {
            public Lookup<string, KidsReactionVM> FullResponseList { get; set; }

            public string Comment;

            public string LocationName { get; set; }
            public string TaskPerformed { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }

        }
    public class EmoSummaryVM : ViewModelBase
    {
        public Lookup<string, KidsReactionVM> FullResponseList { get; set; }

        public Lookup<string, KidsReactionVM> FullFromLocationList { get; set; }
        public Lookup<string, KidsReactionVM> FullTravelList { get; set; }
        public Lookup<string, KidsReactionVM> FullToLocationList { get; set; }

        public string Comment;

        public string LocationName { get; set; }
        public string TaskPerformed { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public string TravelFromTo { get; set; }
        public string FromLocationName { get; set; }
        public string ToLocationName { get; set; }
    }
    [Serializable]
    public class KidsEditReactionVM : ViewModelBase
        {
            public string PageQuestion { get; set; }
            public string PageQuestionSurveyVariant { get; set; }
            public DateTime PageStartDateTimeUtc { get; set; }
            public DateTime PageEndTime { get; set; }
            public int TaskId { get; set; }
            public string TaskName { get; set; }
            public DateTime TaskStartDateTime { get; set; }

            public string StartTime { get; set; }
            public string EndTime { get; set; }
            public int EMoID { get; set; }

            public bool IsExist { get; set; }
            public string Q1Display { get; set; }
            public string Q1DB { get; set; }
            public string Q2Display { get; set; }
            public string Q2DB { get; set; }
            public string Q3Display { get; set; }
            public string Q3DB { get; set; }
            public string Q4Display { get; set; }
            public string Q4DB { get; set; }
            public string Q5Display { get; set; }
            public string Q5DB { get; set; }
            public string Q6Display { get; set; }
            public string Q6DB { get; set; }
            public string Q7Display { get; set; }
            public string Q7DB { get; set; }
            public string Q8Display { get; set; }
            public string Q8DB { get; set; }
            public string Q9Display { get; set; }
            public string Q9DB { get; set; }
            public string Q10Display { get; set; }
            public string Q10DB { get; set; }
            public string Q1Ans { get; set; }
            public string Q2Ans { get; set; }
            public string Q3Ans { get; set; }
            public string Q4Ans { get; set; }
            public string Q5Ans { get; set; }
            public string Q6Ans { get; set; }
            public string Q7Ans { get; set; }
            public string Q8Ans { get; set; }
            public string Q9Ans { get; set; }
            public string Q10Ans { get; set; }
            public string Q12Display { get; set; }
            public string Q12DB { get; set; }
            public string Q12Ans { get; set; }

            public KidsEditReactionVM()
            {
                IsExist = false;
                Q1Display = Constants.Q1Display;
                Q1DB = Constants.Q1DB;
                Q2Display = Constants.Q2Display;
                Q2DB = Constants.Q2DB;
                Q3Display = Constants.Q3Display;
                Q3DB = Constants.Q3DB;
                Q4Display = Constants.Q4Display;
                Q4DB = Constants.Q4DB;
                Q5Display = Constants.Q5Display;
                Q5DB = Constants.Q5DB;
                Q6Display = Constants.Q6Display;
                Q6DB = Constants.Q6DB;
                Q7Display = Constants.Q7Display;
                Q7DB = Constants.Q7DB;
                Q8Display = Constants.Q8Display;
                Q8DB = Constants.Q8DB;
                Q9Display = Constants.Q9Display;
                Q9DB = Constants.Q9DB;
                Q10Display = Constants.Q10Display;
                Q10DB = Constants.Q10DB;
                Q12Display = Constants.Q12Display;
                Q12DB = Constants.Q12DB;

            PageQuestion = "Edit your response on how you felt during ";
                PageQuestionSurveyVariant = "Edit your response on how you felt during ";
                PageStartDateTimeUtc = DateTime.UtcNow;
            }
        }
    
}