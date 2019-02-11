using System;

namespace SANSurveyWebAPI.Models
{
    public class SessionSurveyTask
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        //public string SurveyPeriod { get; set; }
        public bool IsCompleted { get; set; }
        //public string SurveyId { get; set; }
        public decimal StepValue { get; set; }

        //public SessionSurveyTask()
        //{
        //    IsCompleted = false;
        //    UId = "Survey1";
        //    SurveyPeriod = "10:00am and 2:00pm";
        //}

    }

    public class SessionSurveyByDuration
    {
        public DateTime ShiftStart { get; set; }
        public DateTime ShiftEnd { get; set; }
        public int TotalDurationInMinutes { get; set; }
        public int TotalElapsed { get; set; }
        public string UId { get; set; }
        public decimal StepValue { get; set; }


        public SessionSurveyByDuration(DateTime shiftStart, DateTime shiftEnd)
        {
            ShiftEnd = shiftEnd;
            ShiftStart = shiftEnd;
            TimeSpan span = shiftEnd.Subtract(shiftStart);

            TotalDurationInMinutes = (int) Math.Round(span.TotalMinutes);
            TotalElapsed = 0;

            UId = "ShiftTime Survey";

        }


        public void IncreaseStepValue(int val)
        {
            StepValue += val;
        }

  





    }

}