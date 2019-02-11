using SANSurveyWebAPI.DAL;
using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.ViewModels.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static SANSurveyWebAPI.Constants;

namespace SANSurveyWebAPI.BLL
{
    public class KidsService
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private static bool UpdateDatabase = true;
        private ProfileService profileService;

        public KidsService()
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
        public IList<PeopleSelectionOptions> GetPeopleOptions()
        {
            string option1 = "Nobody - I was alone";
            string option2 = "Parent";
            string option3 = "Sibling";
            string option4 = "Other relative";
            string option5 = "Friend";
            string option6 = "Teacher";

            string tooltip = "";

            PeopleSelectionOptions option1o = new PeopleSelectionOptions { ID = 0, Name = option1, LongName = null, Ans = null };
            PeopleSelectionOptions option2o = new PeopleSelectionOptions { ID = 1, Name = option2, LongName = null, Ans = null };
            PeopleSelectionOptions option3o = new PeopleSelectionOptions { ID = 2, Name = option3, LongName = null, Ans = null };
            PeopleSelectionOptions option4o = new PeopleSelectionOptions { ID = 3, Name = option4, LongName = null, Ans = null };
            PeopleSelectionOptions option5o = new PeopleSelectionOptions { ID = 4, Name = option5, LongName = null, Ans = null };
            PeopleSelectionOptions option6o = new PeopleSelectionOptions { ID = 5, Name = option6, LongName = null, Ans = null };

            List<PeopleSelectionOptions> list = new List<PeopleSelectionOptions>();
            list.Add(option1o);
            list.Add(option2o);
            list.Add(option3o);
            list.Add(option4o);
            list.Add(option5o);
            list.Add(option6o);

            return list;
        }
        public IList<KidsTasks> GetKidsTaskByProfileandSurveyId(int ProfileId, int SurveyId)
        {
            var list = _unitOfWork.KidsTaskRepository
               .Get(x => x.ProfileId == ProfileId
                        && x.KidsSurveyId == SurveyId)
               .Select(x => new KidsTasks()
               {
                   Id = x.Id,
                   ProfileId = x.ProfileId,
                   KidsSurveyId = x.KidsSurveyId,
                   TaskName = x.TaskName,
                   Venue = x.Venue,
                   Travel = x.Travel,
                   StartTime = x.StartTime,
                   EndTime = x.EndTime,
                   InOutLocation = x.InOutLocation,
                   People = x.People,
                   IsEmotionalStageCompleted = x.IsEmotionalStageCompleted,
                   IsRandomlySelected = x.IsRandomlySelected,
               }).ToList();

            return list;
        }
        public List<KidsTasks> GetKidsRandomlySelectedTasksByProfileAndSurveyId(int ProfileId, int kidsSurveyId, bool randomSelection, bool emoStageComp)
        {
            var list = _unitOfWork.KidsTaskRepository
               .Get(x => x.ProfileId == ProfileId && x.KidsSurveyId == kidsSurveyId)
               .Where(x => x.IsRandomlySelected == randomSelection && x.IsEmotionalStageCompleted == emoStageComp)
               .Select(x => new KidsTasks()
               {
                   Id = x.Id,
                   ProfileId = x.ProfileId,
                   KidsSurveyId = x.KidsSurveyId,
                   TaskName = x.TaskName,
                   Venue = x.Venue,
                   Travel = x.Travel,
                   StartTime = x.StartTime,
                   EndTime = x.EndTime,
                   InOutLocation = x.InOutLocation,
                   People = x.People,
                   IsEmotionalStageCompleted = x.IsEmotionalStageCompleted,
                   IsRandomlySelected = x.IsRandomlySelected,
               }).ToList();

            return list;
        }
        public void AddKidsResponses(KidsResponsesDto r)
        {
            var e = ObjectMapper.GetKidsResponsesEntity(r);
            _unitOfWork.KidsResponseRepository.Insert(e);
        }
        public void SaveKidsResponses()
        {  _unitOfWork.SaveChanges(); }

        public void AddKidsFeedback(KidsFeedbackDto r)
        {
            var e = ObjectMapper.GetKidsFeedback(r);
            _unitOfWork.KidsFeedbackRepository.Insert(e);
        }
        public void SaveKidsFeedback()
        { _unitOfWork.SaveChanges(); }
        public void UpdateKidsResponse(KidsResponses r)
        {
            _unitOfWork.KidsResponseRepository.Update(r);
        }
        public KidsSurveyDto GetKidsSurveyById(int id)
        {
            KidsSurvey s = _unitOfWork.KidsSurveyRepository.GetByID(id);
            if (s != null)
            {
                return ObjectMapper.GetKidsSurveyDto(s);
            }
            return null;
        }
        public KidsFeedback GetKidsCommentByProfileandSurveyId(int profileId, int kidsSurveyId)
        {
            var list = _unitOfWork.KidsFeedbackRepository
                                  .Get(x => x.ProfileId == profileId
                                            && x.KidsSurveyId == kidsSurveyId)
                                  .Select(x => new KidsFeedback() {
                                      Id = x.Id,
                                      ProfileId = x.ProfileId,
                                      KidsSurveyId = x.KidsSurveyId,
                                      Comments = x.Comments
                                  }).FirstOrDefault();

            return list;
        }
        public virtual IList<KidsResponses> GetGenericKidsResponses(int kidsSurveyId, int profileId)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<KidsResponses> result = null;

            if (!IsWebApiRequest)
            { result = HttpContext.Current.Session["SurveyResponses"] as IList<KidsResponses>; }

            if (result == null || UpdateDatabase)
            {
                try
                {
                    result = _unitOfWork.KidsResponseRepository.GetUsingNoTracking(
                        i => (i.ProfileId == profileId
                        && i.KidsSurveyId == kidsSurveyId)
                        ).ToList();
                }
                catch (Exception) { throw; }
                if (!IsWebApiRequest)
                { HttpContext.Current.Session["SurveyResponses"] = result; }
            }
            return result;
        }
        public virtual IList<KidsResponses> GetAllKidsResponses(int taskId, int profileId, int kidSurveyId, DateTime taskStartDate)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<KidsResponses> result = null;

            if (!IsWebApiRequest)
            { result = HttpContext.Current.Session["SurveyResponses"] as IList<KidsResponses>; }

            if (result == null || UpdateDatabase)
            {
                try
                {
                    result = _unitOfWork.KidsResponseRepository.GetUsingNoTracking(
                       i => (i.ProfileId == profileId
                       && i.KidsSurveyId == kidSurveyId
                       && i.KidsTaskId == taskId
                       )).ToList();
                }
                catch (Exception)
                { throw; }

                if (!IsWebApiRequest)
                { HttpContext.Current.Session["SurveyResponses"] = result; }
            }
            return result;
        }
        public KidsTasksDto GetKidsTaskByTaskId(int id)
        {
            KidsTasks s = _unitOfWork.KidsTaskRepository.GetByID(id);
            if (s != null)
            {
                return ObjectMapper.GetKidsTasksDto(s);
            }
            return null;
        }
        public IList<IdValueOptions> GetYesNoOptions()
        {
            string option1 = "Yes";
            string option2 = "No";            

            string tooltip = "";

            IdValueOptions option1o = new IdValueOptions { Id = 1, Value = "Yes"};
            IdValueOptions option2o = new IdValueOptions { Id = 2, Value = "No" };
            
            List<IdValueOptions> list = new List<IdValueOptions>();
            list.Add(option1o);
            list.Add(option2o);

            return list;
        }
        public IList<IdValueOptions> GetInOutOptions()
        {
            string option1 = "Inside";
            string option2 = "Outside";

            string tooltip = "";

            IdValueOptions option1o = new IdValueOptions { Id = 1, Value = "Inside" };
            IdValueOptions option2o = new IdValueOptions { Id = 2, Value = "Outside" };

            List<IdValueOptions> list = new List<IdValueOptions>();
            list.Add(option1o);
            list.Add(option2o);

            return list;
        }
        public IList<IdValueOptions> GetTransportOptions()
        {
            string option1 = "Walk / Run";
            string option2 = "Bicycle";
            string option3 = "Car";
            string option4 = "Public transport e.g. Bus";
            string option5 = "Other";

            string tooltip = "";

            IdValueOptions option1o = new IdValueOptions { Id = 1, Value = option1, LongName = "walking/running" };
            IdValueOptions option2o = new IdValueOptions { Id = 2, Value = option2, LongName = "cycling" };
            IdValueOptions option3o = new IdValueOptions { Id = 3, Value = option3, LongName = "driving" };
            IdValueOptions option4o = new IdValueOptions { Id = 4, Value = option4, LongName = "riding public transport" };
            IdValueOptions option5o = new IdValueOptions { Id = 5, Value = option5, LongName = "traveling by" };

            List<IdValueOptions> list = new List<IdValueOptions>();
            list.Add(option1o);
            list.Add(option2o);
            list.Add(option3o);
            list.Add(option4o);
            list.Add(option5o);

            return list;
        }
        public IList<IdValueOptions> GetLocationOptions()
        {
            string option1 = "Home";
            string option2 = "A friend / relative's house";
            string option3 = "School";
            string option4 = "A sports field / Park";
            string option5 = "A shopping mall";
            string option6 = "Other";

            string tooltip = "";

            IdValueOptions option1o = new IdValueOptions { Id = 1, Value = option1, LongName = null};
            IdValueOptions option2o = new IdValueOptions { Id = 2, Value = option2, LongName = null};
            IdValueOptions option3o = new IdValueOptions { Id = 3, Value = option3, LongName = null };
            IdValueOptions option4o = new IdValueOptions { Id = 4, Value = option4, LongName = null };
            IdValueOptions option5o = new IdValueOptions { Id = 5, Value = option5, LongName = null };
            IdValueOptions option6o = new IdValueOptions { Id = 6, Value = option6, LongName = null };

            List<IdValueOptions> list = new List<IdValueOptions>();
            list.Add(option1o);
            list.Add(option2o);
            list.Add(option3o);
            list.Add(option4o);
            list.Add(option5o);
            list.Add(option6o);

            return list;
        }
        public int SaveKidsLocation(KidsLocationDto r)
        {
            var e = ObjectMapper.GetKidsLocation(r);
            _unitOfWork.KidsLocationRepository.Insert(e);
            _unitOfWork.SaveChanges();

            int locationId = e.Id;
            return locationId;
        }
        public void UpdateKidsProfile(Profile r)
        {
            _unitOfWork.ProfileRespository.Update(r);
            _unitOfWork.SaveChanges();
        }
        public Profile GetKidsProfile(int profileId)
        {
            Profile profile = _unitOfWork.ProfileRespository.GetByID(profileId);
            return profile;
        }
        public KidsSurvey GetKidsSurvey(int surveyId)
        {
            KidsSurvey survey = _unitOfWork.KidsSurveyRepository.GetByID(surveyId);
            return survey;
        }
        public void UpdateKidsSurvey(KidsSurvey r)
        {
            _unitOfWork.KidsSurveyRepository.Update(r);
            _unitOfWork.SaveChanges();
        }
        public List<KidsLocation> GetKidsLocationByProfileAndSurveyId(int profileId, int kidsSurveyId)
        {
            var list = _unitOfWork.KidsLocationRepository
                                  .Get(x => x.ProfileId == profileId
                                            && x.KidsSurveyId == kidsSurveyId)
                                  .Select(x => new KidsLocation()
                                  {
                                      Id = x.Id,
                                      ProfileId = x.ProfileId,
                                      KidsSurveyId = x.KidsSurveyId,
                                      Location = x.Location,
                                      OtherLocation = x.OtherLocation,
                                      TimeSpentInHours = x.TimeSpentInHours,
                                      TimeSpentInMins = x.TimeSpentInMins,
                                      LocationSequence = x.LocationSequence,
                                      StartedAt = x.StartedAt,
                                      EndedAt = x.EndedAt,
                                      IsTasksEntered = x.IsTasksEntered
                                  }).ToList();

            return list;
        }
        public int SaveKidsTravelDetails(KidsTravelDto r)
        {
            var e = ObjectMapper.GetKidsTravelEntity(r);
            _unitOfWork.KidsTravelRepository.Insert(e);
            _unitOfWork.SaveChanges();

            int travelId = e.Id;
            return travelId;
        }
        public List<KidsTravel> GetKidsTravelByProfileAndSurveyId(int profileId, int kidsSurveyId)
        {
            var list = _unitOfWork.KidsTravelRepository
                                  .Get(x => x.ProfileId == profileId
                                            && x.KidsSurveyId == kidsSurveyId)
                                  .Select(x => new KidsTravel()
                                  {
                                      Id = x.Id,
                                      ProfileId = x.ProfileId,
                                      KidsSurveyId = x.KidsSurveyId,
                                      FromLocationId = x.FromLocationId,
                                      ToLocationId = x.ToLocationId,
                                      ModeOfTransport = x.ModeOfTransport,
                                      OtherModeOfTransport = x.OtherModeOfTransport,
                                      TravelTimeInHours = x.TravelTimeInHours,
                                      TravelTimeInMins = x.TravelTimeInMins,
                                      TravelStartedAt = x.TravelStartedAt,
                                      TravelEndedAt = x.TravelEndedAt
                                  }).ToList();

            return list;
        }
        public IList<IdValueOptions> GetTaskOptions()
        {
            string option1 = "School work";
            string option2 = "House work or chores";
            string option3 = "Sports";
            string option4 = "Hanging out";
            string option5 = "TV or video games";
            string option6 = "Eating";
            string option7 = "Personal hygiene";
            string option8 = "Shopping";
            string option9 = "Other";

            string tooltip = "";

            IdValueOptions option1o = new IdValueOptions { Id = 1, Value = option1, LongName = "doing school work" };
            IdValueOptions option2o = new IdValueOptions { Id = 2, Value = option2, LongName = "doing house work or chores" };
            IdValueOptions option3o = new IdValueOptions { Id = 3, Value = option3, LongName = "playing sports " };
            IdValueOptions option4o = new IdValueOptions { Id = 4, Value = option4, LongName = "hanging out" };
            IdValueOptions option5o = new IdValueOptions { Id = 5, Value = option5, LongName = "watching TV/video games" };
            IdValueOptions option6o = new IdValueOptions { Id = 6, Value = option6, LongName = "eating" };
            IdValueOptions option7o = new IdValueOptions { Id = 7, Value = option7, LongName = "taking care of personal hygiene" };
            IdValueOptions option8o = new IdValueOptions { Id = 8, Value = option8, LongName = "shopping" };
            IdValueOptions option9o = new IdValueOptions { Id = 9, Value = option9, LongName = null };

            List<IdValueOptions> list = new List<IdValueOptions>();
            list.Add(option1o);
            list.Add(option2o);
            list.Add(option3o);
            list.Add(option4o);
            list.Add(option5o);
            list.Add(option6o);
            list.Add(option7o);
            list.Add(option8o);
            list.Add(option9o);

            return list;
        }
        public int SaveKidsTasksOnLocation(KidsTasksOnLocationDto r)
        {
            var e = ObjectMapper.GetKidsTaskOnLocation(r);
            _unitOfWork.KidsTasksOnLocationRepository.Insert(e);
            _unitOfWork.SaveChanges();

            int taskId = e.Id;
            return taskId;
        }
        public List<KidsTasksOnLocation> GetKidsTasksByProfileAndSurveyId(int profileId, int kidsSurveyId)
        {
            var list = _unitOfWork.KidsTasksOnLocationRepository
                                  .Get(x => x.ProfileId == profileId
                                            && x.KidsSurveyId == kidsSurveyId)
                                  .Select(x => new KidsTasksOnLocation()
                                  {
                                      Id = x.Id,
                                      ProfileId = x.ProfileId,
                                      KidsSurveyId = x.KidsSurveyId,
                                      KidsLocationId = x.KidsLocationId,
                                      LocationName = x.LocationName,
                                      SpentStartTime = x.SpentStartTime,
                                      SpentEndTime = x.SpentEndTime,
                                      TasksDone = x.TasksDone,
                                      TaskOther = x.TaskOther
                                  }).ToList();

            return list;
        }
        public void SaveEmoTrack(KidsEmoStageTrackedDto r)
        {
            var e = ObjectMapper.GetKidsEMoTrack(r);
            _unitOfWork.KidsEMoTrackRepository.Insert(e);
            _unitOfWork.SaveChanges();
        }
        public void AddKidsReaction(KidsReactionDto r)
        {
            var e = ObjectMapper.GetKidsReactionEntity(r);
            _unitOfWork.KidsReactionRepository.Insert(e);
        }
        public void SaveKidsReaction()
        { _unitOfWork.SaveChanges(); }
        public virtual IList<KidsReaction> GetGenericKidsReactions(int kidsSurveyId, int profileId)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<KidsReaction> result = null;

            if (!IsWebApiRequest)
            { result = HttpContext.Current.Session["SurveyResponses"] as IList<KidsReaction>; }

            if (result == null || UpdateDatabase)
            {
                try
                {
                    result = _unitOfWork.KidsReactionRepository.GetUsingNoTracking(
                        i => (i.ProfileId == profileId
                        && i.KidsSurveyId == kidsSurveyId)
                        ).ToList();
                }
                catch (Exception) { throw; }
                if (!IsWebApiRequest)
                { HttpContext.Current.Session["SurveyResponses"] = result; }
            }
            return result;
        }
        public virtual IList<KidsReaction> GetGenericKidsReactionsByTravelId(int kidsSurveyId, int profileId, int? travelId)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<KidsReaction> result = null;

            if (!IsWebApiRequest)
            { result = HttpContext.Current.Session["SurveyResponses"] as IList<KidsReaction>; }

            if (result == null || UpdateDatabase)
            {
                try
                {
                    result = _unitOfWork.KidsReactionRepository.GetUsingNoTracking(
                        i => (i.ProfileId == profileId
                        && i.KidsSurveyId == kidsSurveyId)
                        && i.KidsTravelId == travelId)
                        .ToList();
                }
                catch (Exception) { throw; }
                if (!IsWebApiRequest)
                { HttpContext.Current.Session["SurveyResponses"] = result; }
            }
            return result;
        }
        public virtual IList<KidsReaction> GetGenericKidsReactionsByLocationId(int kidsSurveyId, int profileId, int? locationId)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<KidsReaction> result = null;

            if (!IsWebApiRequest)
            { result = HttpContext.Current.Session["SurveyResponses"] as IList<KidsReaction>; }

            if (result == null || UpdateDatabase)
            {
                try
                {
                    result = _unitOfWork.KidsReactionRepository.GetUsingNoTracking(
                        i => (i.ProfileId == profileId
                        && i.KidsSurveyId == kidsSurveyId)
                        && i.KidsLocationId == locationId)
                        .ToList();
                }
                catch (Exception) { throw; }
                if (!IsWebApiRequest)
                { HttpContext.Current.Session["SurveyResponses"] = result; }
            }
            return result;
        }
        public virtual IList<KidsReaction> GetAllKidsReactions(int profileId, int kidSurveyId, int kidsEmoTrackId)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<KidsReaction> result = null;

            if (!IsWebApiRequest)
            { result = HttpContext.Current.Session["SurveyResponses"] as IList<KidsReaction>; }

            if (result == null || UpdateDatabase)
            {
                try
                {
                    result = _unitOfWork.KidsReactionRepository.GetUsingNoTracking(
                       i => (i.ProfileId == profileId
                       && i.KidsSurveyId == kidSurveyId
                       && i.KidsEmoTrackId ==  kidsEmoTrackId)).ToList();
                }
                catch (Exception)
                { throw; }

                if (!IsWebApiRequest)
                { HttpContext.Current.Session["SurveyResponses"] = result; }
            }
            return result;
        }
        public void UpdateKidsReaction(KidsReaction r)
        {
            _unitOfWork.KidsReactionRepository.Update(r);
        }
    }
}