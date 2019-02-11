using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.Models.Api;
using SANSurveyWebAPI.ViewModels.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.DTOs
{
    public static class ObjectMapper
    {
        public static MedicalUniversity GetMedicalUniversityEntity(MedicalUniversityDto p)
        {
            var entity = new MedicalUniversity
            {
                Id = p.Id,
                Name = p.Name,
                Sequence = p.Sequence
            };

            return entity;
        }
        public static MedicalUniversityDto GetMedicalUniversityDto(MedicalUniversity p)
        {
            var entity = new MedicalUniversityDto
            {
                Id = p.Id,
                Name = p.Name,
                Sequence = p.Sequence
            };

            return entity;
        }

        public static ApplicationRole GetRoleEntity(RoleDto p)
        {
            var entity = new ApplicationRole
            {
                Id = p.Id,
                Name = p.Name,
            };

            return entity;
        }
        public static RoleDto GetRoleDto(ApplicationRole p)
        {
            var entity = new RoleDto
            {
                Id = p.Id,
                Name = p.Name,
            };

            return entity;
        }
        public static WAMWithWhomTaskTime GetWAMWIthWHom(WAMWithWhomTaskTimeDto p)
        {
            var entity = new WAMWithWhomTaskTime
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                SurveyId = p.SurveyId,                
                TaskId = p.TaskId,
                QuestionId = p.QuestionId,
                WithWhomOptions = p.WithWhomOptions,
                WithWhomOther = p. WithWhomOther
            };
            return entity;
        }
        public static ResponseAffects GetResponseAffectEntity(ResponseAffectDto p)
        {
            var entity = new ResponseAffects
            {
                Id = p.Id,
                SurveyId = p.SurveyId,
                ProfileId = p.ProfileId,
                TaskId = p.TaskId,
                StartResponseDateTimeUtc = p.StartResponseDateTimeUtc,
                EndResponseDateTimeUtc = p.EndResponseDateTimeUtc,
                ShiftStartDateTime = p.ShiftStartDateTime,
                ShiftEndDateTime = p.ShiftEndDateTime,
                TaskStartDateTime = p.TaskStartDateTime,
                TaskEndDateTime = p.TaskEndDateTime,
                Question = p.Question,
                Answer = p.Answer,
                SurveyWindowStartDateTime = p.SurveyWindowStartDateTime,
                SurveyWindowEndDateTime = p.SurveyWindowEndDateTime,
                TaskOther = p.TaskOther,
                IsOtherTask = p.IsOtherTask,
            };
            return entity;
        }
        public static WAMResponse GetWAMResponseEntity(WAMResponseDto p)
        {
            var entity = new WAMResponse
            {
                Id = p.Id,
                SurveyId = p.SurveyId,
                ProfileId = p.ProfileId,
                TaskId = p.TaskId,
                StartResponseDateTimeUtc = p.StartResponseDateTimeUtc,
                EndResponseDateTimeUtc = p.EndResponseDateTimeUtc,
                ShiftStartDateTime = p.ShiftStartDateTime,
                ShiftEndDateTime = p.ShiftEndDateTime,
                TaskStartDateTime = p.TaskStartDateTime,
                TaskEndDateTime = p.TaskEndDateTime,
                Question = p.Question,
                Answer = p.Answer,
                SurveyWindowStartDateTime = p.SurveyWindowStartDateTime,
                SurveyWindowEndDateTime = p.SurveyWindowEndDateTime,
                TaskOther = p.TaskOther,
                IsOtherTask = p.IsOtherTask
            };
            return entity;
        }
        public static Response GetResponseEntity(ResponseDto p)
        {
            var entity = new Response
            {
                Id = p.Id,
                SurveyId = p.SurveyId,
                ProfileId = p.ProfileId,
                TaskId = p.TaskId,
                PageStatId = p.PageStatId,
                StartResponseDateTimeUtc = p.StartResponseDateTimeUtc,
                EndResponseDateTimeUtc = p.EndResponseDateTimeUtc,
                ShiftStartDateTime = p.ShiftStartDateTime,
                ShiftEndDateTime = p.ShiftEndDateTime,
                TaskStartDateTime = p.TaskStartDateTime,
                TaskEndDateTime = p.TaskEndDateTime,
                Question = p.Question,
                Answer = p.Answer,
                SurveyWindowStartDateTime = p.SurveyWindowStartDateTime,
                SurveyWindowEndDateTime = p.SurveyWindowEndDateTime,
                TaskOther = p.TaskOther,
                IsOtherTask = p.IsOtherTask,
                IsWardRoundTask = p.IsWardRoundTask,
                WardRoundTaskId = p.WardRoundTaskId,
                WardRoundWindowStartDateTime = p.WardRoundWindowStartDateTime,
                WardRoundWindowEndDateTime = p.WardRoundWindowEndDateTime
            };

            return entity;
        }
        public static ResponseDto GetResponseDto(Response p)
        {
            return new ResponseDto
            {
                Id = p.Id,
                SurveyId = p.SurveyId,
                ProfileId = p.ProfileId,
                TaskId = p.TaskId,
                PageStatId = p.PageStatId,
                StartResponseDateTimeUtc = p.StartResponseDateTimeUtc,
                EndResponseDateTimeUtc = p.EndResponseDateTimeUtc,
                ShiftStartDateTime = p.ShiftStartDateTime,
                ShiftEndDateTime = p.ShiftEndDateTime,
                TaskStartDateTime = p.TaskStartDateTime,
                TaskEndDateTime = p.TaskEndDateTime,
                Question = p.Question,
                Answer = p.Answer,
                SurveyWindowStartDateTime = p.SurveyWindowStartDateTime,
                SurveyWindowEndDateTime = p.SurveyWindowEndDateTime,
                TaskOther = p.TaskOther,
                IsOtherTask = p.IsOtherTask,
                IsWardRoundTask = p.IsWardRoundTask,
                WardRoundTaskId = p.WardRoundTaskId,
                WardRoundWindowStartDateTime = p.WardRoundWindowStartDateTime,
                WardRoundWindowEndDateTime = p.WardRoundWindowEndDateTime
            };
        }
        public static MyDayTaskList GetTaskListEntity(MyDayTaskListDto p)
        {
            var entity = new MyDayTaskList
            {
                Id = p.Id,
                SurveyId = p.SurveyId,
                ProfileId = p.ProfileId,
                TaskId = p.TaskId,                
            };
            return entity;
        }
        public static ProfileContract GetProfileContractEntity(ProfileContractDto p)
        {
            var entity = new ProfileContract
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                ContractType = p.ContractType,
                WorkingType = p.WorkingType,
                HoursWorkedLastMonth = p.HoursWorkedLastMonth,

            };

            return entity;
        }
        public static ProfileContractDto GetProfileContractDto(ProfileContract p)
        {
            return new ProfileContractDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                ContractType = p.ContractType,
                WorkingType = p.WorkingType,
                HoursWorkedLastMonth = p.HoursWorkedLastMonth,
            };
        }

        public static ProfileComment GetProfileCommentEntity(ProfileCommentDto p)
        {
            var entity = new ProfileComment
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                Comment = p.Comment
            };

            return entity;
        }
        public static ProfileCommentDto GetProfileCommentDto(ProfileComment p)
        {
            return new ProfileCommentDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                Comment = p.Comment
            };
        }

        public static ProfileDemographic GetProfileDemographicEntity(ProfileDemographicDto p)
        {
            var entity = new ProfileDemographic
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                BirthYear = p.BirthYear,
                Gender = p.Gender,
                UniversityAttended = p.UniversityAttended,
                UniversityAttendedOtherText = p.UniversityAttendedOtherText,
                MaritialStatus = p.MaritialStatus,
                IsCaregiverAdult = p.IsCaregiverAdult,
                IsCaregiverChild = p.IsCaregiverChild,
                IsUniversityBritish = p.IsUniversityBritish,
                IsLeadership = p.IsLeadership
            };

            return entity;
        }
        public static ProfileDemographicDto GetProfileDemographicDto(ProfileDemographic p)
        {
            return new ProfileDemographicDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                BirthYear = p.BirthYear,
                Gender = p.Gender,
                UniversityAttended = p.UniversityAttended,
                UniversityAttendedOtherText = p.UniversityAttendedOtherText,
                MaritialStatus = p.MaritialStatus,
                IsCaregiverAdult = p.IsCaregiverAdult,
                IsCaregiverChild = p.IsCaregiverChild,
                IsUniversityBritish = p.IsUniversityBritish,
                IsLeadership = p.IsLeadership
            };
        }

        public static ProfilePlacement GetProfilePlacementEntity(ProfilePlacementDto p)
        {
            var entity = new ProfilePlacement
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                PlacementStartYear = p.PlacementStartYear,
                PlacementStartMonth = p.PlacementStartMonth,
                PlacementIsInHospital = p.PlacementIsInHospital,
                PlacementHospitalName = p.PlacementHospitalName,
                PlacementHospitalNameOther = p.PlacementHospitalNameOther,
                PlacementHospitalStartMonth = p.PlacementHospitalStartMonth,
                PlacementHospitalStartYear = p.PlacementHospitalStartYear
            };

            return entity;
        }
        public static ProfilePlacementDto GetProfilePlacementDto(ProfilePlacement p)
        {
            return new ProfilePlacementDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                PlacementStartYear = p.PlacementStartYear,
                PlacementStartMonth = p.PlacementStartMonth,
                PlacementIsInHospital = p.PlacementIsInHospital,
                PlacementHospitalName = p.PlacementHospitalName,
                PlacementHospitalNameOther = p.PlacementHospitalNameOther,
                PlacementHospitalStartMonth = p.PlacementHospitalStartMonth,
                PlacementHospitalStartYear = p.PlacementHospitalStartYear
            };
        }

        public static ProfileTaskTime GetProfileTaskTimeEntity(ProfileTaskTimeDto p)
        {
            var entity = new ProfileTaskTime
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                ClinicalActualTime = p.ClinicalActualTime,
                ResearchActualTime = p.ResearchActualTime,
                TeachingLearningActualTime = p.TeachingLearningActualTime,
                AdminActualTime = p.AdminActualTime,
                ClinicalDesiredTime = p.ClinicalDesiredTime,
                ResearchDesiredTime = p.ResearchDesiredTime,
                TeachingLearningDesiredTime = p.TeachingLearningDesiredTime,
                AdminDesiredTime = p.AdminDesiredTime,
            };

            return entity;
        }
        public static ProfileTaskTimeDto GetProfileTaskTimeDto(ProfileTaskTime p)
        {
            return new ProfileTaskTimeDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                ClinicalActualTime = p.ClinicalActualTime,
                ResearchActualTime = p.ResearchActualTime,
                TeachingLearningActualTime = p.TeachingLearningActualTime,
                AdminActualTime = p.AdminActualTime,
                ClinicalDesiredTime = p.ClinicalDesiredTime,
                ResearchDesiredTime = p.ResearchDesiredTime,
                TeachingLearningDesiredTime = p.TeachingLearningDesiredTime,
                AdminDesiredTime = p.AdminDesiredTime

            };
        }

        public static ProfileTraining GetProfileTrainingEntity(ProfileTrainingDto p)
        {
            var entity = new ProfileTraining
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                TrainingStartYear = p.TrainingStartYear,
                IsTrainingBreak = p.IsTrainingBreak,
                TrainingBreakLengthMonths = p.TrainingBreakLengthMonths
            };

            return entity;
        }
        public static ProfileTrainingDto GetProfileTrainingDto(ProfileTraining p)
        {
            return new ProfileTrainingDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                TrainingStartYear = p.TrainingStartYear,
                IsTrainingBreak = p.IsTrainingBreak,
                TrainingBreakLengthMonths = p.TrainingBreakLengthMonths

            };
        }
        public static MyDayErrorLogs GetMyDayErroLogs_Entity(MyDayErrorLogs_Dto myday)
        {
            var entity = new MyDayErrorLogs
            {
                Id = myday.Id,
                ProfileId = myday.ProfileId,
                SurveyUID = myday.SurveyUID,
                ErrorMessage = myday.ErrorMessage,
                AccessedDateTime = myday.AccessedDateTime,
                HtmlContent = myday.HtmlContent,
            };
            return entity;
        }
        public static ExitSurvey_Page3 GetExitSurvey_Page3_Entity(ExitSurveyPage3_Dto p)
        {
            var entity = new ExitSurvey_Page3
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                Q1 = p.Q1,
                Q2 = p.Q2,

            };

            return entity;
        }
        public static ExitSurveyPage3_Dto GetExitSurvey_Page3Dto(ExitSurvey_Page3 p)
        {
            return new ExitSurveyPage3_Dto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                Q1 = p.Q1,
                Q2 = p.Q2,
            };
        }


        public static ExitSurvey_Page6 GetExitSurvey_Page6_Entity(ExitSurveyPage6_Dto p)
        {
            var entity = new ExitSurvey_Page6
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                Q1 = p.Q1,
                Q2 = p.Q2,
                Q1Other = p.Q1Other,
                Q2Other = p.Q2Other,
            };

            return entity;
        }
        public static ExitSurveyPage6_Dto GetExitSurvey_Page6Dto(ExitSurvey_Page6 p)
        {
            return new ExitSurveyPage6_Dto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                Q1 = p.Q1,
                Q2 = p.Q2,
                Q1Other = p.Q1Other,
                Q2Other = p.Q2Other,


            };
        }



        public static ExitSurvey_Page8 GetExitSurvey_Page8_Entity(ExitSurveyPage8_Dto p)
        {
            var entity = new ExitSurvey_Page8
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                Q1 = p.Q1,
                Q2 = p.Q2,
                Q3 = p.Q3,
                Q4 = p.Q4,
                Q5 = p.Q5,
                Q6 = p.Q6,
                Q7 = p.Q7,
                Q8 = p.Q8,
            };

            return entity;
        }
        public static ExitSurveyPage8_Dto GetExitSurvey_Page8Dto(ExitSurvey_Page8 p)
        {
            return new ExitSurveyPage8_Dto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                Q1 = p.Q1,
                Q2 = p.Q2,
                Q3 = p.Q3,
                Q4 = p.Q4,
                Q5 = p.Q5,
                Q6 = p.Q6,
                Q7 = p.Q7,
                Q8 = p.Q8,

            };
        }

       


        
        public static ExitSurveyPage11_Dto GetExitSurvey_Page11Dto(ExitSurvey_Page11 p)
        {
            return new ExitSurveyPage11_Dto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                QnId = p.QnId,
               Options = p.Options,
               OtherOption = p.OtherOption,
            };
        }


        public static ExitSurvey_Page13 GetExitSurvey_Page13_Entity(ExitSurveyPage13_Dto p)
        {
            var entity = new ExitSurvey_Page13
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                Q1_Applicable = p.Q1_Applicable,
                Q1_Year = p.Q1_Year,
                Q2_PTWork = p.Q2_PTWork,
                Q2_Other = p.Q2_Other,
                Q3_NoOfPeople = p.Q3_NoOfPeople,
                Q4_Martial = p.Q4_Martial,
                Q5_PartnershipMarried = p.Q5_PartnershipMarried,
            };

            return entity;
        }
        public static ExitSurveyPage13_Dto GetExitSurvey_Page13Dto(ExitSurvey_Page13 p)
        {
            return new ExitSurveyPage13_Dto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                Q1_Applicable = p.Q1_Applicable,
                Q1_Year = p.Q1_Year,
                Q2_PTWork = p.Q2_PTWork,
                Q2_Other = p.Q2_Other,
                Q3_NoOfPeople = p.Q3_NoOfPeople,
                Q4_Martial = p.Q4_Martial,
                Q5_PartnershipMarried = p.Q5_PartnershipMarried,
            };
        }


        public static ExitSurvey_Page14 GetExitSurvey_Page14_Entity(ExitSurveyPage14_Dto p)
        {
            var entity = new ExitSurvey_Page14
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                Q1 = p.Q1,

            };

            return entity;
        }
        public static ExitSurveyPage14_Dto GetExitSurvey_Page14Dto(ExitSurvey_Page14 p)
        {
            return new ExitSurveyPage14_Dto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                Q1 = p.Q1,
            };
        }
        public static ExitSurvey_Feedback GetExitSurvey_Feedback_Entity(ExitSurveyFeedback_Dto p)
        {
            var entity = new ExitSurvey_Feedback
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                feedbackComments = p.feedbackComments,

            };

            return entity;
        }
        public static ExitSurveyFeedback_Dto GetExitSurvey_FeedbackDto(ExitSurvey_Feedback p)
        {
            return new ExitSurveyFeedback_Dto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                feedbackComments = p.feedbackComments,
            };
        }


        public static ProfileWellbeing GetProfileWellbeingEntity(ProfileWellbeingDto p)
        {
            var entity = new ProfileWellbeing
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                SwbLife = p.SwbLife,
                SwbHome = p.SwbHome,
                SwbJob = p.SwbJob,
            };

            return entity;
        }
        public static ProfileWellbeingDto GetProfileWellbeingDto(ProfileWellbeing p)
        {
            return new ProfileWellbeingDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                SwbLife = p.SwbLife,
                SwbHome = p.SwbHome,
                SwbJob = p.SwbJob,


            };
        }

        public static Profile GetProfileEntity(ProfileDto p)
        {
            var entity = new Profile
            {
                Id = p.Id,
                CreatedDateTimeUtc = p.CreatedDateTimeUtc,
                LastUpdatedDateTimeUtc = p.LastUpdatedDateTimeUtc,
                Name = p.Name,
                MobileNumber = p.MobileNumber,
                LoginEmail = StringCipher.EncryptRfc2898(p.LoginEmail),
                CurrentLevelOfTraining = p.CurrentLevelOfTraining,
                UserId = p.UserId,
                RegistrationProgressNext = p.RegistrationProgressNext,
                ExitSurveyProgressNext = p.ExitSurveyProgressNext,

                Uid = p.Uid,
                MaxStep = p.MaxStep,
                MaxStepExitSurvey = p.MaxStepExitSurvey,
                OffSetFromUTC = p.OffSetFromUTC,
                RegistrationEmailJobId = p.RegistrationEmailJobId,
                RegistrationSmsJobId = p.RegistrationSmsJobId,
                RegisteredDateTimeUtc = p.RegisteredDateTimeUtc,
                ProfileTaskType = p.ProfileTaskType,
                Incentive = p.Incentive,

                MaxExitV2Step = p.MaxExitV2Step,
                ClientName = p.ClientName,
                ClientInitials = p.ClientInitials,
            };

            return entity;
        }
        public static ProfileDto GetProfileDto(Profile p)
        {
            return new ProfileDto
            {
                Id = p.Id,
                CreatedDateTimeUtc = p.CreatedDateTimeUtc,
                LastUpdatedDateTimeUtc = p.LastUpdatedDateTimeUtc,
                Name = p.Name,
                MobileNumber = p.MobileNumber,
                LoginEmail = StringCipher.DecryptRfc2898(p.LoginEmail),
                CurrentLevelOfTraining = p.CurrentLevelOfTraining,
                UserId = p.UserId,
                RegistrationProgressNext = p.RegistrationProgressNext,
                ExitSurveyProgressNext = p.ExitSurveyProgressNext,
                Uid = p.Uid,
                MaxStep = p.MaxStep,
                MaxStepExitSurvey = p.MaxStepExitSurvey,
                OffSetFromUTC = p.OffSetFromUTC,
                RegistrationEmailJobId = p.RegistrationEmailJobId,
                RegistrationSmsJobId = p.RegistrationSmsJobId,
                RegisteredDateTimeUtc = p.RegisteredDateTimeUtc,
                ProfileTaskType = p.ProfileTaskType,
                Incentive = p.Incentive,
                MaxExitV2Step = p.MaxExitV2Step,
                ClientName = p.ClientName,
                ClientInitials = p.ClientInitials,
            };
        }

        public static ProfileRoster GetRosterEntity(ProfileRosterDto p)
        {
            var entity = new ProfileRoster
            {
                Id = p.Id,
                Name = p.Name,
                IsAllDay = p.IsAllDay,
                Start = p.Start,
                End = p.End,
                StartUtc = p.StartUtc,
                EndUtc = p.EndUtc,
                RecurrenceRule = p.RecurrenceRule,
                RecurrenceID = p.RecurrenceID,
                RecurrenceException = p.RecurrenceException,
                ProfileId = p.ProfileId,
                Description = p.Description,
                StartTimezone = p.StartTimezone,
                EndTimezone = p.EndTimezone,
            };

            return entity;
        }
        public static ProfileRoster GetProfileRosterEntity(ProfileRosterDto p)
        {

            ProfileRoster e = new ProfileRoster();
            e.Id = p.Id;
            e.Name = p.Name;
            e.IsAllDay = p.IsAllDay;
            e.Start = p.Start;
            e.End = p.End;
            e.StartUtc = p.StartUtc;
            e.EndUtc = p.EndUtc;
            e.RecurrenceRule = p.RecurrenceRule;
            e.RecurrenceID = p.RecurrenceID;
            e.RecurrenceException = p.RecurrenceException;
            e.ProfileId = p.ProfileId;
            e.Description = p.Description;
            e.StartTimezone = p.StartTimezone;
            e.EndTimezone = p.EndTimezone;
            return e;
        }

        public static ProfileRosterDto GetProfileRosterDto(ProfileRoster p)
        {

            ProfileRosterDto e = new ProfileRosterDto();
            e.Id = p.Id;
            e.Name = p.Name;
            e.IsAllDay = p.IsAllDay;
            e.Start = p.Start;
            e.End = p.End;
            e.StartUtc = p.StartUtc;
            e.EndUtc = p.EndUtc;
            e.RecurrenceRule = p.RecurrenceRule;
            e.RecurrenceID = p.RecurrenceID;
            e.RecurrenceException = p.RecurrenceException;
            e.ProfileId = p.ProfileId;
            e.Description = p.Description;
            e.StartTimezone = p.StartTimezone;
            e.EndTimezone = p.EndTimezone;
            return e;
        }


        //public static RecurrentSurveyDetailsViewModel GetProfileRosterViewModel(ProfileRoster p, Survey s, List<HangfireStateCustomDto> currStates = null)
        //{

        //    RecurrentSurveyDetailsViewModel e = new RecurrentSurveyDetailsViewModel();
        //    e.ProfileRosterItemId = p.Id;
        //    e.ProfileId = p.ProfileId;
        //    e.Description = p.Description;
        //    e.HasSurvey = false;
        //    e.SurveyId = s.Id;
        //    e.SurveyStatus = string.Empty;
        //    return e;
        //}

        public static RecurrentSurveyDetailsViewModel GetRecurrentSurveyDetails(ProfileRoster p, IEnumerable<Survey> surveyListByProfile, List<HangfireStateCustomDto> jobStauses = null)
        {

            RecurrentSurveyDetailsViewModel e = new RecurrentSurveyDetailsViewModel();
            e.ProfileRosterItemId = p.Id;
            e.ProfileId = p.ProfileId;
            e.Description = p.Description;

            var surveys = surveyListByProfile.Where(s => s.RosterItemId == p.Id).ToList();
            if (surveys.Count() > 0)
            {
                e.SurveyId = surveys.Single().Id;
                e.HasSurvey = true;
                e.SurveyStatus = surveys.Single().SurveyProgressNext;
            }
            else
            {
                e.HasSurvey = false;
                e.SurveyId = null;
                e.SurveyStatus = string.Empty;
            }


            return e;
        }


        public static Survey GetSurveyEntity(SurveyDto p)
        {
            var entity = new Survey
            {
                Id = p.Id,
                Uid = p.Uid,
                ProfileId = p.ProfileId,
                RosterItemId = p.RosterItemId,
                CreatedDateTimeUtc = p.CreatedDateTimeUtc,
                SurveyUserStartDateTimeUtc = p.SurveyUserStartDateTimeUtc,
                SurveyUserCompletedDateTimeUtc = p.SurveyUserCompletedDateTimeUtc,
                SurveyProgressNext = p.SurveyProgressNext,
                MaxStep = p.MaxStep,
                Status = p.Status,
                SurveyWindowStartDateTime = p.SurveyWindowStartDateTime,
                SurveyWindowStartDateTimeUtc = p.SurveyWindowStartDateTimeUtc,
                SurveyWindowEndDateTime = p.SurveyWindowEndDateTime,
                SurveyWindowEndDateTimeUtc = p.SurveyWindowEndDateTimeUtc,
                SurveyExpiryDateTime = p.SurveyExpiryDateTime,
                SurveyExpiryDateTimeUtc = p.SurveyExpiryDateTimeUtc,
                SysGenRandomNumber = p.SysGenRandomNumber,
                SurveyDescription = p.SurveyDescription,
                CurrTaskStartTime = p.CurrTaskStartTime,
                CurrTaskEndTime = p.CurrTaskEndTime,
                NextTaskStartTime = p.NextTaskStartTime,
                RemainingDuration = p.RemainingDuration,
                FirstQuestion = p.FirstQuestion,
                CurrTask = p.CurrTask,
                IsOnCall = p.IsOnCall,
                AddTaskId = p.AddTaskId,
                WRCurrTaskId = p.WRCurrTaskId,
                WRCurrTasksId = p.WRCurrTasksId,
                WRRemainingDuration = p.WRRemainingDuration,
                WRCurrTaskStartTime = p.WRCurrTaskStartTime,
                WRCurrTaskEndTime = p.WRCurrTaskEndTime,
                WRNextTaskStartTime = p.WRNextTaskStartTime,
                WRCurrWindowEndTime = p.WRCurrWindowEndTime,
                WRCurrWindowStartTime = p.WRCurrWindowStartTime,
                Comment = p.Comment
            };

            return entity;
        }
        public static SurveyDto GetSurveyDto(Survey p)
        {
            return new SurveyDto
            {
                Id = p.Id,
                Uid = p.Uid,
                ProfileId = p.ProfileId,
                RosterItemId = p.RosterItemId,
                CreatedDateTimeUtc = p.CreatedDateTimeUtc,
                SurveyUserStartDateTimeUtc = p.SurveyUserStartDateTimeUtc,
                SurveyUserCompletedDateTimeUtc = p.SurveyUserCompletedDateTimeUtc,
                SurveyProgressNext = p.SurveyProgressNext,
                MaxStep = p.MaxStep,
                Status = p.Status,
                SurveyWindowStartDateTime = p.SurveyWindowStartDateTime,
                SurveyWindowStartDateTimeUtc = p.SurveyWindowStartDateTimeUtc,
                SurveyWindowEndDateTime = p.SurveyWindowEndDateTime,
                SurveyWindowEndDateTimeUtc = p.SurveyWindowEndDateTimeUtc,
                SurveyExpiryDateTime = p.SurveyExpiryDateTime,
                SurveyExpiryDateTimeUtc = p.SurveyExpiryDateTimeUtc,
                SysGenRandomNumber = p.SysGenRandomNumber,
                SurveyDescription = p.SurveyDescription,
                CurrTaskStartTime = p.CurrTaskStartTime,
                CurrTaskEndTime = p.CurrTaskEndTime,
                NextTaskStartTime = p.NextTaskStartTime,
                RemainingDuration = p.RemainingDuration,
                FirstQuestion = p.FirstQuestion,
                CurrTask = p.CurrTask,
                IsOnCall = p.IsOnCall,
                AddTaskId = p.AddTaskId,
                WRCurrTaskId = p.WRCurrTaskId,
                WRCurrTasksId = p.WRCurrTasksId,
                WRRemainingDuration = p.WRRemainingDuration,
                WRCurrTaskStartTime = p.WRCurrTaskStartTime,
                WRCurrTaskEndTime = p.WRCurrTaskEndTime,
                WRNextTaskStartTime = p.WRNextTaskStartTime,
                WRCurrWindowEndTime = p.WRCurrWindowEndTime,
                WRCurrWindowStartTime = p.WRCurrWindowStartTime,
                Comment = p.Comment
            };
        }
        public static MyDayTaskListDto GetTaskListDto(MyDayTaskList p)
        {
            return new MyDayTaskListDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                SurveyId = p.SurveyId,
                TaskId = p.TaskId,
                TaskStartDateCurrentTime = p.TaskStartDateCurrentTime,
                TaskEndDateCurrentTime = p.TaskEndDateCurrentTime,
                TaskStartDateTimeUtc = p.TaskStartDateTimeUtc,
                TaskEndDateTimeUtc = p.TaskEndDateTimeUtc
            };
        }

        public static PageStat GetPageStatEntity(PageStatDto p)
        {
            var entity = new PageStat
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                SurveyId = p.SurveyId,
                TaskId = p.TaskId,
                WholePageIndicator = p.WholePageIndicator,
                PageName = p.PageName,
                PageType = p.PageType,
                PageAction = p.PageAction,
                Remark = p.Remark,
                PageDateTimeUtc = p.PageDateTimeUtc

            };

            return entity;
        }
        public static PageStatDto GetPageStatDto(PageStat p)
        {
            var entity = new PageStatDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                SurveyId = p.SurveyId,
                TaskId = p.TaskId,
                WholePageIndicator = p.WholePageIndicator,
                PageName = p.PageName,
                PageType = p.PageType,
                PageAction = p.PageAction,
                Remark = p.Remark,
                PageDateTimeUtc = p.PageDateTimeUtc
            };

            return entity;
        }


        public static BirthYear GetBirthYearsEntity(BirthYearDto p)
        {
            var entity = new BirthYear
            {
                Id = p.Id,
                Name = p.Name,
                Sequence = p.Sequence
            };

            return entity;
        }
        public static BirthYearDto GetBirthYearsDto(BirthYear p)
        {
            var entity = new BirthYearDto
            {
                Id = p.Id,
                Name = p.Name,
                Sequence = p.Sequence
            };

            return entity;
        }


        public static Ethinicity GetEthinicityEntity(EthinicityDto p)
        {
            var entity = new Ethinicity
            {
                Id = p.Id,
                Name = p.Name,
                Sequence = p.Sequence
            };

            return entity;
        }
        public static EthinicityDto GetEthinicityDto(Ethinicity p)
        {
            var entity = new EthinicityDto
            {
                Id = p.Id,
                Name = p.Name,
                Sequence = p.Sequence
            };

            return entity;
        }


        public static Specialty GetSpecialityEntity(SpecialityDto p)
        {
            var entity = new Specialty
            {
                Id = p.Id,
                Name = p.Name,
                Sequence = p.Sequence
            };

            return entity;
        }
        public static SpecialityDto GetSpecialityDto(Specialty p)
        {
            var entity = new SpecialityDto
            {
                Id = p.Id,
                Name = p.Name,
                Sequence = p.Sequence
            };

            return entity;
        }


        public static TaskItem GetTaskItemEntity(TaskItemDto p)
        {
            var entity = new TaskItem
            {
                Id = p.Id,
                ShortName = p.ShortName,
                LongName = p.LongName,
                Type = p.Type,
                Sequence = p.Sequence,
                WardRoundIndicator = p.WardRoundIndicator,
                OtherTaskIndicator = p.OtherTaskIndicator,
                IsWardRoundTask = p.IsWardRoundTask,
            };

            return entity;
        }
        public static TaskItemDto GetTaskItemDto(TaskItem p)
        {
            var entity = new TaskItemDto
            {
                Id = p.Id,
                ShortName = p.ShortName,
                LongName = p.LongName,
                Type = p.Type,
                Sequence = p.Sequence,
                WardRoundIndicator = p.WardRoundIndicator,
                OtherTaskIndicator = p.OtherTaskIndicator,
                IsWardRoundTask = p.IsWardRoundTask,
            };

            return entity;
        }
        public static MyDayTaskListDto GetMyDayTaskItemDto(MyDayTaskList p)
        {
            var entity = new MyDayTaskListDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                SurveyId = p.SurveyId,
                TaskId = p.TaskId,
                TaskName = p.TaskName,
                TaskCategoryId = p.TaskCategoryId,
                TaskStartDateCurrentTime = p.TaskStartDateCurrentTime,
                TaskEndDateCurrentTime = p.TaskEndDateCurrentTime,
                TaskStartDateTimeUtc = p.TaskStartDateTimeUtc,
                TaskEndDateTimeUtc = p.TaskEndDateTimeUtc
            };

            return entity;
        }


        public static JobLogShiftReminderEmail GetJobLogShiftReminderEmailEntity(JobLogShiftReminderEmailDto p)
        {
            var entity = new JobLogShiftReminderEmail
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                //Email = p.Email,
                JobMethod = p.JobMethod,
                HangfireJobId = p.HangfireJobId,
                RunAfterMin = p.RunAfterMin,
                CreatedDateTimeServer = p.CreatedDateTimeServer,
                CreatedDateTimeUtc = p.CreatedDateTimeUtc
            };

            return entity;
        }
        public static JobLogShiftReminderEmailDto GetJobLogShiftReminderEmailDto(JobLogShiftReminderEmail p)
        {
            var entity = new JobLogShiftReminderEmailDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                //Email = p.Email,
                JobMethod = p.JobMethod,
                HangfireJobId = p.HangfireJobId,
                RunAfterMin = p.RunAfterMin,
                CreatedDateTimeServer = p.CreatedDateTimeServer,
                CreatedDateTimeUtc = p.CreatedDateTimeUtc
            };

            return entity;
        }


        public static JobLogStartSurveyReminderEmail GetJobLogStartSurveyReminderEmailEntity(JobLogStartSurveyReminderEmailDto p)
        {
            var entity = new JobLogStartSurveyReminderEmail
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                //Email = p.Email,
                JobMethod = p.JobMethod,
                HangfireJobId = p.HangfireJobId,
                RunAfterMin = p.RunAfterMin,
                CreatedDateTimeServer = p.CreatedDateTimeServer,
                CreatedDateTimeUtc = p.CreatedDateTimeUtc
            };

            return entity;
        }
        public static JobLogStartSurveyReminderEmailDto GetJobLogStartSurveyReminderEmailDto(JobLogStartSurveyReminderEmail p)
        {
            var entity = new JobLogStartSurveyReminderEmailDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                //Email = p.Email,
                JobMethod = p.JobMethod,
                HangfireJobId = p.HangfireJobId,
                RunAfterMin = p.RunAfterMin,
                CreatedDateTimeServer = p.CreatedDateTimeServer,
                CreatedDateTimeUtc = p.CreatedDateTimeUtc
            };

            return entity;
        }


        public static JobLogCompleteSurveyReminderEmail GetJobLogCompleteSurveyReminderEmailEntity(JobLogCompleteSurveyReminderEmailDto p)
        {
            var entity = new JobLogCompleteSurveyReminderEmail
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                //Email = p.Email,
                JobMethod = p.JobMethod,
                HangfireJobId = p.HangfireJobId,
                RunAfterMin = p.RunAfterMin,
                CreatedDateTimeServer = p.CreatedDateTimeServer,
                CreatedDateTimeUtc = p.CreatedDateTimeUtc
            };

            return entity;
        }
        public static JobLogCompleteSurveyReminderEmailDto GetJobLogCompleteSurveyReminderEmailDto(JobLogCompleteSurveyReminderEmail p)
        {
            var entity = new JobLogCompleteSurveyReminderEmailDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                //Email = p.Email,
                JobMethod = p.JobMethod,
                HangfireJobId = p.HangfireJobId,
                RunAfterMin = p.RunAfterMin,
                CreatedDateTimeServer = p.CreatedDateTimeServer,
                CreatedDateTimeUtc = p.CreatedDateTimeUtc
            };

            return entity;
        }

        public static JobLogExpiringSoonSurveyNotStartedReminderEmail GetJobLogExpiringSoonSurveyNotStartedReminderEmailEntity(JobLogExpiringSoonSurveyNotStartedReminderEmailDto p)
        {
            var entity = new JobLogExpiringSoonSurveyNotStartedReminderEmail
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                //Email = p.Email,
                JobMethod = p.JobMethod,
                HangfireJobId = p.HangfireJobId,
                RunAfterMin = p.RunAfterMin,
                CreatedDateTimeServer = p.CreatedDateTimeServer,
                CreatedDateTimeUtc = p.CreatedDateTimeUtc
            };

            return entity;
        }
        public static JobLogExpiringSoonSurveyNotStartedReminderEmailDto GetJobLogExpiringSoonSurveyNotStartedReminderEmailDto(JobLogExpiringSoonSurveyNotStartedReminderEmail p)
        {
            var entity = new JobLogExpiringSoonSurveyNotStartedReminderEmailDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                //Email = p.Email,
                JobMethod = p.JobMethod,
                HangfireJobId = p.HangfireJobId,
                RunAfterMin = p.RunAfterMin,
                CreatedDateTimeServer = p.CreatedDateTimeServer,
                CreatedDateTimeUtc = p.CreatedDateTimeUtc
            };

            return entity;
        }

        public static JobLogExpiringSoonSurveyNotCompletedReminderEmail GetJobLogExpiringSoonSurveyNotCompletedReminderEmailEntity(JobLogExpiringSoonSurveyNotCompletedReminderEmailDto p)
        {
            var entity = new JobLogExpiringSoonSurveyNotCompletedReminderEmail
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                //Email = p.Email,
                JobMethod = p.JobMethod,
                HangfireJobId = p.HangfireJobId,
                RunAfterMin = p.RunAfterMin,
                CreatedDateTimeServer = p.CreatedDateTimeServer,
                CreatedDateTimeUtc = p.CreatedDateTimeUtc
            };

            return entity;
        }
        public static JobLogExpiringSoonSurveyNotCompletedReminderEmailDto GetJobLogExpiringSoonSurveyNotCompletedReminderEmailDto(JobLogExpiringSoonSurveyNotCompletedReminderEmail p)
        {
            var entity = new JobLogExpiringSoonSurveyNotCompletedReminderEmailDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                //Email = p.Email,
                JobMethod = p.JobMethod,
                HangfireJobId = p.HangfireJobId,
                RunAfterMin = p.RunAfterMin,
                CreatedDateTimeServer = p.CreatedDateTimeServer,
                CreatedDateTimeUtc = p.CreatedDateTimeUtc
            };

            return entity;
        }

        #region WAMDemo

        public static WAMWellBeingDto GetWAMWellbeingDto(WAMWellBeing p)
        {
            return new WAMWellBeingDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                SwbLife = p.SwbLife,
                SwbHome = p.SwbHome,
                SwbJob = p.SwbJob,
            };
        }
        public static WAMWellBeing GetWAMWellbeingEntity(WAMWellBeingDto p)
        {
            var entity = new WAMWellBeing
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                SwbLife = p.SwbLife,
                SwbHome = p.SwbHome,
                SwbJob = p.SwbJob,
            };
            return entity;
        }
        public static WAMProfileRoleDto GetWAMProfileRoleDto(WAMProfileRole p)
        {
            return new WAMProfileRoleDto
            { Id = p.Id, ProfileId = p.ProfileId, MyProfileRole = p.MyProfileRole, StartYear = p.StartYear };
        }

        public static WAMProfileRole GetWAMProfileRoleEntity(WAMProfileRoleDto profileRoledto)
        {
            var entity = new WAMProfileRole
            {
                Id = profileRoledto.Id,
                ProfileId = profileRoledto.ProfileId,
                StartYear = profileRoledto.StartYear,
                MyProfileRole = profileRoledto.MyProfileRole
            };

            return entity;
        }
        public static WAMTasksDto GetWAMTasksDto(WAMTasks p)
        {
            return new WAMTasksDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                ClinicalActualTime = p.ClinicalActualTime,
                ResearchActualTime = p.ResearchActualTime,
                TeachingLearningActualTime = p.TeachingLearningActualTime,
                AdminActualTime = p.AdminActualTime,
                ClinicalDesiredTime = p.ClinicalDesiredTime,
                ResearchDesiredTime = p.ResearchDesiredTime,
                TeachingLearningDesiredTime = p.TeachingLearningDesiredTime,
                AdminDesiredTime = p.AdminDesiredTime
            };
        }
        public static WAMTasks GetWAMTasksEntity(WAMTasksDto p)
        {
            var entity = new WAMTasks
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                ClinicalActualTime = p.ClinicalActualTime,
                ResearchActualTime = p.ResearchActualTime,
                TeachingLearningActualTime = p.TeachingLearningActualTime,
                AdminActualTime = p.AdminActualTime,
                ClinicalDesiredTime = p.ClinicalDesiredTime,
                ResearchDesiredTime = p.ResearchDesiredTime,
                TeachingLearningDesiredTime = p.TeachingLearningDesiredTime,
                AdminDesiredTime = p.AdminDesiredTime,
            };

            return entity;
        }
        public static WAMDemographicsDto GetWAMDemographicsDto(WAMDemographics p)
        {
            return new WAMDemographicsDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                BirthYear = p.BirthYear,
                Gender = p.Gender,
                IsCaregiverChild = p.IsCaregiverChild,
            };
        }
        public static WAMDemographics GetWAMDemographicsEntity(WAMDemographicsDto p)
        {
            var entity = new WAMDemographics
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                BirthYear = p.BirthYear,
                Gender = p.Gender,
                IsCaregiverChild = p.IsCaregiverChild,
            };
            return entity;
        }
        public static WAMIntentionsDto GetWAMIntentionsDto(WAMIntentions p)
        {
            return new WAMIntentionsDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                SameEmployer = p.SameEmployer,
                SameIndustry = p.SameIndustry,
            };
        }
        public static WAMIntentions GetWAMIntentionsEntity(WAMIntentionsDto p)
        {
            var entity = new WAMIntentions
            {
               Id = p.Id,
               ProfileId = p.ProfileId,
               SameEmployer = p.SameEmployer,
               SameIndustry = p.SameIndustry
            };

            return entity;
        }
        public static WAMFeedback GetWAMFeedbackEntity(WAMFeedbackDto p)
        {
            var entity = new WAMFeedback
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                Feedback = p.Feedback
            };

            return entity;
        }
        #endregion


        #region KIDS

        public static KidsResponses GetKidsResponsesEntity(KidsResponsesDto p)
        {
            var entity = new KidsResponses
            {
                Id = p.Id,
                KidsSurveyId = p.KidsSurveyId,
                ProfileId = p.ProfileId,
                KidsTaskId = p.KidsTaskId,
                TaskName = p.TaskName,
                QuestionId = p.QuestionId,
                Answer = p.Answer,
                SurveyDate = p.SurveyDate,
                ResponseStartTimeUTC = p.ResponseStartTimeUTC,
                ResponseEndTimeUTC = p.ResponseEndTimeUTC,
            };
            return entity;
        }
        public static KidsFeedback GetKidsFeedback(KidsFeedbackDto p)
        {
            var entity = new KidsFeedback
            {
                Id = p.Id,
                KidsSurveyId = p.KidsSurveyId,
                ProfileId = p.ProfileId,
                Comments = p.Comments,
            };
            return entity;
        }
        public static KidsSurveyDto GetKidsSurveyDto(KidsSurvey p)
        {
            return new KidsSurveyDto
            {
                Id = p.Id,
                Uid = p.Uid,
                ProfileId = p.ProfileId,
                EntryStartUTC = p.EntryStartUTC,
                EntryStartCurrent = p.EntryStartCurrent,
                EndTimeUTC = p.EndTimeUTC,
                EndTimeCurrent = p.EndTimeCurrent,
                SurveyProgress = p.SurveyProgress,
                SurveyDate = p.SurveyDate,
            };
        }
        public static KidsFeedbackDto GetKidsFeedbackDto(KidsFeedback p)
        {
            return new KidsFeedbackDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                KidsSurveyId = p.KidsSurveyId,
                Comments = p.Comments,
            };
        }
        public static KidsTasksDto GetKidsTasksDto(KidsTasks p)
        {
            var entity = new KidsTasksDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                KidsSurveyId = p.KidsSurveyId,
                TaskName = p.TaskName,
                Venue = p.Venue,
                Travel = p.Travel,
                StartTime = p.StartTime,
                EndTime = p.EndTime,
                IsEmotionalStageCompleted = p.IsEmotionalStageCompleted,
                IsRandomlySelected = p.IsRandomlySelected,
                InOutLocation = p.InOutLocation,
                People = p.People,
                SurveyDate = p.SurveyDate,
            };

            return entity;
        }
        public static KidsLocation GetKidsLocation(KidsLocationDto p)
        {
            var entity = new KidsLocation
            {
                Id = p.Id,
                KidsSurveyId = p.KidsSurveyId,
                ProfileId = p.ProfileId,
                Location = p.Location,
                OtherLocation = p.OtherLocation,
                TimeSpentInHours = p.TimeSpentInHours,
                TimeSpentInMins = p.TimeSpentInMins,
                LocationSequence =p.LocationSequence,
            };
            return entity;
        }
        public static KidsTravel GetKidsTravelEntity(KidsTravelDto p)
        {
            var entity = new KidsTravel
            {
                Id = p.Id,
                KidsSurveyId = p.KidsSurveyId,
                ProfileId = p.ProfileId,
                FromLocationId = p.FromLocationId,
                ToLocationId = p.ToLocationId,
                ModeOfTransport = p.ModeOfTransport,
                OtherModeOfTransport = p.OtherModeOfTransport,
                TravelTimeInHours = p.TravelTimeInHours,
                TravelTimeInMins = p.TravelTimeInMins,
                TravelStartedAt = p.TravelStartedAt,
                TravelEndedAt = p.TravelEndedAt                
            };
            return entity;
        }
        public static KidsTasksOnLocation GetKidsTaskOnLocation(KidsTasksOnLocationDto p)
        {
            var entity = new KidsTasksOnLocation
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                KidsSurveyId = p.KidsSurveyId,
                KidsLocationId = p.KidsLocationId,
                LocationName = p.LocationName,
                OtherLocationName = p.OtherLocationName,
                SpentStartTime = p.SpentStartTime,
                SpentEndTime = p.SpentEndTime,
                TasksDone = p.TasksDone,
                TaskOther = p.TaskOther
            };
            return entity;
        }
        public static KidsEmoStageTracked GetKidsEMoTrack(KidsEmoStageTrackedDto p)
        {
            var entity = new KidsEmoStageTracked
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                KidsSurveyId = p.KidsSurveyId,
                TaskWhile = p.TaskWhile,
                SurveyDate = p.SurveyDate,
                TaskStartTime = p.TaskStartTime,
                TaskEndTime = p.TaskEndTime,
                KidsLocationId = p.KidsLocationId,
                KidsTravelId = p.KidsTravelId,
                LocationName = p.LocationName,
                ModeOfTransport = p.ModeOfTransport,
                KidsTaskId = p.KidsTaskId,
                TaskPerformed = p.TaskPerformed,
                IsEmoAffStageCompleted = p.IsEmoAffStageCompleted,
                SequenceToQEmo = p.SequenceToQEmo,
                OtherLocationName = p.OtherLocationName,
                OtherModeOfTransport = p.OtherModeOfTransport,
                OtherTask = p.OtherTask,
                TravelDetails = p.TravelDetails
            };
            return entity;
        }
        public static KidsReaction GetKidsReactionEntity(KidsReactionDto p)
        {
            var entity = new KidsReaction
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                KidsSurveyId = p.KidsSurveyId,
                SurveyDate = p.SurveyDate,
                ResponseStartTime = p.ResponseStartTime,
                ResponseEndTime = p.ResponseEndTime,
                KidsLocationId = p.KidsLocationId,
                KidsTravelId = p.KidsTravelId,
                KidsTaskId = p.KidsTaskId,    
                TasksPerformed = p.TasksPerformed,                           
                QuestionId = p.QuestionId,
                Answer = p.Answer,
                TaskStartTime = p.TaskStartTime,
                TaskEndTime = p.TaskEndTime,
                KidsEmoTrackId = p.KidsEmoTrackId
            };
            return entity;
        }
        #endregion

        #region "Exit Survey Version 2"

        public static SecondJobDto GetSecondJobDto(SecondJob p)
        {
            return new SecondJobDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                ExitSurveyId = p.ExitSurveyId,
                Q1 = p.Q1,
                Q2 = p.Q2,
            };
        }
        public static SecondJob GetSecondJobEntity(SecondJobDto p)
        {
            var entity = new SecondJob
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                ExitSurveyId = p.ExitSurveyId,
                Q1 = p.Q1,
                Q2 = p.Q2,
            };
            return entity;
        }
        public static ThirdWorkEnvironmentDto GetThirdWEDto(ThirdWorkEnvironment p)
        {
            return new ThirdWorkEnvironmentDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                ExitSurveyId = p.ExitSurveyId,
                Q1 = p.Q1,
                Q1Other = p.Q1Other,      
               
            };
        }
        public static ThirdWorkEnvironment GetThirdWEEntity(ThirdWorkEnvironmentDto p)
        {
            var entity = new ThirdWorkEnvironment
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                ExitSurveyId = p.ExitSurveyId,
                Q1 = p.Q1,
                Q1Other = p.Q1Other,                
            };
            return entity;
        }
        public static FifthWorkEnvironmentDto GetFifthWEDto(FifthWorkEnvironment p)
        {
            return new FifthWorkEnvironmentDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                ExitSurveyId = p.ExitSurveyId,
                Q1 = p.Q1,
                Q2 = p.Q2,
                Q3 = p.Q3,
                Q4 = p.Q4,
                Q5 = p.Q5,
                Q6 = p.Q6,
                Q7 = p.Q7,
                Q8 = p.Q8,
            };
        }
        public static FifthWorkEnvironment GetFifthWEEntity(FifthWorkEnvironmentDto p)
        {
            var entity = new FifthWorkEnvironment
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                ExitSurveyId = p.ExitSurveyId,
                Q1 = p.Q1,
                Q2 = p.Q2,
                Q3 = p.Q3,
                Q4 = p.Q4,
                Q5 = p.Q5,
                Q6 = p.Q6,
                Q7 = p.Q7,
                Q8 = p.Q8,
            };

            return entity;
        }
        public static AboutYouESDto GetAboutYouDto(AboutYouES p)
        {
            return new AboutYouESDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                ExitSurveyId = p.ExitSurveyId,
                Q1_Applicable = p.Q1_Applicable,
                Q1_Year = p.Q1_Year,
                Q2_PTWork = p.Q2_PTWork,
                Q2_Other = p.Q2_Other,
                Q3_NoOfPeople = p.Q3_NoOfPeople,
                Q4_Martial = p.Q4_Martial,
                Q5_PartnershipMarried = p.Q5_PartnershipMarried,
            };
        }
        public static AboutYouES GetAboutYouEntity(AboutYouESDto p)
        {
            var entity = new AboutYouES
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                ExitSurveyId = p.ExitSurveyId,
                Q1_Applicable = p.Q1_Applicable,
                Q1_Year = p.Q1_Year,
                Q2_PTWork = p.Q2_PTWork,
                Q2_Other = p.Q2_Other,
                Q3_NoOfPeople = p.Q3_NoOfPeople,
                Q4_Martial = p.Q4_Martial,
                Q5_PartnershipMarried = p.Q5_PartnershipMarried,
            };

            return entity;
        }
        #endregion

        #region Social Workers - Baseline or Registration 

        public static SWSubjectiveWellBeingDto GetSWSubjectiveWellbeingDto(SWSubjectiveWellBeing p)
        {
            return new SWSubjectiveWellBeingDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                SwbLife = p.SwbLife,
                SwbHome = p.SwbHome,
                SwbJob = p.SwbJob,
            };
        }

        public static SWSubjectiveWellBeing GetSWSubjectiveMWellbeingEntity(SWSubjectiveWellBeingDto p)
        {
            var entity = new SWSubjectiveWellBeing
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                SwbLife = p.SwbLife,
                SwbHome = p.SwbHome,
                SwbJob = p.SwbJob,
            };
            return entity;
        }
        public static CurrentWorkPlaceContdDto GetCurrentWorkPlaceContdDto(CurrentWorkplaceContd p)
        {
            return new CurrentWorkPlaceContdDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                WorkStatus = p.WorkStatus,
                WorkPosition = p.WorkPosition,
                WorkCountry = p.WorkCountry,
                OtherWorkCountry = p.OtherWorkCountry,
            };
        }
        public static CaseLoadDto GetCaseLoadDto(CaseLoad p)
        {
            return new CaseLoadDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,               
                Option = p.Option
            };
        }
        public static CaseLoad GetCaseLoadEntity(CaseLoadDto p)
        {
            var entity = new CaseLoad
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                Option = p.Option,
            };
            return entity;
        }
        public static CurrentWorkplaceContd GetCurrentWorkPlaceContdEntity(CurrentWorkPlaceContdDto p)
        {
            var entity = new CurrentWorkplaceContd
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                WorkStatus = p.WorkStatus,
                WorkPosition = p.WorkPosition,
                WorkCountry = p.WorkCountry,
                OtherWorkCountry = p.OtherWorkCountry,
            };
            return entity;
        }
        public static TimeAllocationDto GetTimeAllocatedDto(TimeAllocation p)
        {
            return new TimeAllocationDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                ClinicalActualTime = p.ClinicalActualTime,
                ResearchActualTime = p.ResearchActualTime,
                TeachingLearningActualTime = p.TeachingLearningActualTime,
                AdminActualTime = p.AdminActualTime,
                ClinicalDesiredTime = p.ClinicalDesiredTime,
                ResearchDesiredTime = p.ResearchDesiredTime,
                TeachingLearningDesiredTime = p.TeachingLearningDesiredTime,
                AdminDesiredTime = p.AdminDesiredTime
            };
        }
        public static TimeAllocation GetTimeAllocatedEntity(TimeAllocationDto p)
        {
            var entity = new TimeAllocation
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                ClinicalActualTime = p.ClinicalActualTime,
                ResearchActualTime = p.ResearchActualTime,
                TeachingLearningActualTime = p.TeachingLearningActualTime,
                AdminActualTime = p.AdminActualTime,
                ClinicalDesiredTime = p.ClinicalDesiredTime,
                ResearchDesiredTime = p.ResearchDesiredTime,
                TeachingLearningDesiredTime = p.TeachingLearningDesiredTime,
                AdminDesiredTime = p.AdminDesiredTime,
            };

            return entity;
        }
        public static DemographicsDto GetCWDemographics(Demographics p)
        {
            return new DemographicsDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                BirthYear = p.BirthYear,
                Gender = p.Gender,
                MaritalStatus = p.MaritalStatus,
                IsCaregiverChild = p.IsCaregiverChild,
                IsCaregiverAdult = p.IsCaregiverAdult,
                EthnicityOrRace = p.EthnicityOrRace,
            };
        }
        public static Demographics GetCWDemographicsEntity(DemographicsDto p)
        {
            var entity = new Demographics
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                BirthYear = p.BirthYear,
                Gender = p.Gender,
                MaritalStatus = p.MaritalStatus,
                IsCaregiverChild = p.IsCaregiverChild,
                IsCaregiverAdult = p.IsCaregiverAdult,
                EthnicityOrRace = p.EthnicityOrRace,
            };
            return entity;
        }
        public static EducationBackgroundDto GetEducationBackgroundDto(EducationBackground p)
        {
            return new EducationBackgroundDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,               
                BachelorsDegree = p.BachelorsDegree,
                MasterDegree = p.MasterDegree,
                PreServiceTraining = p.PreServiceTraining
            };
        }
        public static EducationBackground GetEducationBackgroundEntity(EducationBackgroundDto p)
        {
            var entity = new EducationBackground
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                BachelorsDegree = p.BachelorsDegree,
                MasterDegree = p.MasterDegree,
                PreServiceTraining = p.PreServiceTraining
            };
            return entity;
        }
        public static JobIntentionsDto GetJobIntentionsDto(JobIntentions p)
        {
            return new JobIntentionsDto
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                CurrentWorkplace = p.CurrentWorkplace,
                CurrentIndustry = p.CurrentIndustry
            };
        }
        public static JobIntentions GetJobIntentionsEntity(JobIntentionsDto p)
        {
            var entity = new JobIntentions
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                CurrentWorkplace = p.CurrentWorkplace,
                CurrentIndustry = p.CurrentIndustry
            };
            return entity;
        }
        public static CaseWorkersFeedback GetCaseWorkersFeedback(CaseWorkersFeedbackDto p)
        {
            var entity = new CaseWorkersFeedback
            {
                Id = p.Id,
                ProfileId = p.ProfileId,
                Comments = p.Comments,
            };
            return entity;
        }
        #endregion
    }
}