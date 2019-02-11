using SANSurveyWebAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.ViewModels.Web
{

    public abstract class ViewModelBase
    {
        public string ShiftSpan { get; set; }
        public string SurveySpan { get; set; }
        public string Uid { get; set; }
        public string SurveyProgressNext { get; set; }


    }

    #region NEW MULTITASK

    [Serializable]
    public class SurveyMultiTaskVM : ViewModelBase
    {
        public int TaskId { get; set; }
        public string PageQuestion { get; set; }
        public string PageQuestionNext { get; set; }
        public string QuestionHint { get; set; }
        public string QuestionHint2 { get; set; }
        public string QuestionHint3 { get; set; }
        public string QuestionHint4 { get; set; }
        public string PageQuestionSurveyVariant { get; set; }
        public DateTime PageStartDateTimeUtc { get; set; }
        public DateTime PageEndTime { get; set; }
        public string OtherTaskName { get; set; }
        public List<CheckBoxListItem> FullTaskList { get; set; }
        public IList<MyDayTaskList> GetFullTasksByProfileId { get; set; }
        public SurveyMultiTaskVM()
        {
            FullTaskList = new List<CheckBoxListItem>();
            PageStartDateTimeUtc = DateTime.UtcNow;
            PageQuestion = "What did you do between ";
            QuestionHint = "Please select all of the tasks that you did during this time. ";
            QuestionHint2 = "Note: To select multiple tasks, ";
            QuestionHint3 = "On Windows: please CTRL - click and add";
            QuestionHint4 = "On MAC: please SHIFT - click and add";
            PageQuestionNext = "Please select the task that you did next, starting at ";
            PageQuestionSurveyVariant = "We would like to learn what you did and how you felt ";
            PageStartDateTimeUtc = DateTime.UtcNow;
        }
        public string HiddenSelectedTasksIds { get; set; }
        public DateTime TotalMinsSpentOnTask { get; set; }
        public List<AlltheTasks> AllTaskItemsObj { get; set; }
        public bool IsEdit { get; set; }
        public int TotalTaskSelectionLimit { get; set; }
    }
    public class AlltheTasks
    {
        public int Id { get; set; }
        public string ShortName { get; set; }
        public string IsSelected { get; set; }
    }

    [Serializable]
    public class SurveyTaskViewVM : ViewModelBase
    {
        public int ID { get; set; }
        public int ProfileId { get; set; }
        public int SurveyId { get; set; }
        public string TaskName { get; set; }
        public string TaskCategory { get; set; }
        public DateTime TotalMinsSpentOnTask { get; set; }

    }
    [Serializable]
    public class SurveyMyTaskViewVM:ViewModelBase
    {
        public MyDayTaskList TaskListObj { get; set; }//contains single meeting details
        public List<MyDayTaskList> TaskListsObj { get; set; }//list of meeting
        public string Status { get; set; }//This will help us to handle the view logic                    
        public int hiddenTaskIds { get; set; }
        public int totalRowCount { get; set; }
        public IList<MyDayTaskList> RandomSelectedTaskListObj { get; set; }
        public string PageQuestion { get; set; }
        public string QuestionHint { get; set; }
        public SurveyMyTaskViewVM()
        {
            PageQuestion = "How much time in total did you spend on each of these tasks between ";
            QuestionHint = "Please enter the total amount of time, in minutes, that you spend on each task.";
        }
        public int TotalTaskHoursAlloted { get; set; }
    }

    [Serializable]
    public class GetSelectedTasksVM : ViewModelBase
    {
        public List<MyDayTaskList> SelectedTasks { get; set; }
        public int[] SelectedTaskIds { get; set; }
        public List<TaskItem> AvailableTasks { get; set; }
        public int[] AvailableTaskIds { get; set; }
    }

    #endregion

    public class SurveyExistsCheck
    {
        public bool SurveysExists { get; set; }
        public bool IsHangFireJobsCreated { get; set; }
    }

    #region Survey1 Models

    [Serializable]
    public class SurveyTasks1ViewModel : ViewModelBase
    {
        //public string SurveyId { get; set; }
        //public string SurveyPeriod { get; set; }


        public string PageQuestion { get; set; }
        public string PageQuestionNext { get; set; }
        public string PageQuestionSurveyVariant { get; set; }

        public DateTime PageStartDateTimeUtc { get; set; }
        public DateTime PageEndTime { get; set; }

        public string OtherTaskName { get; set; }



        //Currently used in Survey1
        //public decimal CurrProgressValue { get; set; }

        public List<CheckBoxListItem> FullTaskList { get; set; }



        public SurveyTasks1ViewModel()
        {
            FullTaskList = new List<CheckBoxListItem>();
            PageStartDateTimeUtc = DateTime.UtcNow;

            //if its first time, Please select the task that you did at this time
            //if not, what did you do next? Please select the task that you did at this time
            PageQuestion = "Please select the task that you did starting at ";
            PageQuestionNext = "Please select the task that you did next, starting at ";
            PageQuestionSurveyVariant = "We would like to learn what you did and how you felt ";
            PageStartDateTimeUtc = DateTime.UtcNow;

        }

    }

    [Serializable]
    public class SurveyTaskTime1ViewModel : ViewModelBase
    {
        //public string SurveyId { get; set; }
        //public string SurveyPeriod { get; set; }

        public string PageQuestion { get; set; }
        public string PageQuestionSurveyVariant { get; set; }

        public DateTime PageStartDateTimeUtc { get; set; }
        public DateTime PageEndDateTimeUtc { get; set; }

        public int TaskId { get; set; }
        public string TaskName { get; set; }

        //public decimal StepValue { get; set; }
        //public decimal CurrProgressValue { get; set; }

        //public DateTime TaskTimeStartTime { get; set; }
        //public DateTime TaskTimeEndTime { get; set; }

        public int TimeHours { get; set; }
        public int TimeMinutes { get; set; }


        public int remainingTimeHours { get; set; }
        public int remainingTimeMinutes { get; set; }

        public string QDisplay { get; set; }
        public string QDisplayShort { get; set; }

        public string QDB { get; set; }



        public SurveyTaskTime1ViewModel()
        {
            QDisplay = Constants.QDisplay;
            QDisplayShort = Constants.QDisplayShort;

            QDB = Constants.QDB;

            PageQuestion = "How much time did you spend on ";
            PageQuestionSurveyVariant = "We would like to learn how much time was spent on ";
            PageStartDateTimeUtc = DateTime.UtcNow;
        }


    }

    [Serializable]
    public class SurveyWAMTaskTimeVM : ViewModelBase
    {
        public string PageQuestion { get; set; }
        public string Question2 { get; set; }
        public string Question2Hint { get; set; }
        public string PageQuestionSurveyVariant { get; set; }
        public DateTime PageStartDateTimeUtc { get; set; }
        public DateTime PageEndDateTimeUtc { get; set; }
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public int TimeHours { get; set; }
        public int TimeMinutes { get; set; }
        public int remainingTimeHours { get; set; }
        public int remainingTimeMinutes { get; set; }
        public string QDisplay { get; set; }
        public string QDisplayShort { get; set; }
        public string QDB { get; set; }
        public string ErrorMessage { get; set; }
        public string Question3 { get; set; }

        public SurveyWAMTaskTimeVM()
        {
            QDisplay = Constants.QDisplay;
            QDisplayShort = Constants.QDisplayShort;
            QDB = Constants.QDB;

            PageQuestion = "How much time did you spend on ";
            PageQuestionSurveyVariant = "We would like to learn how much time was spent on ";
            PageStartDateTimeUtc = DateTime.UtcNow;
            Question2 = "Who were you with during this time, while you were doing this task?";
            Question2Hint = "Please select all that apply.";
            ErrorMessage = "Please select at least one option to proceed.";
            Question3 = "Where were you during this time, while you were doing this task?";
        }
        public IList<WAMTaskTimeOptions> OptionsList { get; set; }
        public string WithWHomOther { get; set; }
        public string HiddenOptionIds { get; set; }
        public string NearestLocation { get; set; }
        public string NearestOtherLocation { get; set; }
    }
    public class WAMTaskTimeOptions
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string Ans { get; set; }
    }    
    [Serializable]
    public class SurveyTaskRatingViewModel : ViewModelBase
    {

        //public string SurveyId { get; set; }
        //public string SurveyPeriod { get; set; }

        public string PageQuestion { get; set; }
        public string PageQuestionSurveyVariant { get; set; }

        public DateTime PageStartDateTimeUtc { get; set; }
        public DateTime PageEndTime { get; set; }

        public int TaskId { get; set; }
        public string TaskName { get; set; }



        //public decimal StepValue { get; set; }
        //public decimal CurrProgressValue { get; set; }


        //public DateTime TaskRatingStartTime { get; set; }
        //public DateTime TaskRatingEndTime { get; set; }


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
        //public Random a = new Random(); // replace from new Random(DateTime.Now.Ticks.GetHashCode());
        //                                // Since similar code is done in default constructor internally
        //public List<int> randomList = new List<int>();
        //int MyNumber = 0;
        //private void NewNumber()
        //{
        //    MyNumber = a.Next(0, 10);
        //    if (!randomList.Contains(MyNumber))
        //        randomList.Add(MyNumber);
        //}

        public SurveyTaskRatingViewModel()
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
            PageStartDateTimeUtc = DateTime.UtcNow;
        }




    }



    [Serializable]
    public class SurveyEditResponseViewModel : ViewModelBase
    {

        //public string SurveyId { get; set; }
        //public string SurveyPeriod { get; set; }

        public string PageQuestion { get; set; }
        public string PageQuestionSurveyVariant { get; set; }

        public DateTime PageStartDateTimeUtc { get; set; }
        public DateTime PageEndTime { get; set; }

        public int TaskId { get; set; }
        public string TaskName { get; set; }

        public DateTime TaskStartDateTime { get; set; }



        //public decimal StepValue { get; set; }
        //public decimal CurrProgressValue { get; set; }


        //public DateTime TaskRatingStartTime { get; set; }
        //public DateTime TaskRatingEndTime { get; set; }


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
        //public Random a = new Random(); // replace from new Random(DateTime.Now.Ticks.GetHashCode());
        //                                // Since similar code is done in default constructor internally
        //public List<int> randomList = new List<int>();
        //int MyNumber = 0;
        //private void NewNumber()
        //{
        //    MyNumber = a.Next(0, 10);
        //    if (!randomList.Contains(MyNumber))
        //        randomList.Add(MyNumber);
        //}

        public SurveyEditResponseViewModel()
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
    public class SurveyTaskDurationVM : ViewModelBase
    {
        public List<MyDayTaskList> MyDayTaskListsObj { get; set; }
    }
        [Serializable]
    public class SurveyEditResponseAffectVM : ViewModelBase
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

        public SurveyEditResponseAffectVM()
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
    public class SurveyWAMTaskRating1VM : ViewModelBase
    {
        public string PageQuestion { get; set; }
        public string PageQuestionSurveyVariant { get; set; }
        public DateTime PageStartDateTimeUtc { get; set; }
        public DateTime PageEndDateTimeUtc { get; set; }
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
        //http://stackoverflow.com/questions/14473321/generating-random-unique-values-c-sharp
        //http://codereview.stackexchange.com/questions/61338/generate-random-numbers-without-repetitions       
        public string DisplayPara1 { get; set; }
        public string DisplayPara2 { get; set; }
        public string NextTaskName { get; set; }
        public int TaskRound { get; set; }
        public string EAPageQuestion1 { get; set; }
        public string EAPageQuestion2 { get; set; }
        public string QuestionHint { get; set; }
        public SurveyWAMTaskRating1VM()
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
            PageStartDateTimeUtc = DateTime.UtcNow;

            EAPageQuestion1 = "We would like to know how you felt, on average, during the ";
            EAPageQuestion2 = "minutes you spent on ";
            QuestionHint = "Please rate each feeling on the scale given. A rating of 0 means that you did not experience that feeling at all. A rating of 6 means that this feeling was a very important part of the experience. Please select the number between 0 and 6 that best describes how you felt.";
        }
    }
    [Serializable]
    public class SurveyTaskRating1ViewModel : ViewModelBase
    {       
        public string PageQuestion { get; set; }
        public string PageQuestionSurveyVariant { get; set; }
        public DateTime PageStartDateTimeUtc { get; set; }
        public DateTime PageEndDateTimeUtc { get; set; }
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
        //http://stackoverflow.com/questions/14473321/generating-random-unique-values-c-sharp
        //http://codereview.stackexchange.com/questions/61338/generate-random-numbers-without-repetitions       
        public string DisplayPara1 { get; set; }
        public string DisplayPara2 { get; set; }
        public string NextTaskName { get; set; }
        public int TaskRound { get; set; }
        public string EAPageQuestion1 { get; set; }
        public string EAPageQuestion2 { get; set; }
        public string QuestionHint { get; set; }
        public SurveyTaskRating1ViewModel()
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
            PageStartDateTimeUtc = DateTime.UtcNow;

            EAPageQuestion1 = "We would like to know how you felt, on average, during the ";
            EAPageQuestion2 = "minutes you spent on ";
            QuestionHint = "Please rate each feeling on the scale given. A rating of 0 means that you did not experience that feeling at all. A rating of 6 means that this feeling was a very important part of the experience. Please select the number between 0 and 6 that best describes how you felt.";
        }
    }

    [Serializable]
    public class SurveyFeedbackViewModel : ViewModelBase
    {
        public string Comment { get; set; }
    }
    [Serializable]
    public class SurveyWAMTaskRating2VM : ViewModelBase
    {
        public string PageQuestion { get; set; }
        public string PageQuestionSurveyVariant { get; set; }
        public DateTime PageStartDateTimeUtc { get; set; }
        public DateTime PageEndDateTimeUtc { get; set; }
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

        //http://stackoverflow.com/questions/14473321/generating-random-unique-values-c-sharp
        //http://codereview.stackexchange.com/questions/61338/generate-random-numbers-without-repetitions       
        public string EAPageQuestion1 { get; set; }
        public string EAPageQuestion2 { get; set; }
        public string QuestionHint { get; set; }
        public SurveyWAMTaskRating2VM()
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
            PageStartDateTimeUtc = DateTime.UtcNow;

            EAPageQuestion1 = "We would like to know how you felt, on average, during the ";
            EAPageQuestion2 = "minutes you spent on ";
            QuestionHint = "Please rate each feeling on the scale given. A rating of 0 means that you did not experience that feeling at all. A rating of 6 means that this feeling was a very important part of the experience. Please select the number between 0 and 6 that best describes how you felt.";
        }
    }

    [Serializable]
    public class SurveyTaskRating2ViewModel : ViewModelBase
    {
        public string PageQuestion { get; set; }
        public string PageQuestionSurveyVariant { get; set; }
        public DateTime PageStartDateTimeUtc { get; set; }
        public DateTime PageEndDateTimeUtc { get; set; }
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

        //http://stackoverflow.com/questions/14473321/generating-random-unique-values-c-sharp
        //http://codereview.stackexchange.com/questions/61338/generate-random-numbers-without-repetitions       
        public string EAPageQuestion1 { get; set; }
        public string EAPageQuestion2 { get; set; }
        public string QuestionHint { get; set; }
        public SurveyTaskRating2ViewModel()
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
            PageStartDateTimeUtc = DateTime.UtcNow;

            EAPageQuestion1 = "We would like to know how you felt, on average, during the ";
            EAPageQuestion2 = "minutes you spent on ";
            QuestionHint = "Please rate each feeling on the scale given. A rating of 0 means that you did not experience that feeling at all. A rating of 6 means that this feeling was a very important part of the experience. Please select the number between 0 and 6 that best describes how you felt.";
        }
    }   


    public class SurveyTaskTimeCheckViewModel
    {
        public int remainingTimeHrs { get; set; }
        public int remainingTimeMins { get; set; }
    }



    public class SurveyResponseViewModel
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
    public class SurveyWAMResponseVM
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
    public class SurveyResponseAffectVM
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
    public class SurveyResponseWRViewModel
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

        public DateTime? WRStartDateTime { get; set; }
        public string WRStartDate { get; set; }
        public string WRStartTime { get; set; }

        public DateTime? WREndDateTime { get; set; }
        public string WREndDate { get; set; }
        public string WREndTime { get; set; }

    }


    public class SurveyNonEditViewModel : UserHeaderMV
    {
        public string surveySpan { get; set; }
        public Lookup<string, SurveyResponseViewModel> FullResponseList { get; set; }
        public Lookup<string, SurveyResponseViewModel> FullResponseAdditionalList { get; set; }
        public Lookup<string, SurveyResponseWRViewModel> FullResponseWRList { get; set; }
    }

    public class SurveyWAMNonEditViewModel : UserHeaderMV
    {
        public string surveySpan { get; set; }
        public Lookup<string, SurveyWAMResponseVM> FullResponseList { get; set; }
        public Lookup<string, SurveyWAMResponseVM> FullResponseAdditionalList { get; set; }        
    }



    public class SurveyResults1ViewModel : ViewModelBase
    {
        public Lookup<string, SurveyResponseViewModel> FullResponseList { get; set; }
        public Lookup<string, SurveyResponseAffectVM> FullResponseAffectList { get; set; }
        public Lookup<string, SurveyResponseViewModel> FullResponseAdditionalList { get; set; }
        public Lookup<string, SurveyResponseWRViewModel> FullResponseWRList { get; set; }
        public Lookup<string, SurveyWAMResponseVM> FullWAMResponseList { get; set; }
        public Lookup<string, SurveyWAMResponseVM> FullWAMResponseAdditionalList { get; set; }

        public string Comment;
    }


    public class EmailFeedbackViewModel
    {
        //public bool EmailAddress { get; set; }
        [MaxLength(256)]
        public string EmailAddress { get; set; }
        [MaxLength(256)]
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        [MaxLength(50)]
        public string PreferedContact { get; set; } //Email, Phone
        [MaxLength(50)]
        public string PreferedTime { get; set; }
    }




    #endregion


    #region Survey 2 Models
    [Serializable]
    public class Survey2ShiftTime1ViewModel : ViewModelBase
    {
        public string PageQuestion { get; set; }
        public DateTime PageStartDateTimeUtc { get; set; }
        public DateTime PageEndTime { get; set; }
        public DateTime ShiftStartTime { get; set; }
        public DateTime ShiftEndTime { get; set; }
        public DateTime SurveyStartTime { get; set; }
        public DateTime SurveyEndTime { get; set; }
        public string IsOnCall { get; set; }
        public string WasWorking { get; set; }
        public Survey2ShiftTime1ViewModel()
        {
            //PageQuestion = "First, what time did you start and finish your last work day?";
            PageQuestion = "Welcome to your MyDay survey. In this survey, we would like to ask you about the following time window";
            PageStartDateTimeUtc = DateTime.UtcNow;
        }

    }


    [Serializable]
    public class InvalidError : ViewModelBase
    {

    }


    [Serializable]
    public class SidMV : ViewModelBase
    {
        //public string uid { get; set; }
    }


    [Serializable]
    public class ResumeSurveyMV : ViewModelBase
    {
    }



    [Serializable]
    public class SurveyError : ViewModelBase
    {

        public string Source { get; set; }
        public string HResult { get; set; }
        public string TargetSite { get; set; }
        public string InnerException { get; set; }
        public string StackTrace { get; set; }
        public string Message { get; set; }
        public string LineNumber { get; set; }
        public string CodeFile { get; set; }



        public string DisplayError()
        {
            LineNumber = StackTrace.Substring(StackTrace.LastIndexOf(' '));
            return String.Format("Message: {0}, CodeFile: {1}, LineNumber: {2}", Message, CodeFile, LineNumber);
        }



    }













    #endregion


    public class MyDayErrorLogViewModel : ViewModelBase
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string SurveyUID { get; set; }
        public DateTime AccessedDateTime { get; set; }
        public string ErrorMessage { get; set; }
        public string HtmlContent { get; set; }
    }

}