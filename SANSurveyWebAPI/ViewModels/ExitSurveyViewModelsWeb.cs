using SANSurveyWebAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.ViewModels.Web
{

    #region Exit Survey VM
    public class ExitSurveyVM
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileEmail { get; set; }
        public int ProfileOffset { get; set; }
        public int MaxStepExitSurvey { get; set; }
        public string ProgressNext { get; set; }
        public DateTime? LastUpdatedDateTimeUtc { get; set; }
    }

    [Serializable]
    public class WizardViewModel_ExitSurvey
    {
        public int CurrStep { get; set; }
        public int MaxStep { get; set; }

    }

    [Serializable]
    public class Page1_ExitSurveyVM : ExitSurveyVM
    {
        public IList<Page1Qns> QnsList { get; set; }

        public Page1_ExitSurveyVM()
        {
            QnsList = new List<Page1Qns>();
        }
    }

    [Serializable]
    public class Page2_ExitSurveyVM : ExitSurveyVM
    {
        public IList<Page2Qns> QnsList { get; set; }

        public Page2_ExitSurveyVM()
        {
            QnsList = new List<Page2Qns>();
        }
    }
    public class Page1Qns
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string Ans { get; set; }
    }
    public class Page2Qns
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string Ans { get; set; }
    }



    [Serializable]
    public class Page3_ExitSurveyVM : ExitSurveyVM
    {
        public string Q1 { get; set; } //Have you ever considered leaving medical practice
        public string Q2 { get; set; } //Have you considered working in another country
    }


    [Serializable]
    public class Page4_ExitSurveyVM : ExitSurveyVM
    {
        public IList<Page4Qns> QnsList { get; set; }

        public Page4_ExitSurveyVM()
        {
            QnsList = new List<Page4Qns>();
        }
    }

    public class Page4Qns
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string Ans { get; set; }
    }


    [Serializable]
    public class Page5_ExitSurveyVM : ExitSurveyVM
    {
        public IList<Page5Qns> QnsList { get; set; }

        public Page5_ExitSurveyVM()
        {
            QnsList = new List<Page5Qns>();
        }
    }
    public class Page5Qns
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string Ans { get; set; }
    }
    [Serializable]
    public class PageContinued5_ExitSurveyVM : ExitSurveyVM
    {
        public IList<PageContinued5Qns> QnsList { get; set; }

        public PageContinued5_ExitSurveyVM()
        {
            QnsList = new List<PageContinued5Qns>();
        }
    }    
    public class PageContinued5Qns
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string Ans { get; set; }
    }


    [Serializable]
    public class Page6_ExitSurveyVM : ExitSurveyVM
    {
        public string Q1 { get; set; } //Please mark the appropriate answer: My immediate senior1 is ...
        public string Q2 { get; set; } //Please mark the appropriate answer: My immediate senior1 is ...

        public string Q1Other { get; set; } //other text
        public string Q2Other { get; set; } //other text
    }

    [Serializable]
    public class Page7_ExitSurveyVM : ExitSurveyVM
    {
        public IList<Page7Qns> QnsList { get; set; }

        public Page7_ExitSurveyVM()
        {
            QnsList = new List<Page7Qns>();
        }
    }

    public class Page7Qns
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string Ans { get; set; }
    }


    [Serializable]
    public class Page8_ExitSurveyVM : ExitSurveyVM
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



    [Serializable]
    public class Page9_ExitSurveyVM : ExitSurveyVM
    {
        public IList<Page9Qns> QnsList { get; set; }

        public Page9_ExitSurveyVM()
        {
            QnsList = new List<Page9Qns>();
        }
    }

    public class Page9Qns
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string Ans { get; set; }
    }



 


    [Serializable]
    public class Page10_ExitSurveyVM : ExitSurveyVM
    {
        public IList<Page10Qns> QnsList { get; set; }

        public Page10_ExitSurveyVM()
        {
            QnsList = new List<Page10Qns>();
        }
    }

    public class Page10Qns
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string Ans { get; set; }
    }
    public class Page11Qptions
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string Ans { get; set; }
    }
    [Serializable]
    public class Page11_ExitSurveyVM : ExitSurveyVM
    {
        public IList<Page11Qptions> QptionsList { get; set; }

        public Page11_ExitSurveyVM()
        {
            QptionsList = new List<Page11Qptions>();
        }
        public string HiddenTrainingOptionIds { get; set; }
        public string HiddenValuedOptionIds { get; set; }
        public string TrainingOther { get; set; }
        public string ValuedOther { get; set; }
        public string AllOptionIndexes { get; set; }
    } 


    [Serializable]
    public class Page12_ExitSurveyVM : ExitSurveyVM
    {
        public IList<Page12Qns> QnsList { get; set; }

        public Page12_ExitSurveyVM()
        {
            QnsList = new List<Page12Qns>();
        }
    }

    public class Page12Qns
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string Ans { get; set; }
    }
    [Serializable]
    public class PageContinued12_ExitSurveyVM : ExitSurveyVM
    {
        public IList<PageContinued12Qns> QnsList { get; set; }

        public PageContinued12_ExitSurveyVM()
        {
            QnsList = new List<PageContinued12Qns>();
        }
    }
    public class PageContinued12Qns
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string Ans { get; set; }
    }

    [Serializable]
    public class Page13_ExitSurveyVM : ExitSurveyVM
    {
        public string Q1_Applicable { get; set; }
        public int Q1_Year { get; set; }
        public string Q2_PTWork { get; set; }
        public string Q2_Other { get; set; }
        public int Q3_NoOfPeople { get; set; }
        public string Q4_Martial { get; set; }
        public string Q5_PartnershipMarried { get; set; }
    }

    [Serializable]
    public class Page14_ExitSurveyVM : ExitSurveyVM
    {
        public string Q1 { get; set; }
    }

    [Serializable]
    public class FeedbackFor_ExitSurveyVM : ExitSurveyVM
    {
        public string feedbackComments { get; set; }
    }
    
    [Serializable]
    public class CompletedViewModel_ExitSurvey : ExitSurveyVM
    {}

    [Serializable]
    public class FeedbackViewModel_ExitSurvey : ExitSurveyVM
    {
        public string Comment { get; set; }
    }


   

    #endregion

}
