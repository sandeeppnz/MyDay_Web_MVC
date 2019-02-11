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

    public class RegistrationService : IDisposable
    {
        /*
         
            - This layer implements UnitOfWork pattern
            - The Tables ProfileComments, ProfileDemo... are retrieved from the methods in this file
             
             
             */
        readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public RegistrationService()
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
        public IList<SelectedTaskVM> GetProfileTaskItemTableListByProfileId(int profileId)
        {
            var list = _unitOfWork.ProfileTasksRespository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .Select(m => new SelectedTaskVM()
                {
                    ID = m.TaskItemId,
                    Frequency = m.Frequency
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
        public IList<TaskVM> GetTasksItemsTableListByType(string type)
        {
            var list = _unitOfWork.TaskItemRespository.Get(x => x.Type == type)
                .Select(x => new TaskVM()
                {
                    ID = x.Id,
                    Name = x.ShortName,
                    LongName = x.LongName,
                    Frequency = null
                }).ToList();

            return list;
        }
        public ProfileWellbeingDto GetProfileWellBeingByProfileId(int profileId)
        {
            ProfileWellbeing profile = _unitOfWork.ProfileWellbeingRespository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetProfileWellbeingDto(profile);
            }
            return null;

        }
        public ProfilePlacementDto GetProfilePlacementByProfileId(int profileId)
        {
            ProfilePlacement profile = _unitOfWork.ProfilePlacementRespository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetProfilePlacementDto(profile);
            }
            return null;
        }
        public ProfileTaskTimeDto GetProfileTaskTimeByProfileId(int profileId)
        {
            ProfileTaskTime profile = _unitOfWork.ProfileTaskTimeRespository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetProfileTaskTimeDto(profile);
            }
            return null;
        }
        public ProfileContractDto GetProfileContractByProfileId(int profileId)
        {
            ProfileContract profile = _unitOfWork.ProfileContractRespository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetProfileContractDto(profile);
            }
            return null;
        }
        public ProfileTrainingDto GetProfileTrainingByProfileId(int profileId)
        {
            ProfileTraining profile = _unitOfWork.ProfileTrainingRespository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetProfileTrainingDto(profile);
            }
            return null;
        }
        public ProfileDemographicDto GetProfileDemographicByProfileId(int profileId)
        {
            ProfileDemographic profile = _unitOfWork.ProfileDemographicRespository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetProfileDemographicDto(profile);
            }
            return null;
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
        public async Task<ProfileDto> GetProfileByIdAsync(int id)
        {
            Profile profile = await _unitOfWork.ProfileRespository.GetByIDAsync(id);
            if (profile != null)
            {
                return ObjectMapper.GetProfileDto(profile);
            }
            return null;
        }
        public ProfileDto GetProfileByUid(string uid)
        {
            var profile = _unitOfWork.ProfileRespository
                .GetUsingNoTracking(m => m.Uid == uid)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetProfileDto(profile);
            }
            return null;
        }
        public ProfileDto GetProfileByLoginEmailAndUid(string loginEmail, string uid)
        {
            loginEmail = StringCipher.EncryptRfc2898(loginEmail);

            var profile = _unitOfWork.ProfileRespository
                                .GetUsingNoTracking(m => (m.LoginEmail == loginEmail) && (m.Uid == uid))
                                .FirstOrDefault();
            if (profile != null)
            {
                return ObjectMapper.GetProfileDto(profile);
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
        public RedirectionUrlDto CheckRegistrationAtLogin(string loginEmail, out int profileId)
        {

            ProfileDto profile = GetProfileByLoginEmail(loginEmail);
            if (profile != null)
            {
                string currRegProgressNext = profile.RegistrationProgressNext;
                profileId = profile.Id;

                if (currRegProgressNext != Constants.StatusRegistrationProgress.Completed.ToString())
                {
                    if (currRegProgressNext == Constants.StatusRegistrationProgress.Invited.ToString())
                    {
                        return new RedirectionUrlDto { Action = "NeedToRegister", Controller = "Register" };
                    }

                    if (currRegProgressNext == Constants.StatusRegistrationProgress.Signup.ToString())
                    {
                        return new RedirectionUrlDto { Action = "Signup", Controller = "Account" };
                    }

                    return new RedirectionUrlDto { Action = currRegProgressNext, Controller = "Register" };
                }
                return new RedirectionUrlDto { Action = "Login", Controller = "Account" };
            }
            else
            {
                profileId = 0;
                return null;
            }


        }
        public bool RedirectConsentOnAgree(string uid)
        {
            var profile = GetProfileByUid(uid);

            if (profile != null)
            {
                profile.RegistrationProgressNext = Constants.StatusRegistrationProgress.Signup.ToString();
                if (profile.ClientInitials.ToLower().ToString() == "wam")
                { profile.MaxStep = 1; }
                UpdateProfile(profile);
                return true;
            }

            return false;
        }
        public bool RedirectConsentOnDisagree(string uid)
        {
            var profile = GetProfileByUid(uid);
            if (profile != null)
            {
                profile.RegistrationProgressNext = Constants.StatusRegistrationProgress.DNP.ToString();
                UpdateProfile(profile);
                return true;

            }

            return false;
        }
        public bool IsEmailValid(string signupEmail)
        {
            int numOfEmails = GetSameNumberOfEmails(signupEmail);
            if (numOfEmails < 2)
            {
                return true;
            }

            return false;
        }
        public bool IsRegisteredProfile(string signupEmail)
        {  
            ProfileDto profile = GetProfileByLoginEmail(signupEmail);

            if (profile != null)
            {
                return true;
            }

            return false;
        }
        public bool IsRegistrationCompleted(string signupEmail, string uid)
        {

            ProfileDto profile = GetProfileByLoginEmailAndUid(signupEmail, uid);

            if (profile != null)
            {
                if (profile.RegistrationProgressNext == Constants.StatusRegistrationProgress.DNP.ToString())
                {
                    return true;

                }
            }
            return false;
        }
        public ProfileDto SignupOnExit(string signupEmail, string uid, string userId)
        {
            ProfileDto profile = GetProfileByLoginEmailAndUid(signupEmail, uid);
            if (profile != null)
            {
                string newStatus = profile.RegistrationProgressNext;
                profile.UserId = userId;

                if (profile.RegistrationProgressNext == Constants.StatusRegistrationProgress.Signup.ToString())
                {
                    if (profile.ClientInitials.ToLower().ToString() == "jd") //For junior doctors
                    { newStatus = Constants.StatusRegistrationProgress.Wellbeing.ToString(); }
                    else if(profile.ClientInitials.ToLower().ToString() == "wam") //For warren and mahony
                    { newStatus = Constants.StatusRegistrationProgress.WAMWellbeing.ToString(); }
                }

                profile.RegistrationProgressNext = newStatus;

                UpdateProfile(profile);

                return profile;
            }

            return null;
        }
        public RedirectionUrlDto ValidateConsentOnExit(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                return new RedirectionUrlDto { Action = "Login", Controller = "Account" };
            }

            var profile = GetProfileByUid(uid);

            if (profile != null)
            {
                string currRegStatus = profile.RegistrationProgressNext;
                
                if (
                    //For Junior Doctors
                    currRegStatus == Constants.StatusRegistrationProgress.Completed.ToString() ||
                    currRegStatus == Constants.StatusRegistrationProgress.Task.ToString() ||
                    currRegStatus == Constants.StatusRegistrationProgress.Demographics.ToString() ||
                    currRegStatus == Constants.StatusRegistrationProgress.Roster.ToString() ||
                    currRegStatus == Constants.StatusRegistrationProgress.Wellbeing.ToString() ||
                    currRegStatus == Constants.StatusRegistrationProgress.Screening.ToString()
                                                    ||
                    //For Warren and Mahony
                    currRegStatus == Constants.StatusRegistrationProgress.WAMWellbeing.ToString() ||
                    currRegStatus == Constants.StatusRegistrationProgress.WAMProfileRole.ToString() ||
                    currRegStatus == Constants.StatusRegistrationProgress.WAMTasks.ToString() ||
                    currRegStatus == Constants.StatusRegistrationProgress.WAMDemographics.ToString() ||
                    currRegStatus == Constants.StatusRegistrationProgress.WAMIntentions.ToString() ||
                    currRegStatus == Constants.StatusRegistrationProgress.WAMRoster.ToString()
                    )
                {
                    return new RedirectionUrlDto { Action = "Login", Controller = "Account" };

                }                
               
                return new RedirectionUrlDto { Action = "Signup", Controller = "Account" };
            }
            return null;
        }
        public RedirectionUrlDto ValidateConsentOnEntry(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                return new RedirectionUrlDto { Action = "Login", Controller = "Account" };
            }

            var profile = GetProfileByUid(uid);

            if (profile != null)
            {
                string currRegStatus = profile.RegistrationProgressNext;                
                     
                if (
                    //For Junior doctors
                    currRegStatus == Constants.StatusRegistrationProgress.Completed.ToString() ||
                    currRegStatus == Constants.StatusRegistrationProgress.Task.ToString() ||
                    currRegStatus == Constants.StatusRegistrationProgress.Demographics.ToString() ||
                    currRegStatus == Constants.StatusRegistrationProgress.Roster.ToString() ||
                    currRegStatus == Constants.StatusRegistrationProgress.Wellbeing.ToString() ||
                    currRegStatus == Constants.StatusRegistrationProgress.Screening.ToString()
                                                    ||
                    //For Warren and Mahony
                    currRegStatus == Constants.StatusRegistrationProgress.WAMWellbeing.ToString() ||
                    currRegStatus == Constants.StatusRegistrationProgress.WAMProfileRole.ToString() ||
                    currRegStatus == Constants.StatusRegistrationProgress.WAMTasks.ToString() ||
                    currRegStatus == Constants.StatusRegistrationProgress.WAMDemographics.ToString() ||
                    currRegStatus == Constants.StatusRegistrationProgress.WAMIntentions.ToString() ||
                    currRegStatus == Constants.StatusRegistrationProgress.WAMRoster.ToString()
                    )

                {
                    return new RedirectionUrlDto { Action = "Login", Controller = "Account" };

                }                               
            }
            return null;
        }

        public int GetSameNumberOfEmails(string loginEmail)
        {
            loginEmail = StringCipher.EncryptRfc2898(loginEmail);

            return
                _unitOfWork.ProfileRespository
                .GetUsingNoTracking(m => m.LoginEmail == loginEmail)
                .Count();
        }
        public int ValidateRoster(int profileId)
        {
            /*
             
             Logic checked in Controller:
                    1) If the total number of slots is less than 1, the calendar is empty
                    2) If the total number of slots is less than 3, the calendar is incomplete
                    3) Else the calendar is valid
             */

            return
              _unitOfWork.ProfileRosterRespository
              .GetUsingNoTracking(m => (m.ProfileId == profileId))
              .Count();
        }
        //public int Validate1WeekRoster(int profileId)
        //{
        //    /*
        //     Logic: Check at least 1 week of events are added from now
        //     */
        //    double numDays = 7;
        //    DateTime nextWeekDate = DateTime.UtcNow.AddDays(numDays);
        //    await db.RosterItems
        //            .Where(p => p.ProfileId == profile.Id)
        //            .Where(d => d.End <= nextWeekDate)
        //            .CountAsync();

        //    return
        //        _unitOfWork.profileRosterRespository
        //        .GetUsingNoTracking(m => (m.ProfileId == profileId) && (m.End > nextWeekDate))
        //        .Count();
        //}
        public async Task<int> NoOfSameProfileEmailAsync(string loginEmail)
        {
            //TODO: Need to implement
            return 0;
        }
        public async Task<ProfileDto> GetProfileByUidAsync(string uid)
        {
            //TODO: Need to implement
            return null;
        }
        public void UpdateProfile(ProfileDto p)
        {
            Profile e = ObjectMapper.GetProfileEntity(p);
            _unitOfWork.ProfileRespository.Update(e);
            _unitOfWork.SaveChanges();
        }


        public async Task SaveScreening(RegisterScreeningViewModel v)
        {

            if (v.CurrentLevelOfTraining == "Not in training" || v.IsCurrentPlacement == "No")
            {
                Profile profile = _unitOfWork.ProfileRespository.GetByID(v.ProfileId);

                if (profile != null)
                {
                    profile.CurrentLevelOfTraining = v.CurrentLevelOfTraining;
                    profile.IsCurrentPlacement = v.IsCurrentPlacement;
                    profile.RegistrationProgressNext = StatusRegistrationProgress.ScreenedOut.ToString();
                    profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                    profile.MaxStep = v.MaxStep;
                    _unitOfWork.ProfileRespository.Update(profile);
                    _unitOfWork.SaveChanges();
                }
            }
            else
            {
                Profile profile = _unitOfWork.ProfileRespository.GetByID(v.ProfileId);
                profile.CurrentLevelOfTraining = v.CurrentLevelOfTraining;
                profile.IsCurrentPlacement = v.IsCurrentPlacement;
                profile.RegistrationProgressNext = StatusRegistrationProgress.Signup.ToString();
                profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                profile.MaxStep = v.MaxStep;
                if (profile.MaxStep < 1)
                {
                    profile.MaxStep = 1;
                }


                _unitOfWork.ProfileRespository.Update(profile);
                _unitOfWork.SaveChanges();

            }


        }
        public bool SaveScreeningByUid(RegisterScreeningViewModel v)
        {
            //MaxStep = 2

            if (v.CurrentLevelOfTraining == "Not in training" || v.IsCurrentPlacement == "No")
            {
                Profile profile = _unitOfWork.ProfileRespository.GetByID(v.ProfileId);




                profile.CurrentLevelOfTraining = v.CurrentLevelOfTraining;
                profile.IsCurrentPlacement = v.IsCurrentPlacement;
                profile.RegistrationProgressNext = StatusRegistrationProgress.ScreenedOut.ToString();
                profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                profile.MaxStep = v.MaxStep;

                _unitOfWork.ProfileRespository.Update(profile);
                _unitOfWork.SaveChanges();

                return false;
            }
            else
            {
                Profile profile = _unitOfWork.ProfileRespository.GetByID(v.ProfileId);
                profile.CurrentLevelOfTraining = v.CurrentLevelOfTraining;
                profile.IsCurrentPlacement = v.IsCurrentPlacement;
                profile.RegistrationProgressNext = StatusRegistrationProgress.Wellbeing.ToString();
                profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                profile.MaxStep = v.MaxStep;
                if (profile.MaxStep < 2)
                {
                    profile.MaxStep = 2;
                }
                _unitOfWork.ProfileRespository.Update(profile);
                _unitOfWork.SaveChanges();

                return true;
            }


        }
        public async Task SaveProfileWellbeing(ProfileWellbeingDto d)
        {
            var list = _unitOfWork.ProfileWellbeingRespository.Get(p => p.ProfileId == d.ProfileId);
            _unitOfWork.ProfileWellbeingRespository.RemoveRange(list);

            var e = ObjectMapper.GetProfileWellbeingEntity(d);
            _unitOfWork.ProfileWellbeingRespository.Insert(e);
            _unitOfWork.SaveChanges();
        }
        public void SaveTaskTimes(ProfileTaskTimeDto d)
        {
            var list = _unitOfWork.ProfileTaskTimeRespository.Get(p => p.ProfileId == d.ProfileId);
            _unitOfWork.ProfileTaskTimeRespository.RemoveRange(list);

            var e = ObjectMapper.GetProfileTaskTimeEntity(d);
            _unitOfWork.ProfileTaskTimeRespository.Insert(e);
            _unitOfWork.SaveChanges();
        }
        public void SavePlacement(ProfilePlacementDto d)
        {
            var list = _unitOfWork.ProfilePlacementRespository.Get(p => p.ProfileId == d.ProfileId);
            _unitOfWork.ProfilePlacementRespository.RemoveRange(list);

            var e = ObjectMapper.GetProfilePlacementEntity(d);
            _unitOfWork.ProfilePlacementRespository.Insert(e);
            _unitOfWork.SaveChanges();
        }
        public void SaveContract(ProfileContractDto d)
        {
            var list = _unitOfWork.ProfileContractRespository.Get(p => p.ProfileId == d.ProfileId);
            _unitOfWork.ProfileContractRespository.RemoveRange(list);

            var e = ObjectMapper.GetProfileContractEntity(d);
            _unitOfWork.ProfileContractRespository.Insert(e);
            _unitOfWork.SaveChanges();
        }
        public void SaveComment(ProfileCommentDto d)
        {
            var list = _unitOfWork.ProfileCommentRespository.Get(p => p.ProfileId == d.ProfileId);
            _unitOfWork.ProfileCommentRespository.RemoveRange(list);

            var e = ObjectMapper.GetProfileCommentEntity(d);
            _unitOfWork.ProfileCommentRespository.Insert(e);
            _unitOfWork.SaveChanges();
        }
        public void SaveSpecialty(RegisterSpecialtyViewModel v)
        {

            Profile profile = _unitOfWork.ProfileRespository.GetByID(v.ProfileId);

            profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
            profile.RegistrationProgressNext = StatusRegistrationProgress.Training.ToString();
            profile.MaxStep = v.MaxStep;

            if (profile.MaxStep < 7)
            {
                profile.MaxStep = 7;
            }
            _unitOfWork.ProfileRespository.Update(profile);
            _unitOfWork.SaveChanges();
        }
        public void SaveTraining(ProfileTrainingDto d)
        {
            var list = _unitOfWork.ProfileTrainingRespository.Get(p => p.ProfileId == d.ProfileId);
            _unitOfWork.ProfileTrainingRespository.RemoveRange(list);

            var e = ObjectMapper.GetProfileTrainingEntity(d);
            _unitOfWork.ProfileTrainingRespository.Insert(e);
            _unitOfWork.SaveChanges();

        }
        public void SaveDemographics(ProfileDemographicDto d)
        {
            var list = _unitOfWork.ProfileDemographicRespository.Get(p => p.ProfileId == d.ProfileId);
            _unitOfWork.ProfileDemographicRespository.RemoveRange(list);

            var e = ObjectMapper.GetProfileDemographicEntity(d);
            _unitOfWork.ProfileDemographicRespository.Insert(e);
            _unitOfWork.SaveChanges();

        }
        public void Dispose()
        {
            _unitOfWork.Dispose();
        }
        public async Task SaveProfileSpecialtyAsync(int profileId, int[] selected, string otherText)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.ProfileSpecialtiesRespository.Get(p => p.ProfileId == profileId);

                _unitOfWork.ProfileSpecialtiesRespository.RemoveRange(list);
                _unitOfWork.SaveChanges();


                foreach (var r in selected)
                {

                    var e = new ProfileSpecialty
                    {
                        ProfileId = profileId,
                        SpecialtyId = r,
                        OtherText = otherText
                    };
                    _unitOfWork.ProfileSpecialtiesRespository.Insert(e);


                }
                _unitOfWork.SaveChanges();
            });
        }
        public async Task SaveDefaultTaskAsync(int profileId, IList<TaskVM> selectedTasks)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.ProfileTasksRespository.Get(p => p.ProfileId == profileId);

                _unitOfWork.ProfileTasksRespository.RemoveRange(list);
                _unitOfWork.SaveChanges();


                foreach (var r in selectedTasks)
                {
                    if (!string.IsNullOrEmpty(r.Frequency))
                    {
                        var newDefaultTask = new ProfileTask
                        {
                            ProfileId = profileId,
                            TaskItemId = r.ID,
                            Frequency = r.Frequency,
                            CreatedDateTimeUtc = DateTime.UtcNow
                        };
                        _unitOfWork.ProfileTasksRespository.Insert(newDefaultTask);
                    }

                }
                _unitOfWork.SaveChanges();
            });
        }
        public async Task CreateUserFeedback(int profileId, string baseUrl, EmailFeedbackViewModel v)
        {
            //add a record to db

            Feedback e = new Feedback();
            e.Channel = "Register";
            e.CreatedDateTimeUtc = DateTime.UtcNow;

            e.Email = v.EmailAddress;
            e.ContactNumber = v.PhoneNumber;
            e.PreferedContact = v.PreferedContact;
            e.PreferedTime = v.PreferedTime;
            e.Message = v.Message;
            e.ProfileId = profileId;

            _unitOfWork.FeedbackRespository.Insert(e);
            _unitOfWork.SaveChanges();

        }

        #region WAMDemo

        public WAMWellBeingDto GetWAMWellbeingByProfileId(int profileId)
        {
            WAMWellBeing profile = _unitOfWork.WAMWellbeingRespository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetWAMWellbeingDto(profile);
            }
            return null;

        }
        public async Task SaveWAMWellbeing(WAMWellBeingDto d)
        {
            var list = _unitOfWork.WAMWellbeingRespository.Get(p => p.ProfileId == d.ProfileId);
            _unitOfWork.WAMWellbeingRespository.RemoveRange(list);

            var e = ObjectMapper.GetWAMWellbeingEntity(d);
            _unitOfWork.WAMWellbeingRespository.Insert(e);
            _unitOfWork.SaveChanges();
        }
        public WAMProfileRoleDto GetWAMProfileRoleByProfileId(int profileId)
        {
            WAMProfileRole profile = _unitOfWork.WAMProfileRoleRepository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetWAMProfileRoleDto(profile);
            }
            return null;

        }
        public void SaveWAMProfileRole(WAMProfileRoleDto profileRoledto)
        {
            var list = _unitOfWork.WAMProfileRoleRepository.Get(p => p.ProfileId == profileRoledto.ProfileId);
            _unitOfWork.WAMProfileRoleRepository.RemoveRange(list);

            var e = ObjectMapper.GetWAMProfileRoleEntity(profileRoledto);
            _unitOfWork.WAMProfileRoleRepository.Insert(e);
            _unitOfWork.SaveChanges();
        }
        public WAMTasksDto GetWAMTasksByProfileId(int profileId)
        {
            WAMTasks profile = _unitOfWork.WAMTasksRespository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetWAMTasksDto(profile);
            }
            return null;
        }
        public void SaveWAMTasks(WAMTasksDto d)
        {
            var list = _unitOfWork.WAMTasksRespository.Get(p => p.ProfileId == d.ProfileId);
            _unitOfWork.WAMTasksRespository.RemoveRange(list);

            var e = ObjectMapper.GetWAMTasksEntity(d);
            _unitOfWork.WAMTasksRespository.Insert(e);
            _unitOfWork.SaveChanges();
        }
        public WAMDemographicsDto GetWAMDemographicsByProfileId(int profileId)
        {
            WAMDemographics profile = _unitOfWork.WAMDemographicsRespository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetWAMDemographicsDto(profile);
            }
            return null;
        }
        public void SaveWAMDemographics(WAMDemographicsDto d)
        {
            var list = _unitOfWork.WAMDemographicsRespository.Get(p => p.ProfileId == d.ProfileId);
            _unitOfWork.WAMDemographicsRespository.RemoveRange(list);

            var e = ObjectMapper.GetWAMDemographicsEntity(d);
            _unitOfWork.WAMDemographicsRespository.Insert(e);
            _unitOfWork.SaveChanges();

        }
        public WAMIntentionsDto GetWAMIntentionsByProfileId(int profileId)
        {
            WAMIntentions profile = _unitOfWork.WAMIntentionsRepository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetWAMIntentionsDto(profile);
            }
            return null;
        }
        public void SaveWAMIntentions(WAMIntentionsDto d)
        {
            var list = _unitOfWork.WAMIntentionsRepository.Get(p => p.ProfileId == d.ProfileId);
            _unitOfWork.WAMIntentionsRepository.RemoveRange(list);

            var e = ObjectMapper.GetWAMIntentionsEntity(d);
            _unitOfWork.WAMIntentionsRepository.Insert(e);
            _unitOfWork.SaveChanges();
        }
        public void SaveWAMFeedback(WAMFeedbackDto d)
        {
            var list = _unitOfWork.WAMFeedbackRespository.Get(p => p.ProfileId == d.ProfileId);
            _unitOfWork.WAMFeedbackRespository.RemoveRange(list);

            var e = ObjectMapper.GetWAMFeedbackEntity(d);
            _unitOfWork.WAMFeedbackRespository.Insert(e);
            _unitOfWork.SaveChanges();
        }
        #endregion
    }
}