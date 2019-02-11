using System;
using System.Linq;
using System.Web;
using System.Data;
using SANSurveyWebAPI.Models.Api;
using SANSurveyWebAPI.ViewModels.Web;
using System.Data.Entity;
using SANSurveyWebAPI.Models;


using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using static SANSurveyWebAPI.Constants;
using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.DAL;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SANSurveyWebAPI.BLL
{

    public class ExitService : IDisposable
    {
        /*
         
            - This layer implements UnitOfWork pattern
            - The Tables ProfileComments, ProfileDemo... are retrieved from the methods in this file
             
             
             */
        readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public ExitService()
        {
        }
        public ProfileDto GetCurrentLoggedInProfile()
        {
            string user = HttpContext.Current.User.Identity.GetUserName();

            if (user != null)
            {
                var profile = GetProfileByLoginEmail(user);
                return profile;
            }

            return null;
        }
        public IEnumerable<System.Web.Mvc.SelectListItem> GetAllSpecialities()
        {
            var list = _unitOfWork.SpecialitiesRespository.Get()
                .OrderBy(x => x.Sequence)
                .Select(c => new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });

            return list;

        }
        public IEnumerable<System.Web.Mvc.SelectListItem> GetAllBirthYears()
        {
            var list = _unitOfWork.BirthYearRespository.Get()
                .OrderBy(x => x.Sequence)
                .Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Name.ToString()
                });

            return list;
        }
        public int? ConvertToNumber(decimal? val)
        {
            if (val.HasValue)
            {

                decimal x = val.Value * 100;

                return (int) x;
            }
            return null;
        }
        public decimal? ConvertToDecimal(decimal? s)
        {
            if (s.HasValue)
                return s / 100;
            else
                return null;
        }
        public IList<int> GetProfileTaskItemsByProfileId(int profileId)
        {
            var list = _unitOfWork.ProfileTasksRespository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .Select(m => m.TaskItemId)
                .ToList();

            return list;

        }
        public IList<Page1Qns> GetExitSurveyPage1Ans(int profileId)
        {
            var list = _unitOfWork.ExitSurvey_Page1_Respository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .Select(m => new Page1Qns()
                {
                    ID = m.Id,
                    Name = m.Qn,
                    Ans = m.Ans
                }).ToList();

            return list;

        }
        public IList<Page2Qns> GetExitSurveyPage2Ans(int profileId)
        {
            var list = _unitOfWork.ExitSurvey_Page2_Respository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .Select(m => new Page2Qns()
                {
                    ID = m.Id,
                    Name = m.Qn,
                    Ans = m.Ans
                }).ToList();

            return list;

        }


        public IList<Page4Qns> GetExitSurveyPage4Ans(int profileId)
        {
            var list = _unitOfWork.ExitSurvey_Page4_Respository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .Select(m => new Page4Qns()
                {
                    ID = m.Id,
                    Name = m.Qn,
                    Ans = m.Ans
                }).ToList();

            return list;

        }

        public IList<Page5Qns> GetExitSurveyPage5Ans(int profileId)
        {
            var list = _unitOfWork.ExitSurvey_Page5_Respository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .Select(m => new Page5Qns()
                {
                    ID = m.Id,
                    Name = m.Qn,
                    Ans = m.Ans
                }).ToList();

            return list;

        }
        public IList<PageContinued5Qns> GetExitSurveyPageContinued5Ans(int profileId)
        {
            var list = _unitOfWork.ExitSurvey_PageContinued5_Respository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .Select(m => new PageContinued5Qns()
                {
                    ID = m.Id,
                    Name = m.Qn,
                    Ans = m.Ans
                }).ToList();

            return list;

        }
        public IList<Page7Qns> GetExitSurveyPage7Ans(int profileId)
        {
            var list = _unitOfWork.ExitSurvey_Page7_Respository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .Select(m => new Page7Qns()
                {
                    ID = m.Id,
                    Name = m.Qn,
                    Ans = m.Ans
                }).ToList();

            return list;

        }


        public IList<Page9Qns> GetExitSurveyPage9Ans(int profileId)
        {
            var list = _unitOfWork.ExitSurvey_Page9_Respository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .Select(m => new Page9Qns()
                {
                    ID = m.Id,
                    Name = m.Qn,
                    Ans = m.Ans
                }).ToList();

            return list;

        }

        public IList<Page10Qns> GetExitSurveyPage10Ans(int profileId)
        {
            var list = _unitOfWork.ExitSurvey_Page10_Respository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .Select(m => new Page10Qns()
                {
                    ID = m.Id,
                    Name = m.Qn,
                    Ans = m.Ans
                }).ToList();

            return list;

        }


        public IList<Page12Qns> GetExitSurveyPage12Ans(int profileId)
        {
            var list = _unitOfWork.ExitSurvey_Page12_Respository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .Select(m => new Page12Qns()
                {
                    ID = m.Id,
                    Name = m.Qn,
                    Ans = m.Ans
                }).ToList();

            return list;

        }

        public IList<PageContinued12Qns> GetExitSurveyPageContinued12Ans(int profileId)
        {
            var list = _unitOfWork.ExitSurvey_PageContinued12_Respository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .Select(m => new PageContinued12Qns()
                {
                    ID = m.Id,
                    Name = m.Qn,
                    Ans = m.Ans
                }).ToList();

            return list;

        }
        public IList<SelectedSpecialityVM> GetProfileSpecialitiesSpecialityIdByProfileId(int profileId)
        {
            var list = _unitOfWork.ProfileSpecialtiesRespository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .Select(x => new SelectedSpecialityVM() { ID = x.SpecialtyId, OtherText = x.OtherText })
                .ToList();

            return list;
        }
        public IList<int> GetProfileEthiniticitiesEthnicityIdByProfileId(int profileId)
        {
            var list = _unitOfWork.ProfileEthinicitiesRespository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .Select(x => x.EthinicityId)
                .ToList();

            return list;

        }
        public IList<int> GetProfileEthiniticitiesByProfileId(int profileId)
        {
            var list = _unitOfWork.ProfileEthinicitiesRespository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .Select(x => x.Id)
                .ToList();

            return list;

        }
        public IList<EthinicityVM> GetAllEthinicitiesMultiSelectList()
        {
            var list = _unitOfWork.EthinicityRespository.Get()
                .OrderBy(x => x.Sequence)
                .Select(x => new EthinicityVM()
                {
                    ID = x.Id,
                    Name = x.Name
                }).ToList();

            return list;

        }
        public IEnumerable<System.Web.Mvc.SelectListItem> GetAllMedicalUniversities()
        {

            var list = _unitOfWork.MedicalUniversitiesRespository.Get()
               .OrderBy(x => x.Sequence)
               .Select(x => new SelectListItem()
               {
                   Text = x.Name.Trim(),
                   Value = x.Name.ToString().Trim()
               });

            return list;
        }
        public IList<CheckBoxListItem> GetAllTasksItemsCheckBoxList()
        {
            var list = _unitOfWork.TaskItemRespository.Get()
                .OrderBy(x => x.Sequence)
                .Select(x => new CheckBoxListItem()
                {
                    ID = x.Id,
                    Display = x.ShortName,
                    Description = x.LongName,
                    IsChecked = false //set to true if need to select all first
                }).ToList();

            return list;
        }
        public IList<TaskVM> GetAllTasksItemsTableList()
        {
            var list = _unitOfWork.TaskItemRespository.Get()
                .Select(x => new TaskVM()
                {
                    ID = x.Id,
                    Name = x.ShortName,
                    LongName = x.LongName,
                    Frequency = null
                }).ToList();

            return list;
        }
        public IList<Page1Qns> GetQuestionsListPage1()
        {
            string q1 = "Taking all things together, how satisfied are you with your life as a whole?";
            string q2 = "Overall, how satisfied are you with your life at home?";
            string q3 = "Overall, how satisfied are you with your present job?";           

            string tooltip = "";

            Page1Qns q1o = new Page1Qns { ID = 1, Name = q1, LongName = null, Ans = null };
            Page1Qns q2o = new Page1Qns { ID = 2, Name = q2, LongName = null, Ans = null };
            Page1Qns q3o = new Page1Qns { ID = 3, Name = q3, LongName = null, Ans = null };   

            List<Page1Qns> list = new List<Page1Qns>();
            list.Add(q1o);
            list.Add(q2o);
            list.Add(q3o);            

            return list;
        }
        public IList<Page2Qns> GetQuestionsListPage2()
        {
            string q1 = "I receive recognition for a job well done";
            string q2 = "I feel close to the people at work";
            string q3 = "I feel good about working at this hospital";
            string q4 = "I feel secure about my job";
            string q5 = "I believe management is concerned about me";
            string q6 = "On the whole, I believe work is good for my physical health";
            string q7 = "My wages are good";
            string q8 = "All my talents and skills are used at work";
            string q9 = "I get along with supervisors";
            string q10 = "I feel good about my job";

            string q11 = "I feel safe from bullying, discrimination and harassment";
            string q12 = "I have experienced burnout due to the demands of my job";




            string tooltip = "The term 'IMMEDIATE SENIOR' refers to e.g. senior house officer, registrar, or consultant. If you are not sure who your immediate senior is: Who did you report to on your last working shift?";

            Page2Qns q1o = new Page2Qns { ID = 1, Name = q1, LongName = null, Ans = null };
            Page2Qns q2o = new Page2Qns { ID = 2, Name = q2, LongName = null, Ans = null };
            Page2Qns q3o = new Page2Qns { ID = 3, Name = q3, LongName = null, Ans = null };
            Page2Qns q4o = new Page2Qns { ID = 4, Name = q4, LongName = null, Ans = null };
            Page2Qns q5o = new Page2Qns { ID = 5, Name = q5, LongName = null, Ans = null };
            Page2Qns q6o = new Page2Qns { ID = 6, Name = q6, LongName = null, Ans = null };
            Page2Qns q7o = new Page2Qns { ID = 7, Name = q7, LongName = null, Ans = null };
            Page2Qns q8o = new Page2Qns { ID = 8, Name = q8, LongName = null, Ans = null };
            Page2Qns q9o = new Page2Qns { ID = 9, Name = q9, LongName = null, Ans = null };
            Page2Qns q10o = new Page2Qns { ID = 10, Name = q10, LongName = null, Ans = null };
            Page2Qns q11o = new Page2Qns { ID = 11, Name = q11, LongName = null, Ans = null };
            Page2Qns q12o = new Page2Qns { ID = 12, Name = q12, LongName = null, Ans = null };



            List<Page2Qns> list = new List<Page2Qns>();
            list.Add(q1o);
            list.Add(q2o);
            list.Add(q3o);
            list.Add(q4o);
            list.Add(q5o);
            list.Add(q6o);
            list.Add(q7o);
            list.Add(q8o);
            list.Add(q9o);
            list.Add(q10o);
            list.Add(q11o);
            list.Add(q12o);

            return list;
        }


        public IList<Page4Qns> GetQuestionsListPage4()
        {
            string q1 = "My clinical load";
            string q2 = "My teaching load";
            string q3 = "My independence in choosing what I work on";
            string q4 = "The time I have to do research";
            string q5 = "The quality of my work colleagues";
            string q6 = "The contribution my work makes to patient care";
            string q7 = "The contribution my work makes to work processes in my department";
            string q8 = "The amount of time I spend on managerial processes and administration";
            string q9 = "The quality of the administrative support";
            string q10 = "The degree to which my clinical work is  recognized and valued by my immediate senior";
            string q11 = "The degree to which I am consulted by my immediate senior";
            string q12 = "My current length of commute to work";
            string q13 = "The levels of staffing in my team";
            string q14 = "The number of hours on my rota outside of Monday-Friday 8am to 6pm";
            string q15 = "The management of my rota";
            string q16 = "The amount of involvement I have in decision-making at this workplace";

            string tooltip = "The term 'IMMEDIATE SENIOR' refers to e.g. senior house officer, registrar, or consultant. If you are not sure who your immediate senior is: Who did you report to on your last working shift?";

            Page4Qns q1o = new Page4Qns { ID = 1, Name = q1, LongName = null, Ans = null };
            Page4Qns q2o = new Page4Qns { ID = 2, Name = q2, LongName = null, Ans = null };
            Page4Qns q3o = new Page4Qns { ID = 3, Name = q3, LongName = null, Ans = null };
            Page4Qns q4o = new Page4Qns { ID = 4, Name = q4, LongName = null, Ans = null };
            Page4Qns q5o = new Page4Qns { ID = 5, Name = q5, LongName = null, Ans = null };
            Page4Qns q6o = new Page4Qns { ID = 6, Name = q6, LongName = null, Ans = null };
            Page4Qns q7o = new Page4Qns { ID = 7, Name = q7, LongName = null, Ans = null };
            Page4Qns q8o = new Page4Qns { ID = 8, Name = q8, LongName = null, Ans = null };
            Page4Qns q9o = new Page4Qns { ID = 9, Name = q9, LongName = null, Ans = null };
            Page4Qns q10o = new Page4Qns { ID = 10, Name = q10, LongName = tooltip, Ans = null };
            Page4Qns q11o = new Page4Qns { ID = 11, Name = q11, LongName = tooltip, Ans = null };
            Page4Qns q12o = new Page4Qns { ID = 12, Name = q12, LongName = null, Ans = null };
            Page4Qns q13o = new Page4Qns { ID = 13, Name = q13, LongName = null, Ans = null };
            Page4Qns q14o = new Page4Qns { ID = 14, Name = q14, LongName = null, Ans = null };
            Page4Qns q15o = new Page4Qns { ID = 15, Name = q15, LongName = null, Ans = null };
            Page4Qns q16o = new Page4Qns { ID = 16, Name = q16, LongName = null, Ans = null };

            List<Page4Qns> list = new List<Page4Qns>();
            list.Add(q1o);
            list.Add(q2o);
            list.Add(q3o);
            list.Add(q4o);
            list.Add(q5o);
            list.Add(q6o);
            list.Add(q7o);
            list.Add(q8o);
            list.Add(q9o);
            list.Add(q10o);
            list.Add(q11o);
            list.Add(q12o);
            list.Add(q13o);
            list.Add(q14o);
            list.Add(q15o);
            list.Add(q16o);

            return list;
        }

        public IList<Page5Qns> GetQuestionsListPage5()
        {
            string q1 = "My immediate senior understands the nature of the work I do";
            string q2 = "The feedback I receive from my immediate senior helps improve my clinical work";
            string q3 = "The feedback I receive from my immediate senior helps improve my research work";
            string q4 = "My immediate senior is a good leader";
            string q5 = "Morale/work climate within my department is excellent";
            string q6 = "I am allowed to care for patients in my own way";
            string q7 = "There is a good team relationship among the colleagues in my department";
            string q8 = "My immediate senior is a good communicator / communicates well";

            string tooltip = "The term 'IMMEDIATE SENIOR' refers to e.g. senior house officer, registrar, or consultant. If you are not sure who your immediate senior is: Who did you report to on your last working shift?";

            Page5Qns q1o = new Page5Qns { ID = 1, Name = q1, LongName = tooltip, Ans = null };
            Page5Qns q2o = new Page5Qns { ID = 2, Name = q2, LongName = tooltip, Ans = null };
            Page5Qns q3o = new Page5Qns { ID = 3, Name = q3, LongName = tooltip, Ans = null };
            Page5Qns q4o = new Page5Qns { ID = 4, Name = q4, LongName = tooltip, Ans = null };
            Page5Qns q5o = new Page5Qns { ID = 5, Name = q5, LongName = null, Ans = null };
            Page5Qns q6o = new Page5Qns { ID = 6, Name = q6, LongName = null, Ans = null };
            Page5Qns q7o = new Page5Qns { ID = 7, Name = q7, LongName = null, Ans = null };
            Page5Qns q8o = new Page5Qns { ID = 8, Name = q8, LongName = tooltip, Ans = null };

            List<Page5Qns> list = new List<Page5Qns>();
            list.Add(q1o);
            list.Add(q2o);
            list.Add(q3o);
            list.Add(q4o);
            list.Add(q5o);
            list.Add(q6o);
            list.Add(q7o);
            list.Add(q8o);


            return list;
        }

        public IList<PageContinued5Qns> GetQuestionsListPageContinued5()
        {
            string q1 = "Senior Clinicians on my team really care about my wellbeing";
            string q2 = "Nursing and Allied Health Care Staff on my team really care about my wellbeing";
            string q3 = "Management really cares about my wellbeing";            

            string tooltip = "";

            PageContinued5Qns q1o = new PageContinued5Qns { ID = 1, Name = q1, LongName = tooltip, Ans = null };
            PageContinued5Qns q2o = new PageContinued5Qns { ID = 2, Name = q2, LongName = tooltip, Ans = null };
            PageContinued5Qns q3o = new PageContinued5Qns { ID = 3, Name = q3, LongName = tooltip, Ans = null };
            

            List<PageContinued5Qns> list = new List<PageContinued5Qns>();
            list.Add(q1o);
            list.Add(q2o);
            list.Add(q3o);

            return list;
        }

        public IList<Page7Qns> GetQuestionsListPage7()
        {
            string q1 = "… communicates a clear and positive vision of the future";
            string q2 = "… treats staff as individuals, supports and encourages their development";
            string q3 = "… gives encouragement and recognition to staff";
            string q4 = "… fosters trust, involvement and co-operation among team members";
            string q5 = "… encourages thinking about problems in new ways and questions assumptions";
            string q6 = "… is clear about his/her values and practices what he/she preaches";
            string q7 = "… instills pride and respect in others and inspires me by being highly competent";
            string q8 = "… translates research evidence into high quality patient care";

            string tooltip = "The term 'IMMEDIATE SENIOR' refers to e.g. senior house officer, registrar, or consultant. If you are not sure who your immediate senior is: Who did you report to on your last working shift?";

            Page7Qns q1o = new Page7Qns { ID = 1, Name = q1, LongName = null, Ans = null };
            Page7Qns q2o = new Page7Qns { ID = 2, Name = q2, LongName = null, Ans = null };
            Page7Qns q3o = new Page7Qns { ID = 3, Name = q3, LongName = null, Ans = null };
            Page7Qns q4o = new Page7Qns { ID = 4, Name = q4, LongName = null, Ans = null };
            Page7Qns q5o = new Page7Qns { ID = 5, Name = q5, LongName = null, Ans = null };
            Page7Qns q6o = new Page7Qns { ID = 6, Name = q6, LongName = null, Ans = null };
            Page7Qns q7o = new Page7Qns { ID = 7, Name = q7, LongName = null, Ans = null };
            Page7Qns q8o = new Page7Qns { ID = 8, Name = q8, LongName = null, Ans = null };

            List<Page7Qns> list = new List<Page7Qns>();
            list.Add(q1o);
            list.Add(q2o);
            list.Add(q3o);
            list.Add(q4o);
            list.Add(q5o);
            list.Add(q6o);
            list.Add(q7o);
            list.Add(q8o);


            return list;
        }



        public IList<Page9Qns> GetQuestionsListPage9()
        {
            string q1 = "In most ways my life is close to my ideal";
            string q2 = "The conditions of my life are excellent";
            string q3 = "I am satisfied with my life";
            string q4 = "So far I have gotten the important things I want in life";
            string q5 = "If I could live my life over, I would change almost nothing";

            Page9Qns q1o = new Page9Qns { ID = 1, Name = q1, LongName = null, Ans = null };
            Page9Qns q2o = new Page9Qns { ID = 2, Name = q2, LongName = null, Ans = null };
            Page9Qns q3o = new Page9Qns { ID = 3, Name = q3, LongName = null, Ans = null };
            Page9Qns q4o = new Page9Qns { ID = 4, Name = q4, LongName = null, Ans = null };
            Page9Qns q5o = new Page9Qns { ID = 5, Name = q5, LongName = null, Ans = null };

            List<Page9Qns> list = new List<Page9Qns>();
            list.Add(q1o);
            list.Add(q2o);
            list.Add(q3o);
            list.Add(q4o);
            list.Add(q5o);
            return list;
        }


        public IList<Page10Qns> GetQuestionsListPage10()
        {
            string q1 = "- your health?";
            string q2 = "- your sleep?";
            string q3 = "- your leisure time?";

            Page10Qns q1o = new Page10Qns { ID = 1, Name = q1, LongName = null, Ans = null };
            Page10Qns q2o = new Page10Qns { ID = 2, Name = q2, LongName = null, Ans = null };
            Page10Qns q3o = new Page10Qns { ID = 3, Name = q3, LongName = null, Ans = null };

            List<Page10Qns> list = new List<Page10Qns>();
            list.Add(q1o);
            list.Add(q2o);
            list.Add(q3o);
            return list;
        }
        public IList<Page11Qptions> GetOptionsListPage11()
        {
            string option1 = "WiFi access at your place of work";
            string option2 = "Good IT support/access";
            string option3 = "Doctors mess";
            string option4 = "Access to food and drink 24hrs a day";
            string option5 = "A quiet office/space to work/study";
            string option6 = "Careers support";
            string option7 = "Library access";
            string option8 = "Other";

            string option9 = "Good communication between team members";
            string option10 = "Regular consultant presence";
            string option11 = "Supportive management";
            string option12 = "Regular team meetings";
            string option13 = "Regular feedback from colleagues on your performance";
            string option14 = "An environment in which you feel comfortable to raise concerns";
            string option15 = "Other";       

            string tooltip = "";

            Page11Qptions option1o = new Page11Qptions { ID = 0, Name = option1, LongName = null, Ans = null };
            Page11Qptions option2o = new Page11Qptions { ID = 1, Name = option2, LongName = null, Ans = null };
            Page11Qptions option3o = new Page11Qptions { ID = 2, Name = option3, LongName = null, Ans = null };
            Page11Qptions option4o = new Page11Qptions { ID = 3, Name = option4, LongName = null, Ans = null };
            Page11Qptions option5o = new Page11Qptions { ID = 4, Name = option5, LongName = null, Ans = null };
            Page11Qptions option6o = new Page11Qptions { ID = 5, Name = option6, LongName = null, Ans = null };
            Page11Qptions option7o = new Page11Qptions { ID = 6, Name = option7, LongName = null, Ans = null };
            Page11Qptions option8o = new Page11Qptions { ID = 7, Name = option8, LongName = null, Ans = null };
        
            Page11Qptions option9o = new Page11Qptions { ID = 8, Name = option9, LongName = null, Ans = null };
            Page11Qptions option10o = new Page11Qptions { ID = 9, Name = option10, LongName = null, Ans = null };
            Page11Qptions option11o = new Page11Qptions { ID = 10, Name = option11, LongName = null, Ans = null };
            Page11Qptions option12o = new Page11Qptions { ID = 11, Name = option12, LongName = null, Ans = null };
            Page11Qptions option13o = new Page11Qptions { ID = 12, Name = option13, LongName = null, Ans = null };
            Page11Qptions option14o = new Page11Qptions { ID = 13, Name = option14, LongName = null, Ans = null };
            Page11Qptions option15o = new Page11Qptions { ID = 14, Name = option15, LongName = null, Ans = null };
          
            List<Page11Qptions> list = new List<Page11Qptions>();
            list.Add(option1o);
            list.Add(option2o);
            list.Add(option3o);
            list.Add(option4o);
            list.Add(option5o);
            list.Add(option6o);
            list.Add(option7o);
            list.Add(option8o);          

            list.Add(option9o);
            list.Add(option10o);
            list.Add(option11o);
            list.Add(option12o);
            list.Add(option13o);
            list.Add(option14o);
            list.Add(option15o);

            return list;
        }
        public IList<Page11Qptions> GetExitSurveyPage11Options(int profileId)
        {
            var list = _unitOfWork.ExitSurvey_Page11_Respository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .Select(m => new Page11Qptions()
                {
                    ID = m.QnId,
                    Name = m.Options,
                    Ans = m.OtherOption
                }).ToList();

            return list;

        }
        public IList<Page12Qns> GetQuestionsListPage12()
        {
            string q1 = "Genuine interest/passion for that speciality";
            string q2 = "Opportunities for training LTFT as a trainee";
            string q3 = "Opportunities for working LTFT as a consultant/GP";

            string q4 = "The number of on-calls as a trainee";
            string q5 = "The number of on-calls as a consultant/GP";
            string q6 = "The number of anti-social hours as a trainee";

            string q7 = "The number of anti-social hours as a consultant/GP";
            string q8 = "Potential income as a consultant/GP";

            Page12Qns q1o = new Page12Qns { ID = 1, Name = q1, LongName = null, Ans = null };
            Page12Qns q2o = new Page12Qns { ID = 2, Name = q2, LongName = null, Ans = null };
            Page12Qns q3o = new Page12Qns { ID = 3, Name = q3, LongName = null, Ans = null };

            Page12Qns q4o = new Page12Qns { ID = 4, Name = q4, LongName = null, Ans = null };
            Page12Qns q5o = new Page12Qns { ID = 5, Name = q5, LongName = null, Ans = null };
            Page12Qns q6o = new Page12Qns { ID = 6, Name = q6, LongName = null, Ans = null };

            Page12Qns q7o = new Page12Qns { ID = 7, Name = q7, LongName = null, Ans = null };
            Page12Qns q8o = new Page12Qns { ID = 8, Name = q8, LongName = null, Ans = null};


            List<Page12Qns> list = new List<Page12Qns>();
            list.Add(q1o);
            list.Add(q2o);
            list.Add(q3o);

            list.Add(q4o);
            list.Add(q5o);
            list.Add(q6o);

            list.Add(q7o);
            list.Add(q8o);

            return list;
        }

        public IList<PageContinued12Qns> GetQuestionsListPageContinued12()
        {
            string q1 = "Listening/speaking to colleague’s about their experiences in that specialty";
            string q2 = "Hearing colleagues talk about it in a negative way (e"+ "\u22C5" + "g"+ "\u22C5 " + "\u2018" + "Specialty bashing" + "\u2019" +")";
            string q3 = "Your own experience of working in that specialty";
            
            PageContinued12Qns q1o = new PageContinued12Qns { ID = 1, Name = q1, LongName = null, Ans = null };
            PageContinued12Qns q2o = new PageContinued12Qns { ID = 2, Name = q2, LongName = null, Ans = null };
            PageContinued12Qns q3o = new PageContinued12Qns { ID = 3, Name = q3, LongName = null, Ans = null };
            
            List<PageContinued12Qns> list = new List<PageContinued12Qns>();
            list.Add(q1o);
            list.Add(q2o);
            list.Add(q3o);
            
            return list;
        }   

        //public ProfilePlacementDto GetProfilePlacementByProfileId(int profileId)
        //{
        //    ProfilePlacement profile = _unitOfWork.ProfilePlacementRespository
        //        .GetUsingNoTracking(x => x.ProfileId == profileId)
        //        .FirstOrDefault();

        //    if (profile != null)
        //    {
        //        return ObjectMapper.GetProfilePlacementDto(profile);
        //    }
        //    return null;
        //}
        //public ProfileTaskTimeDto GetProfileTaskTimeByProfileId(int profileId)
        //{
        //    ProfileTaskTime profile = _unitOfWork.ProfileTaskTimeRespository
        //        .GetUsingNoTracking(x => x.ProfileId == profileId)
        //        .FirstOrDefault();

        //    if (profile != null)
        //    {
        //        return ObjectMapper.GetProfileTaskTimeDto(profile);
        //    }
        //    return null;
        //}

        public ExitSurveyPage3_Dto GetExitSurveyPage3ById(int profileId)
        {
            ExitSurvey_Page3 profile = _unitOfWork.ExitSurvey_Page3_Respository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetExitSurvey_Page3Dto(profile);
            }
            return null;
        }


        public ExitSurveyPage6_Dto GetExitSurveyPage6ById(int profileId)
        {
            ExitSurvey_Page6 profile = _unitOfWork.ExitSurvey_Page6_Respository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetExitSurvey_Page6Dto(profile);
            }
            return null;
        }

        public ExitSurveyPage8_Dto GetExitSurveyPage8ById(int profileId)
        {
            ExitSurvey_Page8 profile = _unitOfWork.ExitSurvey_Page8_Respository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetExitSurvey_Page8Dto(profile);
            }
            return null;
        }



        public ExitSurveyPage11_Dto GetExitSurveyPage11ById(int profileId)
        {
            ExitSurvey_Page11 profile = _unitOfWork.ExitSurvey_Page11_Respository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetExitSurvey_Page11Dto(profile);
            }
            return null;
        }


        public ExitSurveyPage13_Dto GetExitSurveyPage13ById(int profileId)
        {
            ExitSurvey_Page13 profile = _unitOfWork.ExitSurvey_Page13_Respository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetExitSurvey_Page13Dto(profile);
            }
            return null;
        }

        public ExitSurveyPage14_Dto GetExitSurveyPage14ById(int profileId)
        {
            ExitSurvey_Page14 profile = _unitOfWork.ExitSurvey_Page14_Respository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetExitSurvey_Page14Dto(profile);
            }
            return null;
        }

        public ExitSurveyFeedback_Dto GetExitSurveyFeedbackId(int profileId)
        {
            ExitSurvey_Feedback profile = _unitOfWork.ExitSurvey_Feedback_Respository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetExitSurvey_FeedbackDto(profile);
            }
            return null;
        }
        //public ProfileTrainingDto GetProfileTrainingByProfileId(int profileId)
        //{
        //    ProfileTraining profile = _unitOfWork.ProfileTrainingRespository
        //        .GetUsingNoTracking(x => x.ProfileId == profileId)
        //        .FirstOrDefault();

        //    if (profile != null)
        //    {
        //        return ObjectMapper.GetProfileTrainingDto(profile);
        //    }
        //    return null;
        //}
        //public ProfileDemographicDto GetProfileDemographicByProfileId(int profileId)
        //{
        //    ProfileDemographic profile = _unitOfWork.ProfileDemographicRespository
        //        .GetUsingNoTracking(x => x.ProfileId == profileId)
        //        .FirstOrDefault();

        //    if (profile != null)
        //    {
        //        return ObjectMapper.GetProfileDemographicDto(profile);
        //    }
        //    return null;
        //}
        public ProfileDto GetProfileById(int id)
        {
            Profile profile = _unitOfWork.ProfileRespository.GetByID(id);
            if (profile != null)
            {
                return ObjectMapper.GetProfileDto(profile);
            }
            return null;
        }
        //public async Task<ProfileDto> GetProfileByIdAsync(int id)
        //{
        //    Profile profile = await _unitOfWork.ProfileRespository.GetByIDAsync(id);
        //    if (profile != null)
        //    {
        //        return ObjectMapper.GetProfileDto(profile);
        //    }
        //    return null;
        //}
        //public ProfileDto GetProfileByUid(string uid)
        //{
        //    var profile = _unitOfWork.ProfileRespository
        //        .GetUsingNoTracking(m => m.Uid == uid)
        //        .FirstOrDefault();

        //    if (profile != null)
        //    {
        //        return ObjectMapper.GetProfileDto(profile);
        //    }
        //    return null;
        //}
        //public ProfileDto GetProfileByLoginEmailAndUid(string loginEmail, string uid)
        //{
        //    loginEmail = StringCipher.EncryptRfc2898(loginEmail);

        //    var profile = _unitOfWork.ProfileRespository
        //                        .GetUsingNoTracking(m => (m.LoginEmail == loginEmail) && (m.Uid == uid))
        //                        .FirstOrDefault();
        //    if (profile != null)
        //    {
        //        return ObjectMapper.GetProfileDto(profile);
        //    }
        //    return null;
        //}

        public ProfileDto GetProfileByLoginEmail(string loginEmail)
        {
            loginEmail = StringCipher.EncryptRfc2898(loginEmail);

            var profile = _unitOfWork.ProfileRespository
                .GetUsingNoTracking(m => m.LoginEmail == loginEmail)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetProfileDto(profile);
            }

            return null;
        }

        //public RedirectionUrlDto CheckRegistrationAtLogin(string loginEmail, out int profileId)
        //{

        //    ProfileDto profile = GetProfileByLoginEmail(loginEmail);
        //    if (profile != null)
        //    {
        //        string currRegProgressNext = profile.RegistrationProgressNext;
        //        profileId = profile.Id;

        //        if (currRegProgressNext != Constants.StatusRegistrationProgress.Completed.ToString())
        //        {
        //            if (currRegProgressNext == Constants.StatusRegistrationProgress.Invited.ToString())
        //            {
        //                return new RedirectionUrlDto { Action = "NeedToRegister", Controller = "Register" };
        //            }

        //            if (currRegProgressNext == Constants.StatusRegistrationProgress.Signup.ToString())
        //            {
        //                return new RedirectionUrlDto { Action = "Signup", Controller = "Account" };
        //            }

        //            return new RedirectionUrlDto { Action = currRegProgressNext, Controller = "Register" };
        //        }
        //        return new RedirectionUrlDto { Action = "Login", Controller = "Account" };
        //    }
        //    else
        //    {
        //        profileId = 0;
        //        return null;
        //    }


        //}
        //public bool RedirectConsentOnAgree(string uid)
        //{
        //    var profile = GetProfileByUid(uid);

        //    if (profile != null)
        //    {
        //        profile.RegistrationProgressNext = Constants.StatusRegistrationProgress.Signup.ToString();
        //        UpdateProfile(profile);
        //        return true;
        //    }

        //    return false;
        //}
        //public bool RedirectConsentOnDisagree(string uid)
        //{
        //    var profile = GetProfileByUid(uid);
        //    if (profile != null)
        //    {
        //        profile.RegistrationProgressNext = Constants.StatusRegistrationProgress.DNP.ToString();
        //        UpdateProfile(profile);
        //        return true;

        //    }

        //    return false;
        //}
        //public bool IsEmailValid(string signupEmail)
        //{
        //    int numOfEmails = GetSameNumberOfEmails(signupEmail);
        //    if (numOfEmails < 2)
        //    {
        //        return true;
        //    }

        //    return false;
        //}
        //public bool IsRegisteredProfile(string signupEmail)
        //{
        //    ProfileDto profile = GetProfileByLoginEmail(signupEmail);

        //    if (profile != null)
        //    {
        //        return true;
        //    }

        //    return false;
        //}
        //public bool IsRegistrationCompleted(string signupEmail, string uid)
        //{

        //    ProfileDto profile = GetProfileByLoginEmailAndUid(signupEmail, uid);

        //    if (profile != null)
        //    {
        //        if (profile.RegistrationProgressNext == Constants.StatusRegistrationProgress.DNP.ToString())
        //        {
        //            return true;

        //        }
        //    }
        //    return false;
        //}
        //public ProfileDto SignupOnExit(string signupEmail, string uid, string userId)
        //{
        //    ProfileDto profile = GetProfileByLoginEmailAndUid(signupEmail, uid);
        //    if (profile != null)
        //    {
        //        string newStatus = profile.RegistrationProgressNext;
        //        profile.UserId = userId;

        //        if (profile.RegistrationProgressNext == Constants.StatusRegistrationProgress.Signup.ToString())
        //        {
        //            newStatus = Constants.StatusRegistrationProgress.Wellbeing.ToString();
        //        }

        //        profile.RegistrationProgressNext = newStatus;

        //        UpdateProfile(profile);

        //        return profile;
        //    }

        //    return null;
        //}
        //public RedirectionUrlDto ValidateConsentOnExit(string uid)
        //{
        //    if (string.IsNullOrEmpty(uid))
        //    {
        //        return new RedirectionUrlDto { Action = "Login", Controller = "Account" };
        //    }

        //    var profile = GetProfileByUid(uid);

        //    if (profile != null)
        //    {
        //        string currRegStatus = profile.RegistrationProgressNext;

        //        if (currRegStatus == Constants.StatusRegistrationProgress.DNP.ToString())
        //        {
        //            return new RedirectionUrlDto { Action = "DNP", Controller = "Register" };
        //        }
        //        else if (
        //            currRegStatus == Constants.StatusRegistrationProgress.Completed.ToString() ||
        //            currRegStatus == Constants.StatusRegistrationProgress.Task.ToString() ||
        //            currRegStatus == Constants.StatusRegistrationProgress.Demographics.ToString() ||
        //            currRegStatus == Constants.StatusRegistrationProgress.Roster.ToString() ||
        //            currRegStatus == Constants.StatusRegistrationProgress.Wellbeing.ToString() ||
        //            currRegStatus == Constants.StatusRegistrationProgress.Screening.ToString())

        //        {
        //            return new RedirectionUrlDto { Action = "Login", Controller = "Account" };

        //        }

        //        return new RedirectionUrlDto { Action = "Signup", Controller = "Account" };


        //    }

        //    return null;
        //}
        //public RedirectionUrlDto ValidateConsentOnEntry(string uid)
        //{
        //    if (string.IsNullOrEmpty(uid))
        //    {
        //        return new RedirectionUrlDto { Action = "Login", Controller = "Account" };
        //    }

        //    var profile = GetProfileByUid(uid);

        //    if (profile != null)
        //    {
        //        string currRegStatus = profile.RegistrationProgressNext;

        //        if (currRegStatus == Constants.StatusRegistrationProgress.DNP.ToString())
        //        {
        //            return new RedirectionUrlDto { Action = "DNP", Controller = "Register" };
        //        }
        //        else if (
        //            currRegStatus == Constants.StatusRegistrationProgress.Completed.ToString() ||
        //            currRegStatus == Constants.StatusRegistrationProgress.Task.ToString() ||
        //            currRegStatus == Constants.StatusRegistrationProgress.Demographics.ToString() ||
        //            currRegStatus == Constants.StatusRegistrationProgress.Roster.ToString() ||
        //            currRegStatus == Constants.StatusRegistrationProgress.Wellbeing.ToString() ||
        //            currRegStatus == Constants.StatusRegistrationProgress.Screening.ToString())

        //        {
        //            return new RedirectionUrlDto { Action = "Login", Controller = "Account" };

        //        }
        //    }

        //    return null;
        //}
        //public int GetSameNumberOfEmails(string loginEmail)
        //{
        //    loginEmail = StringCipher.EncryptRfc2898(loginEmail);

        //    return
        //        _unitOfWork.ProfileRespository
        //        .GetUsingNoTracking(m => m.LoginEmail == loginEmail)
        //        .Count();
        //}
        //public int ValidateRoster(int profileId)
        //{
        //    /*

        //     Logic checked in Controller:
        //            1) If the total number of slots is less than 1, the calendar is empty
        //            2) If the total number of slots is less than 3, the calendar is incomplete
        //            3) Else the calendar is valid
        //     */

        //    return
        //      _unitOfWork.ProfileRosterRespository
        //      .GetUsingNoTracking(m => (m.ProfileId == profileId))
        //      .Count();
        //}

        //public async Task<int> NoOfSameProfileEmailAsync(string loginEmail)
        //{
        //    //TODO: Need to implement
        //    return 0;
        //}
        //public async Task<ProfileDto> GetProfileByUidAsync(string uid)
        //{
        //    //TODO: Need to implement
        //    return null;
        //}
        public void UpdateProfile(ProfileDto p)
        {
            Profile e = ObjectMapper.GetProfileEntity(p);
            _unitOfWork.ProfileRespository.Update(e);
            _unitOfWork.SaveChanges();
        }
        //public async Task SaveScreening(RegisterScreeningViewModel v)
        //{

        //    if (v.CurrentLevelOfTraining == "Not in training" || v.IsCurrentPlacement == "No")
        //    {
        //        Profile profile = _unitOfWork.ProfileRespository.GetByID(v.ProfileId);

        //        if (profile != null)
        //        {
        //            profile.CurrentLevelOfTraining = v.CurrentLevelOfTraining;
        //            profile.IsCurrentPlacement = v.IsCurrentPlacement;
        //            profile.RegistrationProgressNext = StatusRegistrationProgress.ScreenedOut.ToString();
        //            profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
        //            profile.MaxStep = v.MaxStep;
        //            _unitOfWork.ProfileRespository.Update(profile);
        //            _unitOfWork.SaveChanges();
        //        }
        //    }
        //    else
        //    {
        //        Profile profile = _unitOfWork.ProfileRespository.GetByID(v.ProfileId);
        //        profile.CurrentLevelOfTraining = v.CurrentLevelOfTraining;
        //        profile.IsCurrentPlacement = v.IsCurrentPlacement;
        //        profile.RegistrationProgressNext = StatusRegistrationProgress.Signup.ToString();
        //        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
        //        profile.MaxStep = v.MaxStep;
        //        if (profile.MaxStep < 1)
        //        {
        //            profile.MaxStep = 1;
        //        }


        //        _unitOfWork.ProfileRespository.Update(profile);
        //        _unitOfWork.SaveChanges();

        //    }


        //}
        //public bool SaveScreeningByUid(RegisterScreeningViewModel v)
        //{
        //    //MaxStep = 2

        //    if (v.CurrentLevelOfTraining == "Not in training" || v.IsCurrentPlacement == "No")
        //    {
        //        Profile profile = _unitOfWork.ProfileRespository.GetByID(v.ProfileId);




        //        profile.CurrentLevelOfTraining = v.CurrentLevelOfTraining;
        //        profile.IsCurrentPlacement = v.IsCurrentPlacement;
        //        profile.RegistrationProgressNext = StatusRegistrationProgress.ScreenedOut.ToString();
        //        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
        //        profile.MaxStep = v.MaxStep;

        //        _unitOfWork.ProfileRespository.Update(profile);
        //        _unitOfWork.SaveChanges();

        //        return false;
        //    }
        //    else
        //    {
        //        Profile profile = _unitOfWork.ProfileRespository.GetByID(v.ProfileId);
        //        profile.CurrentLevelOfTraining = v.CurrentLevelOfTraining;
        //        profile.IsCurrentPlacement = v.IsCurrentPlacement;
        //        profile.RegistrationProgressNext = StatusRegistrationProgress.Wellbeing.ToString();
        //        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
        //        profile.MaxStep = v.MaxStep;
        //        if (profile.MaxStep < 2)
        //        {
        //            profile.MaxStep = 2;
        //        }
        //        _unitOfWork.ProfileRespository.Update(profile);
        //        _unitOfWork.SaveChanges();

        //        return true;
        //    }


        //}       


        //public void SaveTaskTimes(ProfileTaskTimeDto d)
        //{
        //    var list = _unitOfWork.ProfileTaskTimeRespository.Get(p => p.ProfileId == d.ProfileId);
        //    _unitOfWork.ProfileTaskTimeRespository.RemoveRange(list);

        //    var e = ObjectMapper.GetProfileTaskTimeEntity(d);
        //    _unitOfWork.ProfileTaskTimeRespository.Insert(e);
        //    _unitOfWork.SaveChanges();
        //}
        //public void SavePlacement(ProfilePlacementDto d)
        //{
        //    var list = _unitOfWork.ProfilePlacementRespository.Get(p => p.ProfileId == d.ProfileId);
        //    _unitOfWork.ProfilePlacementRespository.RemoveRange(list);

        //    var e = ObjectMapper.GetProfilePlacementEntity(d);
        //    _unitOfWork.ProfilePlacementRespository.Insert(e);
        //    _unitOfWork.SaveChanges();
        //}


        public async Task Save_Page3(ExitSurveyPage3_Dto d)
        {
            var list = _unitOfWork.ExitSurvey_Page3_Respository.Get(p => p.ProfileId == d.ProfileId);
            _unitOfWork.ExitSurvey_Page3_Respository.RemoveRange(list);
            var e = ObjectMapper.GetExitSurvey_Page3_Entity(d);
            _unitOfWork.ExitSurvey_Page3_Respository.Insert(e);
            _unitOfWork.SaveChanges();
        }

        public async Task Save_Page6(ExitSurveyPage6_Dto d)
        {
            var list = _unitOfWork.ExitSurvey_Page6_Respository.Get(p => p.ProfileId == d.ProfileId);
            _unitOfWork.ExitSurvey_Page6_Respository.RemoveRange(list);
            var e = ObjectMapper.GetExitSurvey_Page6_Entity(d);
            _unitOfWork.ExitSurvey_Page6_Respository.Insert(e);
            _unitOfWork.SaveChanges();
        }

        public async Task Save_Page8(ExitSurveyPage8_Dto d)
        {
            var list = _unitOfWork.ExitSurvey_Page8_Respository.Get(p => p.ProfileId == d.ProfileId);
            _unitOfWork.ExitSurvey_Page8_Respository.RemoveRange(list);
            var e = ObjectMapper.GetExitSurvey_Page8_Entity(d);
            _unitOfWork.ExitSurvey_Page8_Respository.Insert(e);
            _unitOfWork.SaveChanges();
        }

        //public void SaveComment(ProfileCommentDto d)
        //{
        //    var list = _unitOfWork.ProfileCommentRespository.Get(p => p.ProfileId == d.ProfileId);
        //    _unitOfWork.ProfileCommentRespository.RemoveRange(list);

        //    var e = ObjectMapper.GetProfileCommentEntity(d);
        //    _unitOfWork.ProfileCommentRespository.Insert(e);
        //    _unitOfWork.SaveChanges();
        //}
        //public void SaveSpecialty(RegisterSpecialtyViewModel v)
        //{

        //    Profile profile = _unitOfWork.ProfileRespository.GetByID(v.ProfileId);

        //    profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
        //    profile.RegistrationProgressNext = StatusRegistrationProgress.Training.ToString();
        //    profile.MaxStep = v.MaxStep;

        //    if (profile.MaxStep < 7)
        //    {
        //        profile.MaxStep = 7;
        //    }
        //    _unitOfWork.ProfileRespository.Update(profile);
        //    _unitOfWork.SaveChanges();
        //}
        //public void SaveTraining(ProfileTrainingDto d)
        //{
        //    var list = _unitOfWork.ProfileTrainingRespository.Get(p => p.ProfileId == d.ProfileId);
        //    _unitOfWork.ProfileTrainingRespository.RemoveRange(list);

        //    var e = ObjectMapper.GetProfileTrainingEntity(d);
        //    _unitOfWork.ProfileTrainingRespository.Insert(e);
        //    _unitOfWork.SaveChanges();

        //}
        //public void SaveDemographics(ProfileDemographicDto d)
        //{
        //    var list = _unitOfWork.ProfileDemographicRespository.Get(p => p.ProfileId == d.ProfileId);
        //    _unitOfWork.ProfileDemographicRespository.RemoveRange(list);

        //    var e = ObjectMapper.GetProfileDemographicEntity(d);
        //    _unitOfWork.ProfileDemographicRespository.Insert(e);
        //    _unitOfWork.SaveChanges();

        //}
        public void Dispose()
        {
            _unitOfWork.Dispose();
        }
        //public async Task SaveProfileSpecialtyAsync(int profileId, int[] selected, string otherText)
        //{
        //    await Task.Run(() =>
        //    {
        //        var list = _unitOfWork.ProfileSpecialtiesRespository.Get(p => p.ProfileId == profileId);

        //        _unitOfWork.ProfileSpecialtiesRespository.RemoveRange(list);
        //        _unitOfWork.SaveChanges();


        //        foreach (var r in selected)
        //        {

        //            var e = new ProfileSpecialty
        //            {
        //                ProfileId = profileId,
        //                SpecialtyId = r,
        //                OtherText = otherText
        //            };
        //            _unitOfWork.ProfileSpecialtiesRespository.Insert(e);


        //        }
        //        _unitOfWork.SaveChanges();
        //    });
        //}
        public async Task Save_Page1(int profileId, IList<Page1Qns> selectedQnsPage1)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.ExitSurvey_Page1_Respository.Get(p => p.ProfileId == profileId);

                _unitOfWork.ExitSurvey_Page1_Respository.RemoveRange(list);
                _unitOfWork.SaveChanges();


                foreach (var r in selectedQnsPage1)
                {
                    if (!string.IsNullOrEmpty(r.Ans))
                    {
                        var newDefaultTask = new ExitSurvey_Page1
                        {
                            ProfileId = profileId,
                            Qn = r.Name,
                            Ans = r.Ans
                        };
                        _unitOfWork.ExitSurvey_Page1_Respository.Insert(newDefaultTask);
                    }

                }
                _unitOfWork.SaveChanges();
            });
        }
        public async Task Save_Page2(int profileId, IList<Page2Qns> selectedQnsPage2)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.ExitSurvey_Page2_Respository.Get(p => p.ProfileId == profileId);

                _unitOfWork.ExitSurvey_Page2_Respository.RemoveRange(list);
                _unitOfWork.SaveChanges();


                foreach (var r in selectedQnsPage2)
                {
                    if (!string.IsNullOrEmpty(r.Ans))
                    {
                        var newDefaultTask = new ExitSurvey_Page2
                        {
                            ProfileId = profileId,
                            //TaskItemId = r.ID,
                            Qn = r.Name,
                            Ans = r.Ans
                        };
                        _unitOfWork.ExitSurvey_Page2_Respository.Insert(newDefaultTask);
                    }

                }
                _unitOfWork.SaveChanges();
            });
        }


        //public async Task CreateUserFeedback(int profileId, string baseUrl, EmailFeedbackViewModel v)
        //{
        //    //add a record to db

        //    Feedback e = new Feedback();
        //    e.Channel = "Register";
        //    e.CreatedDateTimeUtc = DateTime.UtcNow;

        //    e.Email = v.EmailAddress;
        //    e.ContactNumber = v.PhoneNumber;
        //    e.PreferedContact = v.PreferedContact;
        //    e.PreferedTime = v.PreferedTime;
        //    e.Message = v.Message;
        //    e.ProfileId = profileId;

        //    _unitOfWork.FeedbackRespository.Insert(e);
        //    _unitOfWork.SaveChanges();

        //}


        public async Task Save_Page4(int profileId, IList<Page4Qns> selectedQnsPage4)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.ExitSurvey_Page4_Respository.Get(p => p.ProfileId == profileId);

                _unitOfWork.ExitSurvey_Page4_Respository.RemoveRange(list);
                _unitOfWork.SaveChanges();


                foreach (var r in selectedQnsPage4)
                {
                    if (!string.IsNullOrEmpty(r.Ans))
                    {
                        var newDefaultTask = new ExitSurvey_Page4
                        {
                            ProfileId = profileId,
                            //TaskItemId = r.ID,
                            Qn = r.Name,
                            Ans = r.Ans
                        };
                        _unitOfWork.ExitSurvey_Page4_Respository.Insert(newDefaultTask);
                    }

                }
                _unitOfWork.SaveChanges();
            });
        }

        public async Task Save_Page5(int profileId, IList<Page5Qns> selectedQnsPage5)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.ExitSurvey_Page5_Respository.Get(p => p.ProfileId == profileId);

                _unitOfWork.ExitSurvey_Page5_Respository.RemoveRange(list);
                _unitOfWork.SaveChanges();


                foreach (var r in selectedQnsPage5)
                {
                    if (!string.IsNullOrEmpty(r.Ans))
                    {
                        var newDefaultTask = new ExitSurvey_Page5
                        {
                            ProfileId = profileId,
                            //TaskItemId = r.ID,
                            Qn = r.Name,
                            Ans = r.Ans
                        };
                        _unitOfWork.ExitSurvey_Page5_Respository.Insert(newDefaultTask);
                    }

                }
                _unitOfWork.SaveChanges();
            });
        }
        public async Task Save_PageContinued5(int profileId, IList<PageContinued5Qns> selectedQnsPageContinued5)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.ExitSurvey_PageContinued5_Respository.Get(p => p.ProfileId == profileId);

                _unitOfWork.ExitSurvey_PageContinued5_Respository.RemoveRange(list);
                _unitOfWork.SaveChanges();


                foreach (var r in selectedQnsPageContinued5)
                {
                    if (!string.IsNullOrEmpty(r.Ans))
                    {
                        var newDefaultTask = new ExitSurvey_PageContinued5
                        {
                            ProfileId = profileId,                            
                            Qn = r.Name,
                            Ans = r.Ans
                        };
                       _unitOfWork.ExitSurvey_PageContinued5_Respository.Insert(newDefaultTask);
                    }

                }
                _unitOfWork.SaveChanges();
            });
        }

        public async Task Save_Page7(int profileId, IList<Page7Qns> selectedQnsPage7)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.ExitSurvey_Page7_Respository.Get(p => p.ProfileId == profileId);

                _unitOfWork.ExitSurvey_Page7_Respository.RemoveRange(list);
                _unitOfWork.SaveChanges();


                foreach (var r in selectedQnsPage7)
                {
                    if (!string.IsNullOrEmpty(r.Ans))
                    {
                        var newDefaultTask = new ExitSurvey_Page7
                        {
                            ProfileId = profileId,
                            //TaskItemId = r.ID,
                            Qn = r.Name,
                            Ans = r.Ans
                        };
                        _unitOfWork.ExitSurvey_Page7_Respository.Insert(newDefaultTask);
                    }

                }
                _unitOfWork.SaveChanges();
            });
        }


        public async Task Save_Page9(int profileId, IList<Page9Qns> selectedQnsPage9)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.ExitSurvey_Page9_Respository.Get(p => p.ProfileId == profileId);

                _unitOfWork.ExitSurvey_Page9_Respository.RemoveRange(list);
                _unitOfWork.SaveChanges();


                foreach (var r in selectedQnsPage9)
                {
                    if (!string.IsNullOrEmpty(r.Ans))
                    {
                        var newDefaultTask = new ExitSurvey_Page9
                        {
                            ProfileId = profileId,
                            Qn = r.Name,
                            Ans = r.Ans
                        };
                        _unitOfWork.ExitSurvey_Page9_Respository.Insert(newDefaultTask);
                    }

                }
                _unitOfWork.SaveChanges();
            });
        }


        public async Task Save_Page10(int profileId, IList<Page10Qns> selectedQnsPage10)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.ExitSurvey_Page10_Respository.Get(p => p.ProfileId == profileId);

                _unitOfWork.ExitSurvey_Page10_Respository.RemoveRange(list);
                _unitOfWork.SaveChanges();


                foreach (var r in selectedQnsPage10)
                {
                    if (!string.IsNullOrEmpty(r.Ans))
                    {
                        var newDefaultTask = new ExitSurvey_Page10
                        {
                            ProfileId = profileId,
                            Qn = r.Name,
                            Ans = r.Ans
                        };
                        _unitOfWork.ExitSurvey_Page10_Respository.Insert(newDefaultTask);
                    }

                }
                _unitOfWork.SaveChanges();
            });
        }

        //public async Task Save_Page11(ExitSurveyPage11_Dto d)
        //{
        //    var list = _unitOfWork.ExitSurvey_Page11_Respository.Get(p => p.ProfileId == d.ProfileId);
        //    _unitOfWork.ExitSurvey_Page11_Respository.RemoveRange(list);
        //    var e = ObjectMapper.GetExitSurvey_Page11_Entity(d);
        //    _unitOfWork.ExitSurvey_Page11_Respository.Insert(e);
        //    _unitOfWork.SaveChanges();
        //}
        public async Task Save_Page11(int profileId, IList<Page11Qptions> selectedOptionsPage11)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.ExitSurvey_Page11_Respository.Get(p => p.ProfileId == profileId);

                _unitOfWork.ExitSurvey_Page11_Respository.RemoveRange(list);
                _unitOfWork.SaveChanges();


                foreach (var r in selectedOptionsPage11)
                {
                    if (!string.IsNullOrEmpty(r.ID.ToString()))
                    {
                        var newDefaultTask = new ExitSurvey_Page11
                        {
                            ProfileId = profileId,
                            QnId = r.ID,
                            Options = r.Name,
                            //OtherOption = r.Name
                            OtherOption = r.LongName
                        };
                        _unitOfWork.ExitSurvey_Page11_Respository.Insert(newDefaultTask);
                    }

                }
                _unitOfWork.SaveChanges();
            });
        }
        public async Task Save_Page12(int profileId, IList<Page12Qns> selectedQnsPage12)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.ExitSurvey_Page12_Respository.Get(p => p.ProfileId == profileId);

                _unitOfWork.ExitSurvey_Page12_Respository.RemoveRange(list);
                _unitOfWork.SaveChanges();


                foreach (var r in selectedQnsPage12)
                {
                    if (!string.IsNullOrEmpty(r.Ans))
                    {
                        var newDefaultTask = new ExitSurvey_Page12
                        {
                            ProfileId = profileId,
                            Qn = r.Name,
                            Ans = r.Ans
                        };
                        _unitOfWork.ExitSurvey_Page12_Respository.Insert(newDefaultTask);
                    }

                }
                _unitOfWork.SaveChanges();
            });
        }
        public async Task Save_PageContinued12(int profileId, IList<PageContinued12Qns> selectedQnsPageContinued12)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.ExitSurvey_PageContinued12_Respository.Get(p => p.ProfileId == profileId);

                _unitOfWork.ExitSurvey_PageContinued12_Respository.RemoveRange(list);
                _unitOfWork.SaveChanges();


                foreach (var r in selectedQnsPageContinued12)
                {
                    if (!string.IsNullOrEmpty(r.Ans))
                    {
                        var newDefaultTask = new ExitSurvey_PageContinued12
                        {
                            ProfileId = profileId,
                            Qn = r.Name,
                            Ans = r.Ans
                        };
                       _unitOfWork.ExitSurvey_PageContinued12_Respository.Insert(newDefaultTask);
                    }

                }
                _unitOfWork.SaveChanges();
            });
        }

        public async Task Save_Page13(ExitSurveyPage13_Dto d)
        {
            var list = _unitOfWork.ExitSurvey_Page13_Respository.Get(p => p.ProfileId == d.ProfileId);
            _unitOfWork.ExitSurvey_Page13_Respository.RemoveRange(list);
            var e = ObjectMapper.GetExitSurvey_Page13_Entity(d);
            _unitOfWork.ExitSurvey_Page13_Respository.Insert(e);
            _unitOfWork.SaveChanges();
        }


        public async Task Save_Page14(ExitSurveyPage14_Dto d)
        {
            var list = _unitOfWork.ExitSurvey_Page14_Respository.Get(p => p.ProfileId == d.ProfileId);
            _unitOfWork.ExitSurvey_Page14_Respository.RemoveRange(list);
            var e = ObjectMapper.GetExitSurvey_Page14_Entity(d);
            _unitOfWork.ExitSurvey_Page14_Respository.Insert(e);
            _unitOfWork.SaveChanges();
        }
        public async Task Save_Feedback(ExitSurveyFeedback_Dto d)
        {
            var list = _unitOfWork.ExitSurvey_Feedback_Respository.Get(p => p.ProfileId == d.ProfileId);
            _unitOfWork.ExitSurvey_Feedback_Respository.RemoveRange(list);
            var e = ObjectMapper.GetExitSurvey_Feedback_Entity(d);
            _unitOfWork.ExitSurvey_Feedback_Respository.Insert(e);
            _unitOfWork.SaveChanges();
        }

    }
}