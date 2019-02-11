using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.ViewModels.Web
{
    public class ExitV2VM
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileEmail { get; set; }
        public int ProfileOffset { get; set; }
        public int MaxExitV2Step { get; set; }
        public string ProgressNext { get; set; }
        public DateTime? LastUpdatedDateTimeUtc { get; set; }
        public string CurrentDate { get; set; }
    }
    [Serializable]
    public class WizardViewModel_ExitV2VM
    {
        public int CurrStep { get; set; }
        public int MaxStep { get; set; }
    }
    [Serializable]
    public class ExitV2ProfileUpdatesVM : ExitV2VM
    {

    }

    public class QuestionAnswer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string Ans { get; set; }
    }
    //Page one 
    [Serializable]
    public class WellbeingVM : ExitV2VM
    {
        public IList<QuestionAnswer> QnsList { get; set; }

        public WellbeingVM()
        {
            QnsList = new List<QuestionAnswer>();
        }
    }
    //Page two
    [Serializable]
    public class FirstJobVM : ExitV2VM
    {
        public IList<QuestionAnswer> QnsList { get; set; }

        public FirstJobVM()
        {
            QnsList = new List<QuestionAnswer>();
        }
    }
    //Page Three
    [Serializable]
    public class SecondJobVM : ExitV2VM
    {
        public string Q1 { get; set; } //Have you ever considered leaving medical practice
        public string Q2 { get; set; } //Have you considered working in another country
    }
    //Page Four
    [Serializable]
    public class ThirdJobVM : ExitV2VM
    {
        public IList<QuestionAnswer> QnsList { get; set; }

        public ThirdJobVM()
        {
            QnsList = new List<QuestionAnswer>();
        }
    }
    //Page Five
    [Serializable]
    public class FirstWEVM : ExitV2VM
    {
        public IList<QuestionAnswer> QnsList { get; set; }

        public FirstWEVM()
        {
            QnsList = new List<QuestionAnswer>();
        }
    }
    //Page Six
    [Serializable]
    public class SecondWEVM : ExitV2VM
    {
        public IList<QuestionAnswer> QnsList { get; set; }

        public SecondWEVM()
        {
            QnsList = new List<QuestionAnswer>();
        }
    }
    //Page Seven
    [Serializable]
    public class ThirdWEVM : ExitV2VM
    {
        public string Q1 { get; set; } //Please mark the appropriate answer: My immediate senior1 is ...
        public string Q1Other { get; set; } //other text        
    }
    //Page Eight
    [Serializable]
    public class FourthWEVM : ExitV2VM
    {
        public IList<QuestionAnswer> QnsList { get; set; }

        public FourthWEVM()
        {
            QnsList = new List<QuestionAnswer>();
        }
    }
    //Page 9
    [Serializable]
    public class FifthWEVM : ExitV2VM
    {
        public string Q1 { get; set; }
        public string Q2 { get; set; }
        public int Q3 { get; set; } //approx. years
        public string Q4 { get; set; } // i don't know
                                       // public int Q5 { get; set; } //months
        public int Q5 { get; set; } //months
        public int Q6 { get; set; } //weeks
        public int Q7 { get; set; } //days
        public int Q8 { get; set; } //days
    }
    //Page 10
    [Serializable]
    public class FirstTrainingVM : ExitV2VM
    {
        public IList<QuestionAnswer> QptionsList { get; set; }

        public FirstTrainingVM()
        {
            QptionsList = new List<QuestionAnswer>();
        }
        public string HiddenTrainingOptionIds { get; set; }
        public string HiddenValuedOptionIds { get; set; }
        public string TrainingOther { get; set; }
        public string ValuedOther { get; set; }
        public string AllOptionIndexes { get; set; }
    }
    //Page 10 edit
    public class QnAn
    {
        public int ID { get; set; }
        public string ForGridID { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string Ans { get; set; }
        public string OtherOptionString { get; set; }
    }

    [Serializable]
    public class FirstTraining2VM : ExitV2VM
    {
        public IList<QnAn> QnsYourTraining { get; set; }
        public IList<QnAn> QnsFeelingValued { get; set; }

        public FirstTraining2VM()
        {
            QnsYourTraining = new List<QnAn>();
            QnsFeelingValued = new List<QnAn>();
        }
    }
    //Page 11
    [Serializable]
    public class SecondTrainingVM : ExitV2VM
    {
        public IList<QuestionAnswer> QnsList { get; set; }

        public SecondTrainingVM()
        {
            QnsList = new List<QuestionAnswer>();
        }
    }
    //Page 12
    [Serializable]
    public class ThirdTrainingVM : ExitV2VM
    {
        public IList<QuestionAnswer> QnsList { get; set; }

        public ThirdTrainingVM()
        {
            QnsList = new List<QuestionAnswer>();
        }
    }
    //Page 13
    [Serializable]
    public class AboutYouESVM : ExitV2VM
    {
        public string Q1_Applicable { get; set; }
        public int Q1_Year { get; set; }
        public string Q2_PTWork { get; set; }
        public string Q2_Other { get; set; }
        public int Q3_NoOfPeople { get; set; }
        public string Q4_Martial { get; set; }
        public string Q5_PartnershipMarried { get; set; }
    }
}