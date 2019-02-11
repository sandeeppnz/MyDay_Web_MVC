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
    public class ExitV2Service
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private static bool UpdateDatabase = true;
        private ProfileService profileService;

        public ExitV2Service()
        {
            this.profileService = new ProfileService();
        }

        public ProfileDto GetProfileById(int id)
        {
            Profile profile = _unitOfWork.ProfileRespository.GetByID(id);
            if (profile != null)
            {
                return ObjectMapper.GetProfileDto(profile);
            }
            return null;
        }
        public Profile GetExitProfile(int profileId)
        {
            Profile profile = _unitOfWork.ProfileRespository.GetByID(profileId);
            return profile;
        }
        public void UpdateExitProfile(Profile r)
        {
            _unitOfWork.ProfileRespository.Update(r);
            _unitOfWork.SaveChanges();
        }
        public ExitV2Survey GetExitSurvey(int surveyId)
        {
            ExitV2Survey survey = _unitOfWork.ExitV2SurveyRepository.GetByID(surveyId);
            return survey;
        }
        public void UpdateExitSurvey(ExitV2Survey r)
        {
            _unitOfWork.ExitV2SurveyRepository.Update(r);
            _unitOfWork.SaveChanges();
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
        public void UpdateProfile(ProfileDto p)
        {
            Profile e = ObjectMapper.GetProfileEntity(p);
            _unitOfWork.ProfileRespository.Update(e);
            _unitOfWork.SaveChanges();
        }
        public IList<QuestionAnswer> GetQuestionsListWellBeing()
        {
            string q1 = "Overall, how satisfied are you with your present job?";
            string q2 = "Overall, how satisfied are you with your life at home?";
            string q3 = "Taking all things together, how satisfied are you with your life as a whole?";

            string tooltip = "";

            QuestionAnswer q1o = new QuestionAnswer { ID = 1, Name = q1, LongName = null, Ans = null };
            QuestionAnswer q2o = new QuestionAnswer { ID = 2, Name = q2, LongName = null, Ans = null };
            QuestionAnswer q3o = new QuestionAnswer { ID = 3, Name = q3, LongName = null, Ans = null };

            List<QuestionAnswer> list = new List<QuestionAnswer>();
            list.Add(q1o);
            list.Add(q2o);
            list.Add(q3o);

            return list;
        }
        public IList<QuestionAnswer> GetExitSurveyAnsWellbeing(int profileId, int exitSurveyId)
        {
            var list = _unitOfWork.WellBeingRepository
                                  .GetUsingNoTracking(x => x.ProfileId == profileId
                                                        && x.ExitSurveyId == exitSurveyId)
                                  .Select(m => new QuestionAnswer()
                                  {
                                        ID = m.Id,
                                        Name = m.Qn,
                                        Ans = m.Ans
                                  }).ToList();

            return list;
        }
        public async Task SaveWellBeing(int profileId, int exitSurveyId,IList<QuestionAnswer> selectedQns)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.WellBeingRepository.Get(p => p.ProfileId == profileId 
                                                                    && p.ExitSurveyId == exitSurveyId);

                _unitOfWork.WellBeingRepository.RemoveRange(list);
                _unitOfWork.SaveChanges();

                foreach (var r in selectedQns)
                {
                    if (!string.IsNullOrEmpty(r.Ans))
                    {
                        var newDefaultTask = new WellBeing
                        {
                            ProfileId = profileId,
                            ExitSurveyId = exitSurveyId,
                            Qn = r.Name,
                            Ans = r.Ans                            
                        };
                        _unitOfWork.WellBeingRepository.Insert(newDefaultTask);
                    }
                }
                _unitOfWork.SaveChanges();
            });
        }
        public IList<QuestionAnswer> GetQuestionsFirstJob()
        {
            string q1 = "I receive recognition for a job well done";
            string q2 = "I feel close to the people at work";
            string q3 = "I feel good about working at this hospital";
            string q4 = "I feel secure about my job";
            string q5 = "I believe management is concerned about me";
            string q6 = "On the whole, I believe work is good for my physical health";
            string q7 = "My wages are good";
            string q8 = "All my talents and skills are used at work";
            string q9 = "I get along with senior colleagues";
            string q10 = "I feel good about my job";

            string q11 = "I feel safe from bullying, discrimination and harassment";
            string q12 = "I have experienced burnout due to the demands of my job";

            string tooltip = "The term 'IMMEDIATE SENIOR' refers to e.g. senior house officer, registrar, or consultant. If you are not sure who your immediate senior is: Who did you report to on your last working shift?";

            QuestionAnswer q1o = new QuestionAnswer { ID = 1, Name = q1, LongName = null, Ans = null };
            QuestionAnswer q2o = new QuestionAnswer { ID = 2, Name = q2, LongName = null, Ans = null };
            QuestionAnswer q3o = new QuestionAnswer { ID = 3, Name = q3, LongName = null, Ans = null };
            QuestionAnswer q4o = new QuestionAnswer { ID = 4, Name = q4, LongName = null, Ans = null };
            QuestionAnswer q5o = new QuestionAnswer { ID = 5, Name = q5, LongName = null, Ans = null };
            QuestionAnswer q6o = new QuestionAnswer { ID = 6, Name = q6, LongName = null, Ans = null };
            QuestionAnswer q7o = new QuestionAnswer { ID = 7, Name = q7, LongName = null, Ans = null };
            QuestionAnswer q8o = new QuestionAnswer { ID = 8, Name = q8, LongName = null, Ans = null };
            QuestionAnswer q9o = new QuestionAnswer { ID = 9, Name = q9, LongName = null, Ans = null };
            QuestionAnswer q10o = new QuestionAnswer { ID = 10, Name = q10, LongName = null, Ans = null };
            QuestionAnswer q11o = new QuestionAnswer { ID = 11, Name = q11, LongName = null, Ans = null };
            QuestionAnswer q12o = new QuestionAnswer { ID = 12, Name = q12, LongName = null, Ans = null };

            List<QuestionAnswer> list = new List<QuestionAnswer>();
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
        public IList<QuestionAnswer> GetAnsFirstJob(int profileId, int exitSurveyId)
        {
            var list = _unitOfWork.FirstJobRepository.GetUsingNoTracking(x => x.ProfileId == profileId
                                                                            && x.ExitSurveyId == exitSurveyId)
                                    .Select(m => new QuestionAnswer()
                                    {
                                        ID = m.Id,
                                        Name = m.Qn,
                                        Ans = m.Ans
                                    }).ToList();

            return list;
        }
        public async Task SaveFirstJob(int profileId, int exitSurveyId, IList<QuestionAnswer> selectedQns)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.FirstJobRepository.Get(p => p.ProfileId == profileId
                                                                 && p.ExitSurveyId == exitSurveyId);
                           _unitOfWork.FirstJobRepository.RemoveRange(list);
                           _unitOfWork.SaveChanges();

                    foreach (var r in selectedQns)
                    {
                        if (!string.IsNullOrEmpty(r.Ans))
                        {
                            var newDefaultTask = new FirstJob
                            {
                                ProfileId = profileId,
                                ExitSurveyId = exitSurveyId,
                                Qn = r.Name,
                                Ans = r.Ans
                            };
                            _unitOfWork.FirstJobRepository.Insert(newDefaultTask);
                        }
                    }
                    _unitOfWork.SaveChanges();
            });
        }

        public SecondJobDto GetSecondJobById(int profileId, int exitSurveyId)
        {
            SecondJob profile = _unitOfWork.SecondJobRepository
                                           .GetUsingNoTracking(x => x.ProfileId == profileId
                                                                    && x.ExitSurveyId == exitSurveyId)
                                           .FirstOrDefault();
            if (profile != null)
            { return ObjectMapper.GetSecondJobDto(profile); }

            return null;
        }
        public async Task SaveSecondJob(SecondJobDto d)
        {
            var list = _unitOfWork.SecondJobRepository
                                  .Get(p => p.ProfileId == d.ProfileId
                                            && p.ExitSurveyId == d.ExitSurveyId);
                       _unitOfWork.SecondJobRepository.RemoveRange(list);

            var e = ObjectMapper.GetSecondJobEntity(d);
                    _unitOfWork.SecondJobRepository.Insert(e);
                    _unitOfWork.SaveChanges();
        }
        public IList<QuestionAnswer> GetQuestionsThirdJob()
        {   
            string q1 = "The contribution my work makes to patient care";
            string q2 = "The contribution my work makes to work processes in my department";
            string q3 = "The amount of time I spend on managerial processes and administration";
            string q4 = "The quality of the administrative support";
            string q5 = "The degree to which my clinical work is  recognized and valued by my immediate senior";
            string q6 = "The degree to which I am consulted by my immediate senior";
            string q7 = "My current length of commute to work";
            string q8 = "The levels of staffing in my team";
            string q9 = "The number of hours on my rota outside of Monday-Friday 8am to 6pm";
            string q10 = "The management of my rota";
            string q11 = "The amount of involvement I have in decision-making at this workplace";

            string tooltip = "The term 'IMMEDIATE SENIOR' refers to e.g. senior house officer, registrar, or consultant. If you are not sure who your immediate senior is: Who did you report to on your last working shift?";

            QuestionAnswer q1o = new QuestionAnswer { ID = 1, Name = q1, LongName = null, Ans = null };
            QuestionAnswer q2o = new QuestionAnswer { ID = 2, Name = q2, LongName = null, Ans = null };
            QuestionAnswer q3o = new QuestionAnswer { ID = 3, Name = q3, LongName = null, Ans = null };
            QuestionAnswer q4o = new QuestionAnswer { ID = 4, Name = q4, LongName = null, Ans = null };
            QuestionAnswer q5o = new QuestionAnswer { ID = 5, Name = q5, LongName = tooltip, Ans = null };
            QuestionAnswer q6o = new QuestionAnswer { ID = 6, Name = q6, LongName = tooltip, Ans = null };
            QuestionAnswer q7o = new QuestionAnswer { ID = 7, Name = q7, LongName = null, Ans = null };
            QuestionAnswer q8o = new QuestionAnswer { ID = 8, Name = q8, LongName = null, Ans = null };
            QuestionAnswer q9o = new QuestionAnswer { ID = 9, Name = q9, LongName = null, Ans = null };
            QuestionAnswer q10o = new QuestionAnswer { ID = 10, Name = q10, LongName = null, Ans = null };
            QuestionAnswer q11o = new QuestionAnswer { ID = 11, Name = q11, LongName = null, Ans = null };
           
            List<QuestionAnswer> list = new List<QuestionAnswer>();
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

            return list;
        }
        public IList<QuestionAnswer> GetAnsThirdJob(int profileId, int exitSurveyId)
        {
            var list = _unitOfWork.ThirdJobRepository
                                  .GetUsingNoTracking(x => x.ProfileId == profileId
                                                          && x.ExitSurveyId == exitSurveyId)
                                  .Select(m => new QuestionAnswer()
                                    {
                                        ID = m.Id,
                                        Name = m.Qn,
                                        Ans = m.Ans
                                    }).ToList();

            return list;

        }
        public async Task SaveThirdJob(int profileId, int exitSurveyId, IList<QuestionAnswer> selectedQns)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.ThirdJobRepository
                                      .Get(p => p.ProfileId == profileId
                                            && p.ExitSurveyId == exitSurveyId);

                           _unitOfWork.ThirdJobRepository.RemoveRange(list);
                           _unitOfWork.SaveChanges();

                foreach (var r in selectedQns)
                {
                    if (!string.IsNullOrEmpty(r.Ans))
                    {
                        var newDefaultTask = new ThirdJob
                        {
                            ProfileId = profileId,
                            ExitSurveyId = exitSurveyId,
                            Qn = r.Name,
                            Ans = r.Ans
                        };
                        _unitOfWork.ThirdJobRepository.Insert(newDefaultTask);
                    }
                }
                _unitOfWork.SaveChanges();
            });
        }

        public IList<QuestionAnswer> GetQuestionsFirstWE()
        {
            string q1 = "My immediate senior understands the nature of the work I do";
            string q2 = "The feedback I receive from my immediate senior helps improve my clinical work";
            string q3 = "The feedback I receive from my immediate senior helps improve my research work";
            string q4 = "My immediate senior is a good leader";
            string q5 = "Morale/work climate within my department is excellent";
            string q6 = "I am allowed to care for patients in my own way";
            string q7 = "There is a good team relationship among the colleagues in my department";
            string q8 = "My immediate senior is a good communicator / communicates well";

            string q9 = "My immediate senior recognizes and values my clinical work";
            string q10 = "My immediate senior consults me";
            string q11 = "I am happy with the management of my rota";
            string q12 = "I am involved in decision-making at this workplace";

            string q13 = "My current length of commute to work is short enough";
            string q14 = "The staffing levels in my current team are adequate";
            string q15 = "I have an acceptable number of working hours outside of 8am-6pm Monday-Friday";

            string tooltip = "The term 'IMMEDIATE SENIOR' refers to e.g. senior house officer, registrar, or consultant. If you are not sure who your immediate senior is: Who did you report to on your last working shift?";

            QuestionAnswer q1o = new QuestionAnswer { ID = 1, Name = q1, LongName = tooltip, Ans = null };
            QuestionAnswer q2o = new QuestionAnswer { ID = 2, Name = q2, LongName = tooltip, Ans = null };
            QuestionAnswer q3o = new QuestionAnswer { ID = 3, Name = q3, LongName = tooltip, Ans = null };
            QuestionAnswer q4o = new QuestionAnswer { ID = 4, Name = q4, LongName = tooltip, Ans = null };
            QuestionAnswer q5o = new QuestionAnswer { ID = 5, Name = q5, LongName = null, Ans = null };
            QuestionAnswer q6o = new QuestionAnswer { ID = 6, Name = q6, LongName = null, Ans = null };
            QuestionAnswer q7o = new QuestionAnswer { ID = 7, Name = q7, LongName = null, Ans = null };
            QuestionAnswer q8o = new QuestionAnswer { ID = 8, Name = q8, LongName = tooltip, Ans = null };
            QuestionAnswer q9o = new QuestionAnswer { ID = 9, Name = q9, LongName = tooltip, Ans = null };
            QuestionAnswer q10o = new QuestionAnswer { ID = 10, Name = q10, LongName = tooltip, Ans = null };
            QuestionAnswer q11o = new QuestionAnswer { ID = 11, Name = q11, LongName = null, Ans = null };
            QuestionAnswer q12o = new QuestionAnswer { ID = 12, Name = q12, LongName = null, Ans = null };
            QuestionAnswer q13o = new QuestionAnswer { ID = 13, Name = q13, LongName = null, Ans = null };
            QuestionAnswer q14o = new QuestionAnswer { ID = 14, Name = q14, LongName = null, Ans = null };
            QuestionAnswer q15o = new QuestionAnswer { ID = 15, Name = q15, LongName = null, Ans = null };

            List<QuestionAnswer> list = new List<QuestionAnswer>();
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

            return list;
        }
        public IList<QuestionAnswer> GetAnsFirstWE(int profileId, int exitSurveyId)
        {
            var list = _unitOfWork.FirstWERepository
                                  .GetUsingNoTracking(x => x.ProfileId == profileId
                                                            && x.ExitSurveyId == exitSurveyId)
                                  .Select(m => new QuestionAnswer()
                                    {
                                        ID = m.Id,
                                        Name = m.Qn,
                                        Ans = m.Ans
                                    }).ToList();
            return list;
        }
        public async Task SaveFirstWE(int profileId, int exitSurveyId, IList<QuestionAnswer> selectedQns)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.FirstWERepository.Get(p => p.ProfileId == profileId);
                           _unitOfWork.FirstWERepository.RemoveRange(list);
                           _unitOfWork.SaveChanges();

                foreach (var r in selectedQns)
                {
                    if (!string.IsNullOrEmpty(r.Ans))
                    {
                        var newDefaultTask = new FirstWorkEnvironment
                        {
                            ProfileId = profileId,
                            ExitSurveyId = exitSurveyId,
                            Qn = r.Name,
                            Ans = r.Ans
                        };
                        _unitOfWork.FirstWERepository.Insert(newDefaultTask);
                    }
                }
                _unitOfWork.SaveChanges();
            });
        }

        public IList<QuestionAnswer> GetSecondWorkEnvironmentQns()
        {
            string q1 = "Senior Clinicians on my team really care about my wellbeing";
            string q2 = "Nursing and Allied Health Care Staff on my team really care about my wellbeing";
            string q3 = "Management really cares about my wellbeing";

            string tooltip = "";

            QuestionAnswer q1o = new QuestionAnswer { ID = 1, Name = q1, LongName = tooltip, Ans = null };
            QuestionAnswer q2o = new QuestionAnswer { ID = 2, Name = q2, LongName = tooltip, Ans = null };
            QuestionAnswer q3o = new QuestionAnswer { ID = 3, Name = q3, LongName = tooltip, Ans = null };


            List<QuestionAnswer> list = new List<QuestionAnswer>();
            list.Add(q1o);
            list.Add(q2o);
            list.Add(q3o);

            return list;
        }
        public IList<QuestionAnswer> GetSecondWorkEnvironmentAns(int profileId)
        {
            var list = _unitOfWork.SecondWERepository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .Select(m => new QuestionAnswer()
                {
                    ID = m.Id,
                    Name = m.Qn,
                    Ans = m.Ans
                }).ToList();

            return list;
        }
        public async Task SaveSecondWE(int profileId, int exitSurveyId, IList<QuestionAnswer> selectedQns)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.SecondWERepository.Get(p => p.ProfileId == profileId);

                _unitOfWork.SecondWERepository.RemoveRange(list);
                _unitOfWork.SaveChanges();

                foreach (var r in selectedQns)
                {
                    if (!string.IsNullOrEmpty(r.Ans))
                    {
                        var newDefaultTask = new SecondWorkEnvironment
                        {
                            ProfileId = profileId,
                            ExitSurveyId = exitSurveyId,
                            Qn = r.Name,
                            Ans = r.Ans
                        };
                        _unitOfWork.SecondWERepository.Insert(newDefaultTask);
                    }
                }
                _unitOfWork.SaveChanges();
            });
        }
        public ThirdWorkEnvironmentDto GetThirdWEbyId(int profileId, int exitSurveyId)
        {
            ThirdWorkEnvironment profile = _unitOfWork.ThirdWERepository
                                                       .GetUsingNoTracking(x => x.ProfileId == profileId
                                                                                && x.ExitSurveyId == exitSurveyId)
                                                       .FirstOrDefault();
            if (profile != null)
            {
                return ObjectMapper.GetThirdWEDto(profile);
            }
            return null;
        }
        public async Task SaveThirdWE(ThirdWorkEnvironmentDto d)
        {
            var list = _unitOfWork.ThirdWERepository
                                  .Get(p => p.ProfileId == d.ProfileId
                                          && p.ExitSurveyId == d.ExitSurveyId);
                       _unitOfWork.ThirdWERepository.RemoveRange(list);

            var e = ObjectMapper.GetThirdWEEntity(d);
            _unitOfWork.ThirdWERepository.Insert(e);
            _unitOfWork.SaveChanges();
        }

        public IList<QuestionAnswer> GetQuestionsFourthWE()
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

            QuestionAnswer q1o = new QuestionAnswer { ID = 1, Name = q1, LongName = null, Ans = null };
            QuestionAnswer q2o = new QuestionAnswer { ID = 2, Name = q2, LongName = null, Ans = null };
            QuestionAnswer q3o = new QuestionAnswer { ID = 3, Name = q3, LongName = null, Ans = null };
            QuestionAnswer q4o = new QuestionAnswer { ID = 4, Name = q4, LongName = null, Ans = null };
            QuestionAnswer q5o = new QuestionAnswer { ID = 5, Name = q5, LongName = null, Ans = null };
            QuestionAnswer q6o = new QuestionAnswer { ID = 6, Name = q6, LongName = null, Ans = null };
            QuestionAnswer q7o = new QuestionAnswer { ID = 7, Name = q7, LongName = null, Ans = null };
            QuestionAnswer q8o = new QuestionAnswer { ID = 8, Name = q8, LongName = null, Ans = null };

            List<QuestionAnswer> list = new List<QuestionAnswer>();
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
        public IList<QuestionAnswer> GetAnsFourthWE(int profileId, int exitSurveyId)
        {
            var list = _unitOfWork.FourthWERepository
                                  .GetUsingNoTracking(x => x.ProfileId == profileId
                                                            && x.ExitSurveyId == exitSurveyId)
                                  .Select(m => new QuestionAnswer()
                                  {
                                      ID = m.Id,
                                      Name = m.Qn,
                                      Ans = m.Ans
                                  }).ToList();
            return list;
        }
        public async Task SaveFourthWE(int profileId, int exitSurveyId, IList<QuestionAnswer> selectedQns)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.FourthWERepository
                                      .Get(p => p.ProfileId == profileId
                                                && p.ExitSurveyId == exitSurveyId);
                _unitOfWork.FourthWERepository.RemoveRange(list);
                _unitOfWork.SaveChanges();

                foreach (var r in selectedQns)
                {
                    if (!string.IsNullOrEmpty(r.Ans))
                    {
                        var newDefaultTask = new FourthWorkEnvironment
                        {
                            ProfileId = profileId,
                            ExitSurveyId = exitSurveyId,
                            Qn = r.Name,
                            Ans = r.Ans
                        };
                        _unitOfWork.FourthWERepository.Insert(newDefaultTask);
                    }
                }
                _unitOfWork.SaveChanges();
            });
        }

        public FifthWorkEnvironmentDto GetFifthWEById(int profileId, int exitSurveyId)
        {
            FifthWorkEnvironment profile = _unitOfWork.FifthWERepository
                .GetUsingNoTracking(x => x.ProfileId == profileId && x.ExitSurveyId == exitSurveyId)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetFifthWEDto(profile);
            }
            return null;
        }
        public async Task SaveFifthWE(FifthWorkEnvironmentDto d)
        {
            var list = _unitOfWork.FifthWERepository.Get(p => p.ProfileId == d.ProfileId 
                                                        && p.ExitSurveyId == d.ExitSurveyId);
                       _unitOfWork.FifthWERepository.RemoveRange(list);

            var e = ObjectMapper.GetFifthWEEntity(d);
            _unitOfWork.FifthWERepository.Insert(e);
            _unitOfWork.SaveChanges();
        }

        public IList<QuestionAnswer> GetOptionsFirstTraining()
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

            QuestionAnswer option1o = new QuestionAnswer { ID = 0, Name = option1, LongName = null, Ans = null };
            QuestionAnswer option2o = new QuestionAnswer { ID = 1, Name = option2, LongName = null, Ans = null };
            QuestionAnswer option3o = new QuestionAnswer { ID = 2, Name = option3, LongName = null, Ans = null };
            QuestionAnswer option4o = new QuestionAnswer { ID = 3, Name = option4, LongName = null, Ans = null };
            QuestionAnswer option5o = new QuestionAnswer { ID = 4, Name = option5, LongName = null, Ans = null };
            QuestionAnswer option6o = new QuestionAnswer { ID = 5, Name = option6, LongName = null, Ans = null };
            QuestionAnswer option7o = new QuestionAnswer { ID = 6, Name = option7, LongName = null, Ans = null };
            QuestionAnswer option8o = new QuestionAnswer { ID = 7, Name = option8, LongName = null, Ans = null };

            QuestionAnswer option9o = new QuestionAnswer { ID = 8, Name = option9, LongName = null, Ans = null };
            QuestionAnswer option10o = new QuestionAnswer { ID = 9, Name = option10, LongName = null, Ans = null };
            QuestionAnswer option11o = new QuestionAnswer { ID = 10, Name = option11, LongName = null, Ans = null };
            QuestionAnswer option12o = new QuestionAnswer { ID = 11, Name = option12, LongName = null, Ans = null };
            QuestionAnswer option13o = new QuestionAnswer { ID = 12, Name = option13, LongName = null, Ans = null };
            QuestionAnswer option14o = new QuestionAnswer { ID = 13, Name = option14, LongName = null, Ans = null };
            QuestionAnswer option15o = new QuestionAnswer { ID = 14, Name = option15, LongName = null, Ans = null };

            List<QuestionAnswer> list = new List<QuestionAnswer>();
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

        public IList<QnAn> GetQnsFirstTrainingYourTraining()
        {            
            string option1 = "WiFi access at your place of work";
            string option2 = "Good IT support/access";
            string option3 = "Doctors mess";
            string option4 = "Access to food and drink 24hrs a day";
            string option5 = "A quiet office/space to work/study";
            string option6 = "Careers support";
            string option7 = "Library access";
            string option8 = "Other";            

            string tooltip = "";

            QnAn option1o = new QnAn { ID = 0, ForGridID = "YT", Name = option1, LongName = null, Ans = null, OtherOptionString = null};
            QnAn option2o = new QnAn { ID = 1, ForGridID = "YT", Name = option2, LongName = null, Ans = null, OtherOptionString = null};
            QnAn option3o = new QnAn { ID = 2, ForGridID = "YT", Name = option3, LongName = null, Ans = null, OtherOptionString = null };
            QnAn option4o = new QnAn { ID = 3, ForGridID = "YT", Name = option4, LongName = null, Ans = null, OtherOptionString = null };
            QnAn option5o = new QnAn { ID = 4, ForGridID = "YT", Name = option5, LongName = null, Ans = null, OtherOptionString = null };
            QnAn option6o = new QnAn { ID = 5, ForGridID = "YT", Name = option6, LongName = null, Ans = null, OtherOptionString = null };
            QnAn option7o = new QnAn { ID = 6, ForGridID = "YT", Name = option7, LongName = null, Ans = null, OtherOptionString = null };
            QnAn option8o = new QnAn { ID = 7, ForGridID = "YT", Name = option8, LongName = null, Ans = null, OtherOptionString = null };            

            List<QnAn> list = new List<QnAn>();
            list.Add(option1o);
            list.Add(option2o);
            list.Add(option3o);
            list.Add(option4o);
            list.Add(option5o);
            list.Add(option6o);
            list.Add(option7o);
            list.Add(option8o);           

            return list;
        }
        public IList<QnAn> GetQnsFirstTrainingFeelingValued()
        {            
            string option1 = "Good communication between team members";
            string option2 = "Regular consultant presence";
            string option3 = "Supportive management";
            string option4 = "Regular team meetings";
            string option5 = "Regular feedback from colleagues on your performance";
            string option6 = "An environment in which you feel comfortable to raise concerns";
            string option7 = "Other";

            string tooltip = "";

            QnAn option1o = new QnAn { ID = 0, ForGridID = "FV", Name = option1, LongName = null, Ans = null, OtherOptionString = null };
            QnAn option2o = new QnAn { ID = 1, ForGridID = "FV", Name = option2, LongName = null, Ans = null, OtherOptionString = null };
            QnAn option3o = new QnAn { ID = 2, ForGridID = "FV", Name = option3, LongName = null, Ans = null, OtherOptionString = null };
            QnAn option4o = new QnAn { ID = 3, ForGridID = "FV", Name = option4, LongName = null, Ans = null, OtherOptionString = null };
            QnAn option5o = new QnAn { ID = 4, ForGridID = "FV", Name = option5, LongName = null, Ans = null, OtherOptionString = null };
            QnAn option6o = new QnAn { ID = 5, ForGridID = "FV", Name = option6, LongName = null, Ans = null, OtherOptionString = null };
            QnAn option7o = new QnAn { ID = 6, ForGridID = "FV", Name = option7, LongName = null, Ans = null, OtherOptionString = null };
            
            List<QnAn> list = new List<QnAn>();
            list.Add(option1o);
            list.Add(option2o);
            list.Add(option3o);
            list.Add(option4o);
            list.Add(option5o);
            list.Add(option6o);
            list.Add(option7o);            

            return list;
        }
        public IList<QnAn> GetFirstTrainingYourTrainingAns(int profileId, int exitSurveyId)
        {
            var list = _unitOfWork.FirstTrainingRepository
                                  .GetUsingNoTracking(x => x.ProfileId == profileId
                                                        && x.ExitSurveyId == exitSurveyId
                                                        && x.Qn == "YT")
                                  .Select(m => new QnAn()
                                  {
                                      ID = m.Id,
                                      ForGridID = m.Qn,
                                      Name = m.Options,
                                      Ans = m.Ans
                                  }).ToList();

            return list;
        }
        public IList<QnAn> GetFirstTrainingFeelingValued(int profileId, int exitSurveyId)
        {
            var list = _unitOfWork.FirstTrainingRepository
                                  .GetUsingNoTracking(x => x.ProfileId == profileId
                                                        && x.ExitSurveyId == exitSurveyId
                                                        && x.Qn == "FV")
                                  .Select(m => new QnAn()
                                  {
                                      ID = m.Id,
                                      ForGridID = m.Qn,
                                      Name = m.Options,
                                      Ans = m.Ans
                                  }).ToList();

            return list;
        }
        //public IList<QuestionAnswer> GetFirstTrainingOptions(int profileId, int exitSurveyId)
        //{
        //    var list = _unitOfWork.FirstTrainingRepository
        //        .GetUsingNoTracking(x => x.ProfileId == profileId &&  x.ExitSurveyId == exitSurveyId)
        //        .Select(m => new QuestionAnswer()
        //        {
        //            ID = m.QnId,
        //            Name = m.Options,
        //            Ans = m.OtherOption
        //        }).ToList();

        //    return list;
        //}       
        public async Task SaveFirstTraining(int profileId, int exitSurveyId, IList<QnAn> selectedOptions)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.FirstTrainingRepository.Get(p => p.ProfileId == profileId 
                                                                    && p.ExitSurveyId == exitSurveyId);
                           _unitOfWork.FirstTrainingRepository.RemoveRange(list);
                           _unitOfWork.SaveChanges();


                foreach (var r in selectedOptions)
                {
                    if (!string.IsNullOrEmpty(r.Ans))
                    {
                        var newDefaultTask = new FirstTraining
                        {
                            ProfileId = profileId,
                            ExitSurveyId = exitSurveyId,
                            OtherOption = r.OtherOptionString,
                            Options = r.Name,
                            QId = r.ID,
                            Qn = r.ForGridID,
                            Ans = r.Ans                            
                        };
                        _unitOfWork.FirstTrainingRepository.Insert(newDefaultTask);
                    }
                }
                        _unitOfWork.SaveChanges();
            });
        }

        public IList<QuestionAnswer> GetSecondTrainingQuestions()
        {
            //string q1 = "Genuine interest/passion for that speciality";
            string q2 = "Opportunities for training LTFT as a trainee";
            string q3 = "Opportunities for working LTFT as a consultant/GP";

            string q4 = "The number of on-calls as a trainee";
            string q5 = "The number of on-calls as a consultant/GP";
            string q6 = "The number of anti-social hours as a trainee";

            string q7 = "The number of anti-social hours as a consultant/GP";
            string q8 = "Potential income as a consultant/GP";

            string q9 = "Ability to train in a particular part of the UK?";
            string q10 = "Ability to get a consultant/GP post in a particular part of the UK";

            //QuestionAnswer q1o = new QuestionAnswer { ID = 1, Name = q1, LongName = null, Ans = null };
            QuestionAnswer q2o = new QuestionAnswer { ID = 2, Name = q2, LongName = "LTFT is abbreviated term for: Less than full time", Ans = null };
            QuestionAnswer q3o = new QuestionAnswer { ID = 3, Name = q3, LongName = "LTFT is abbreviated term for: Less than full time", Ans = null };

            QuestionAnswer q4o = new QuestionAnswer { ID = 4, Name = q4, LongName = null, Ans = null };
            QuestionAnswer q5o = new QuestionAnswer { ID = 5, Name = q5, LongName = null, Ans = null };
            QuestionAnswer q6o = new QuestionAnswer { ID = 6, Name = q6, LongName = null, Ans = null };

            QuestionAnswer q7o = new QuestionAnswer { ID = 7, Name = q7, LongName = null, Ans = null };
            QuestionAnswer q8o = new QuestionAnswer { ID = 8, Name = q8, LongName = null, Ans = null };

            QuestionAnswer q9o = new QuestionAnswer { ID = 9, Name = q9, LongName = null, Ans = null };
            QuestionAnswer q10o = new QuestionAnswer { ID = 10, Name = q10, LongName = null, Ans = null };

            List<QuestionAnswer> list = new List<QuestionAnswer>();
            //list.Add(q1o);
            list.Add(q2o);
            list.Add(q3o);

            list.Add(q4o);
            list.Add(q5o);
            list.Add(q6o);

            list.Add(q7o);
            list.Add(q8o);
            list.Add(q9o);
            list.Add(q10o);
            return list;
        }
        public IList<QuestionAnswer> GetSecondTrainingAns(int profileId, int exitSurveyId)
        {
            var list = _unitOfWork.SecondTrainingRepository
                                  .GetUsingNoTracking(x => x.ProfileId == profileId)
                                   .Select(m => new QuestionAnswer()
                                   {
                                       ID = m.Id,
                                       Name = m.Qn,
                                       Ans = m.Ans
                                   }).ToList();
            return list;
        }
        public async Task SaveSecondTraining(int profileId, int exitSurveyId, IList<QuestionAnswer> selectedQns)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.SecondTrainingRepository
                                      .Get(p => p.ProfileId == profileId && p.ExitSurveyId == exitSurveyId);

                           _unitOfWork.SecondTrainingRepository.RemoveRange(list);
                           _unitOfWork.SaveChanges();

                foreach (var r in selectedQns)
                {
                    if (!string.IsNullOrEmpty(r.Ans))
                    {
                        var newDefaultTask = new SecondTraining
                        {
                            ProfileId = profileId,
                            ExitSurveyId = exitSurveyId,
                            Qn = r.Name,
                            Ans = r.Ans
                        };
                        _unitOfWork.SecondTrainingRepository.Insert(newDefaultTask);
                    }
                }
                _unitOfWork.SaveChanges();
            });
        }

        public IList<QuestionAnswer> GetThirdTrainingQuestions()
        {
            string q1 = "Listening/speaking to colleagues about their experiences in that specialty";
            string q2 = "Hearing colleagues talk about that specialty in a negative way (e" + "\u22C5" + "g" + "\u22C5 " + "\u2018" + "Specialty bashing" + "\u2019" + ")";
            string q3 = "Your own experience of working in that specialty";

            QuestionAnswer q1o = new QuestionAnswer { ID = 1, Name = q1, LongName = null, Ans = null };
            QuestionAnswer q2o = new QuestionAnswer { ID = 2, Name = q2, LongName = null, Ans = null };
            QuestionAnswer q3o = new QuestionAnswer { ID = 3, Name = q3, LongName = null, Ans = null };

            List<QuestionAnswer> list = new List<QuestionAnswer>();
            list.Add(q1o);
            list.Add(q2o);
            list.Add(q3o);

            return list;
        }
        public IList<QuestionAnswer> GetThirdTrainingAns(int profileId, int exitSurveyId)
        {
            var list = _unitOfWork.ThirdTrainingRepository
                                  .GetUsingNoTracking(x => x.ProfileId == profileId
                                                        && x.ExitSurveyId == exitSurveyId)
                                  .Select(m => new QuestionAnswer()
                                  {
                                    ID = m.Id,
                                    Name = m.Qn,
                                    Ans = m.Ans
                                  }).ToList();
            return list;
        }
        public async Task SaveThirdTraining(int profileId, int exitSurveyId, IList<QuestionAnswer> selectedQns)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.ThirdTrainingRepository
                                      .Get(p => p.ProfileId == profileId
                                                && p.ExitSurveyId == exitSurveyId);

                           _unitOfWork.ThirdTrainingRepository.RemoveRange(list);
                           _unitOfWork.SaveChanges();

                foreach (var r in selectedQns)
                {
                    if (!string.IsNullOrEmpty(r.Ans))
                    {
                        var newDefaultTask = new ThirdTraining
                        {
                            ProfileId = profileId,
                            ExitSurveyId = exitSurveyId,
                            Qn = r.Name,
                            Ans = r.Ans
                        };
                        _unitOfWork.ThirdTrainingRepository.Insert(newDefaultTask);
                    }
                }
                _unitOfWork.SaveChanges();
            });
        }
        public AboutYouESDto GetAboutYouById(int profileId, int exitSurveyId)
        {
            AboutYouES profile = _unitOfWork.AboutYouESRepository
                                            .GetUsingNoTracking(x => x.ProfileId == profileId
                                                                  && x.ExitSurveyId == exitSurveyId)
                                             .FirstOrDefault();

                        if (profile != null)
                        { return ObjectMapper.GetAboutYouDto(profile); }

            return null;
        }
        public async Task SaveAboutYouES(AboutYouESDto d)
        {
            var list = _unitOfWork.AboutYouESRepository
                                  .Get(p => p.ProfileId == d.ProfileId
                                         && p.ExitSurveyId == d.ExitSurveyId);
                       _unitOfWork.AboutYouESRepository.RemoveRange(list);

            var e = ObjectMapper.GetAboutYouEntity(d);

                       _unitOfWork.AboutYouESRepository.Insert(e);
                       _unitOfWork.SaveChanges();
        }
    }
}