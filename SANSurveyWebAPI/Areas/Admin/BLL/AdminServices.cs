using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;
using SANSurveyWebAPI.Models.Api;
using SANSurveyWebAPI.DAL;
using SANSurveyWebAPI.DTOs;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using SANSurveyWebAPI.ViewModels.Web;

namespace SANSurveyWebAPI.BLL
{
    public class AdminService : IDisposable
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private JobService _jobService = new JobService();

        public AdminService()
        {
            //UserManager = _userManager ?? 
            //    HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            //RoleManager = _userManager ?? 
            //    HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        }

        public AdminService(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            //UserManager = userManager;
            //RoleManager = roleManager;
        }

        public void Dispose()
        {
            _jobService.Dispose();
            _unitOfWork.Dispose();
        }


        #region Admin
        //BirthYears

        #region BirthYears
        public IEnumerable<BirthYearDto> GetAllBirthYears()
        {
            var e = _unitOfWork
                        .BirthYearRespository
                        .GetUsingNoTracking(
                );

            var dtos = e
                       .Select(b => ObjectMapper.GetBirthYearsDto(b))
                       .ToList().AsQueryable<BirthYearDto>();
            if (dtos != null)
                return dtos;

            return null;
        }

        //internal IEnumerable<PageStatDto> GetPageStats()
        //{
        //    var e = _unitOfWork
        //                          .PageStatsRespository
        //                          .GetUsingNoTracking(
        //                  );

        //    var dtos = e
        //               .Select(b => ObjectMapper.GetPageStatDto(b))
        //               .ToList().AsQueryable<PageStatDto>();
        //    if (dtos != null)
        //        return dtos;

        //    return null;
        //}

        internal ProfileRosterDto GetProfileRoster(int rosterId)
        {
            var roster = _unitOfWork.ProfileRosterRespository.GetByID(rosterId);
            return ObjectMapper.GetProfileRosterDto(roster);
        }
        internal SurveyDto GetSurveyById(int surveyId)
        {
            var survey = _unitOfWork.SurveyRespository.GetByID(surveyId);

            return ObjectMapper.GetSurveyDto(survey);
        }




        internal JobLogShiftReminderEmailDto GetShiftReminderJobLogDetails(SurveyDto survey)
        {
            //var jobLog = _unitOfWork.JobLogShiftReminderEmailRespository.GetUsingNoTracking(r => r.ProfileRosterId == survey.RosterItemId).SingleOrDefault();
            var jobLog = _unitOfWork.JobLogShiftReminderEmailRespository.GetUsingNoTracking(r => r.ProfileRosterId == survey.RosterItemId).FirstOrDefault();

            if (jobLog != null)
            {
                var dto = ObjectMapper.GetJobLogShiftReminderEmailDto(jobLog);
                return dto;
            }
            return null;
        }

        internal List<JobLogShiftReminderEmailDto> GetShiftReminderJobLogDetailsList(SurveyDto survey)
        {
            var jobLog = _unitOfWork.JobLogShiftReminderEmailRespository
                .GetUsingNoTracking(r => r.ProfileRosterId == survey.RosterItemId);

            var shiftDtos = jobLog.Select(b => ObjectMapper.GetJobLogShiftReminderEmailDto(b))
                  .ToList().AsQueryable<JobLogShiftReminderEmailDto>().ToList();

            return shiftDtos;
        }



        internal List<HangfireStateDto> GetShiftReminderJobDetails(SurveyDto survey)
        {
            List<HangfireStateDto> hangfireJobStates = new List<HangfireStateDto>();
            var jobLog = _unitOfWork.JobLogShiftReminderEmailRespository.GetUsingNoTracking(r => r.ProfileRosterId == survey.RosterItemId);
            //var jobLog = _unitOfWork.JobLogShiftReminderEmailRespository.GetUsingNoTracking(r => r.ProfileRosterId == survey.RosterItemId).SingleOrDefault();

            if (jobLog != null)
            {
                //if (jobLog.HangfireJobId.HasValue)
                //{
                //    hangfireJobStates = _unitOfWork.GetHangfireJobStates(jobLog.HangfireJobId.Value);
                //}
                foreach (var k in jobLog)
                {
                    if (k.HangfireJobId.HasValue)
                    {
                        _unitOfWork.GetHangfireJobStates(k.HangfireJobId.Value, ref hangfireJobStates);
                    }
                }
            }
            return hangfireJobStates;
        }





        internal JobLogStartSurveyReminderEmailDto GetStartSurveyReminderJobLogDetails(SurveyDto survey)
        {
            var jobLog = _unitOfWork.JobLogStartSurveyReminderEmailRespository.GetUsingNoTracking(r => r.ProfileRosterId == survey.RosterItemId).FirstOrDefault();

            if (jobLog != null)
            {
                var dto = ObjectMapper.GetJobLogStartSurveyReminderEmailDto(jobLog);
                return dto;
            }
            return null;
        }

        internal List<JobLogStartSurveyReminderEmailDto> GetStartSurveyReminderJobLogDetailsList(SurveyDto survey)
        {
            var jobLog = _unitOfWork.JobLogStartSurveyReminderEmailRespository
                .GetUsingNoTracking(r => r.ProfileRosterId == survey.RosterItemId);

            var dtos = jobLog.Select(b => ObjectMapper.GetJobLogStartSurveyReminderEmailDto(b))
                  .ToList().AsQueryable<JobLogStartSurveyReminderEmailDto>().ToList();

            return dtos;
        }

       


        internal List<HangfireStateDto> GetStartSurveyReminderJobDetails(SurveyDto survey)
        {
            List<HangfireStateDto> hangfireJobStates = new List<HangfireStateDto>();
            var jobLog = _unitOfWork.JobLogStartSurveyReminderEmailRespository.GetUsingNoTracking(r => r.ProfileRosterId == survey.RosterItemId).FirstOrDefault();

            if (jobLog != null)
            {
                if (jobLog.HangfireJobId.HasValue)
                {
                    hangfireJobStates = _unitOfWork.GetHangfireJobStates(jobLog.HangfireJobId.Value);
                }
            }
            return hangfireJobStates;
        }





        internal JobLogCompleteSurveyReminderEmailDto GetCompleteSurveyReminderJobLogDetails(SurveyDto survey)
        {
            var jobLog = _unitOfWork.JobLogCompleteSurveyReminderEmailRespository.GetUsingNoTracking(r => r.ProfileRosterId == survey.RosterItemId).FirstOrDefault();

            if (jobLog != null)
            {
                var dto = ObjectMapper.GetJobLogCompleteSurveyReminderEmailDto(jobLog);
                return dto;
            }
            return null;
        }

        internal List<JobLogCompleteSurveyReminderEmailDto> GetCompleteSurveyReminderJobDetailsList(SurveyDto survey)
        {
            var jobLog = _unitOfWork.JobLogCompleteSurveyReminderEmailRespository
                .GetUsingNoTracking(r => r.ProfileRosterId == survey.RosterItemId);

            var dtos = jobLog.Select(b => ObjectMapper.GetJobLogCompleteSurveyReminderEmailDto(b))
                  .ToList().AsQueryable<JobLogCompleteSurveyReminderEmailDto>().ToList();

            return dtos;
        }


        internal List<HangfireStateDto> GetCompleteSurveyReminderJobDetails(SurveyDto survey)
        {
            List<HangfireStateDto> hangfireJobStates = new List<HangfireStateDto>();
            var jobLog = _unitOfWork.JobLogCompleteSurveyReminderEmailRespository.GetUsingNoTracking(r => r.ProfileRosterId == survey.RosterItemId).FirstOrDefault();

            if (jobLog != null)
            {
                if (jobLog.HangfireJobId.HasValue)
                {
                    hangfireJobStates = _unitOfWork.GetHangfireJobStates(jobLog.HangfireJobId.Value);
                }
            }
            return hangfireJobStates;
        }




        internal List<JobLogExpiringSoonSurveyNotStartedReminderEmailDto> GetExpiringSoonNotStartedSurveyReminderJobLogDetailsList(SurveyDto survey)
        {
            var jobLog = _unitOfWork.JobLogExpiringSoonSurveyNotStartedReminderEmailRespository
                .GetUsingNoTracking(r => r.ProfileRosterId == survey.RosterItemId);

            var dtos = jobLog.Select(b => ObjectMapper.GetJobLogExpiringSoonSurveyNotStartedReminderEmailDto(b))
                  .ToList().AsQueryable<JobLogExpiringSoonSurveyNotStartedReminderEmailDto>().ToList();
            return dtos;
        }




        internal JobLogExpiringSoonSurveyNotStartedReminderEmailDto GetExpiringSoonNotStartedSurveyReminderJobLogDetails(SurveyDto survey)
        {
            var jobLog = _unitOfWork.JobLogExpiringSoonSurveyNotStartedReminderEmailRespository.GetUsingNoTracking(r => r.ProfileRosterId == survey.RosterItemId).FirstOrDefault();
            if (jobLog != null)
            {
                var dto = ObjectMapper.GetJobLogExpiringSoonSurveyNotStartedReminderEmailDto(jobLog);
                return dto;
            }
            return null;
        }


        internal List<HangfireStateDto> GetExpiringSoonNotStartedSurveyReminderJobDetails(SurveyDto survey)
        {
            List<HangfireStateDto> hangfireJobStates = new List<HangfireStateDto>();
            var jobLog = _unitOfWork.JobLogExpiringSoonSurveyNotStartedReminderEmailRespository.GetUsingNoTracking(r => r.ProfileRosterId == survey.RosterItemId).FirstOrDefault();
            if (jobLog != null)
            {
                if (jobLog.HangfireJobId.HasValue)
                {
                    hangfireJobStates = _unitOfWork.GetHangfireJobStates(jobLog.HangfireJobId.Value);
                }
            }
            return hangfireJobStates;
        }



        internal List<JobLogExpiringSoonSurveyNotCompletedReminderEmailDto> GetExpiringSoonNotCompletedSurveyReminderJobLogDetailsList(SurveyDto survey)
        {
            var jobLog = _unitOfWork.JobLogExpiringSoonSurveyNotCompletedReminderEmailRespository
                .GetUsingNoTracking(r => r.ProfileRosterId == survey.RosterItemId);

            var dtos = jobLog.Select(b => ObjectMapper.GetJobLogExpiringSoonSurveyNotCompletedReminderEmailDto(b))
                  .ToList().AsQueryable<JobLogExpiringSoonSurveyNotCompletedReminderEmailDto>().ToList();
            return dtos;
        }


        internal JobLogExpiringSoonSurveyNotCompletedReminderEmailDto GetExpiringSoonNotCompletedSurveyReminderJobLogDetails(SurveyDto survey)
        {
            var jobLog = _unitOfWork.JobLogExpiringSoonSurveyNotCompletedReminderEmailRespository.GetUsingNoTracking(r => r.ProfileRosterId == survey.RosterItemId).FirstOrDefault();
            if (jobLog != null)
            {
                return ObjectMapper.GetJobLogExpiringSoonSurveyNotCompletedReminderEmailDto(jobLog);
            }
            return null;
        }


        internal List<HangfireStateDto> GetExpiringSoonNotCompletedSurveyReminderJobDetails(SurveyDto survey)
        {
            List<HangfireStateDto> hangfireJobStates = new List<HangfireStateDto>();
            var jobLog = _unitOfWork.JobLogExpiringSoonSurveyNotCompletedReminderEmailRespository.GetUsingNoTracking(r => r.ProfileRosterId == survey.RosterItemId).FirstOrDefault();
            if (jobLog != null)
            {
                if (jobLog != null)
                {
                    if (jobLog.HangfireJobId.HasValue)
                    {
                        hangfireJobStates = _unitOfWork.GetHangfireJobStates(jobLog.HangfireJobId.Value);
                    }
                }
            }
            return hangfireJobStates;
        }


        internal ProfileRosterDto GetProfileRosterByRosterId(int rosterId)
        {
            var roster = _unitOfWork.ProfileRosterRespository.GetUsingNoTracking(p => p.Id == rosterId).SingleOrDefault();
            return ObjectMapper.GetProfileRosterDto(roster);
        }

        internal ProfileDto GetProfileById(int profileId)
        {
            var profile = _unitOfWork.ProfileRespository.GetByID(profileId);
            return ObjectMapper.GetProfileDto(profile);
        }

        internal IEnumerable<PageStatDto> GetPageStatsByProfileId(int profileId)
        {
            var e = _unitOfWork
                     .PageStatsRespository
                     .GetUsingNoTracking(x => (x.ProfileId == profileId) && (x.SurveyId == null));

            var dtos = e
                       .Select(b => ObjectMapper.GetPageStatDto(b))
                       .ToList().AsQueryable<PageStatDto>();
            if (dtos != null)
                return dtos;

            return null;
        }




        internal IEnumerable<RecurrentSurveyDetailsViewModel> GetRecurrentSurveyDetails(int profileId)
        {
            var shifts = _unitOfWork
                     .ProfileRosterRespository
                     .GetUsingNoTracking(x => (x.ProfileId == profileId)).ToList();

            var surveys = _unitOfWork
                   .SurveyRespository
                   .GetUsingNoTracking(x => (x.ProfileId == profileId)).ToList();

            var shiftDtos = shifts.Select(b => ObjectMapper.GetRecurrentSurveyDetails(b, surveys))
                      .ToList().AsQueryable<RecurrentSurveyDetailsViewModel>();

            //var hangfireJobs = _unitOfWork.GetHanfireJobStates().ToList();

            if (shiftDtos != null)
                return shiftDtos;

            return null;
        }



        public async Task<IEnumerable<BirthYearDto>> GetAllBirthYearsAsync()
        {
            await Task.Run(() =>
            {
                var e = _unitOfWork
                        .BirthYearRespository
                        .GetUsingNoTracking(
                );

                var dtos = e
                           .Select(b => ObjectMapper.GetBirthYearsDto(b))
                           .ToList().AsQueryable<BirthYearDto>();
                if (dtos != null)
                    return dtos;

                return null;

            });

            return null;
        }



        public BirthYearDto GetBirthYearById(int id)
        {
            var e = _unitOfWork
                        .BirthYearRespository
                        .GetByID(id);

            var dto = ObjectMapper.GetBirthYearsDto(e);

            if (dto != null)
                return dto;

            return null;
        }

        public async Task<BirthYearDto> GetBirthYearByIdAsync(int id)
        {
            await Task.Run(() =>
            {
                var e = _unitOfWork
                        .BirthYearRespository
                        .GetByID(id);
                var dto = ObjectMapper.GetBirthYearsDto(e);
                if (dto != null)
                    return dto;

                return null;
            });

            return null;
        }


        public void CreateProfileRoster(RosterItemViewModelRevised rosterVM)
        {
            if (string.IsNullOrEmpty(rosterVM.Title))
            {
                rosterVM.Title = "On Shift";
            }

            int offsetFromUTC = 0;
            var profile = _unitOfWork.ProfileRespository.GetByID(rosterVM.ProfileId);
            offsetFromUTC = profile.OffSetFromUTC * -1;

            if (rosterVM.OwnerID == null)
            {
                try
                {
                    rosterVM.OwnerID = profile.Id;
                }
                catch (Exception)
                {

                    throw;
                }
            }


            var entity = new ProfileRoster
            {
                Id = rosterVM.TaskID,
                Name = rosterVM.Title,

                //UTC
                Start = rosterVM.Start,
                End = rosterVM.End,

                //2017-01-28 UTC
                StartUtc = rosterVM.Start.AddHours(offsetFromUTC),
                EndUtc = rosterVM.End.AddHours(offsetFromUTC),

                Description = rosterVM.Start.ToString("dd MMM yyyy hh:mm tt") + " -  " + rosterVM.End.ToString("dd MMM yyyy hh:mm tt"),
                RecurrenceRule = rosterVM.RecurrenceRule,
                RecurrenceException = rosterVM.RecurrenceException,
                RecurrenceID = rosterVM.RecurrenceID,
                IsAllDay = rosterVM.IsAllDay,
                ProfileId = rosterVM.OwnerID.Value
            };

            if (entity != null)
            {
                _unitOfWork.ProfileRosterRespository.Insert(entity);
                _unitOfWork.SaveChanges();
            }
        }



        public async Task CreateProfileRosterAsync(ProfileRosterDto dto)
        {
            await Task.Run(() =>
            {
                ProfileRoster e = ObjectMapper.GetProfileRosterEntity(dto);
                if (e != null)
                {
                    _unitOfWork.ProfileRosterRespository.Insert(e);
                    _unitOfWork.SaveChanges();
                }
            });
        }



        public void CreateBirthYear(BirthYearDto dto)
        {

            BirthYear e = ObjectMapper.GetBirthYearsEntity(dto);

            if (e != null)
            {
                _unitOfWork.BirthYearRespository.Insert(e);
                _unitOfWork.SaveChanges();
            }
        }



        public async Task CreateBirthYearAsync(BirthYearDto dto)
        {
            await Task.Run(() =>
            {
                BirthYear e = ObjectMapper.GetBirthYearsEntity(dto);
                if (e != null)
                {
                    _unitOfWork.BirthYearRespository.Insert(e);
                    _unitOfWork.SaveChanges();
                }
            });
        }


        public void EditBirthYear(BirthYearDto dto)
        {

            BirthYear e = ObjectMapper.GetBirthYearsEntity(dto);

            if (e != null)
            {
                _unitOfWork.BirthYearRespository.Update(e);
                _unitOfWork.SaveChanges();
            }
        }


        public async Task EditBirthYearAsync(BirthYearDto dto)
        {
            await Task.Run(() =>
            {
                BirthYear e = ObjectMapper.GetBirthYearsEntity(dto);
                if (e != null)
                {
                    _unitOfWork.BirthYearRespository.Update(e);
                    _unitOfWork.SaveChanges();
                }
            });
        }

        public void DeleteBirthYear(int? id)
        {
            if (id.HasValue)
            {
                _unitOfWork.BirthYearRespository.Delete(id.Value);
                _unitOfWork.SaveChanges();
            }
        }

        public async Task DeleteBirthYearAsync(int? id)
        {
            await Task.Run(() =>
            {
                if (id.HasValue)
                {
                    _unitOfWork.BirthYearRespository.Delete(id.Value);
                    _unitOfWork.SaveChanges();
                }
            });
        }


        public async Task<ModelStateDto> UploadBirthYears(HttpPostedFileBase file, string path)
        {
            ModelStateDto model = new ModelStateDto();
            file.SaveAs(path);

            FileInfo eFile = new FileInfo(path);

            using (var excelPackage = new OfficeOpenXml.ExcelPackage(eFile))
            {
                if (!eFile.Name.EndsWith("xlsx"))
                {
                    model.Errors.Add("Incompartible Excel Document. Please use MSExcel 2007 and Above!");
                }
                else
                {
                    var worksheet = excelPackage.Workbook.Worksheets[1];
                    if (worksheet == null)
                    {
                        model.Errors.Add("Wrong Excel Format!");
                    }
                    // return ImportResults.WrongFormat;
                    else
                    {
                        var lastRow = worksheet.Dimension.End.Row;
                        while (lastRow >= 1)
                        {
                            var range = worksheet.Cells[lastRow, 1, lastRow, 3];
                            if (range.Any(c => c.Value != null))
                            {
                                break;
                            }
                            lastRow--;
                        }
                        for (var row = 2; row <= lastRow; row++)
                        {
                            var s = new BirthYear
                            {
                                Name = worksheet.Cells[row, 1].Value.ToString(),
                                Sequence = int.Parse(worksheet.Cells[row, 2].Value.ToString())
                            };

                            _unitOfWork.BirthYearRespository.Insert(s);

                            try
                            {
                                _unitOfWork.SaveChanges();
                            }

                            catch (Exception ex)
                            {
                                model.Errors.Add(ex.Message);
                            }
                        }


                    }

                }

            }

            return model;
        }

        #endregion BirthYears


        #region Medical Universities
        public IEnumerable<MedicalUniversityDto> GetAllMedicalUniverisities()
        {
            var e = _unitOfWork
                        .MedicalUniversitiesRespository
                        .GetUsingNoTracking(
                );

            var dtos = e
                       .Select(b => ObjectMapper.GetMedicalUniversityDto(b))
                       .ToList().AsQueryable<MedicalUniversityDto>();
            if (dtos != null)
                return dtos;

            return null;
        }

        public async Task<IEnumerable<MedicalUniversityDto>> GetAllGetAllMedicalUniverisitiesAsync()
        {
            await Task.Run(() =>
            {
                var e = _unitOfWork
                        .MedicalUniversitiesRespository
                        .GetUsingNoTracking(
                );

                var dtos = e
                           .Select(b => ObjectMapper.GetMedicalUniversityDto(b))
                           .ToList().AsQueryable<MedicalUniversityDto>();
                if (dtos != null)
                    return dtos;

                return null;

            });

            return null;
        }
        #endregion



        #region Ethinicities
        public IEnumerable<EthinicityDto> GetAllEthinicities()
        {
            var e = _unitOfWork
                        .EthinicityRespository
                        .GetUsingNoTracking(
                );

            var dtos = e
                       .Select(b => ObjectMapper.GetEthinicityDto(b))
                       .ToList().AsQueryable<EthinicityDto>();
            if (dtos != null)
                return dtos;

            return null;
        }

        public async Task<IEnumerable<EthinicityDto>> GetAllEthinicitiesAsync()
        {
            await Task.Run(() =>
            {
                var e = _unitOfWork
                        .EthinicityRespository
                        .GetUsingNoTracking(
                );

                var dtos = e
                           .Select(b => ObjectMapper.GetEthinicityDto(b))
                           .ToList().AsQueryable<EthinicityDto>();
                if (dtos != null)
                    return dtos;

                return null;

            });

            return null;
        }



        public EthinicityDto GetEthinicityById(int id)
        {
            var e = _unitOfWork
                        .EthinicityRespository
                        .GetByID(id);

            var dto = ObjectMapper.GetEthinicityDto(e);

            if (dto != null)
                return dto;

            return null;
        }

        public async Task<EthinicityDto> GetEthinicityByIdAsync(int id)
        {
            await Task.Run(() =>
            {
                var e = _unitOfWork
                        .EthinicityRespository
                        .GetByID(id);

                var dto = ObjectMapper.GetEthinicityDto(e);

                if (dto != null)
                    return dto;

                return null;
            });

            return null;
        }

        public void CreateEthinicity(EthinicityDto dto)
        {

            var e = ObjectMapper.GetEthinicityEntity(dto);

            if (e != null)
            {
                _unitOfWork.EthinicityRespository.Insert(e);
                _unitOfWork.SaveChanges();
            }
        }



        public async Task CreateEthinicitiesAsync(EthinicityDto dto)
        {
            await Task.Run(() =>
            {
                var e = ObjectMapper.GetEthinicityEntity(dto);
                if (e != null)
                {
                    _unitOfWork.EthinicityRespository.Insert(e);
                    _unitOfWork.SaveChanges();
                }
            });
        }


        public void EditEthinicity(EthinicityDto dto)
        {

            var e = ObjectMapper.GetEthinicityEntity(dto);

            if (e != null)
            {
                _unitOfWork.EthinicityRespository.Update(e);
                _unitOfWork.SaveChanges();
            }
        }


        public async Task EditEthinicityAsync(EthinicityDto dto)
        {
            await Task.Run(() =>
            {
                var e = ObjectMapper.GetEthinicityEntity(dto);
                if (e != null)
                {
                    _unitOfWork.EthinicityRespository.Update(e);
                    _unitOfWork.SaveChanges();
                }
            });
        }

        public void DeleteEthinicity(int? id)
        {
            if (id.HasValue)
            {
                _unitOfWork.EthinicityRespository.Delete(id.Value);
                _unitOfWork.SaveChanges();
            }
        }

        public async Task DeleteEthinicitiyAsync(int? id)
        {
            await Task.Run(() =>
            {
                if (id.HasValue)
                {
                    _unitOfWork.EthinicityRespository.Delete(id.Value);
                    _unitOfWork.SaveChanges();
                }
            });
        }


        public async Task<ModelStateDto> UploadEthinicities(HttpPostedFileBase file, string path)
        {
            ModelStateDto model = new ModelStateDto();
            file.SaveAs(path);

            FileInfo eFile = new FileInfo(path);

            using (var excelPackage = new OfficeOpenXml.ExcelPackage(eFile))
            {
                if (!eFile.Name.EndsWith("xlsx"))
                {
                    model.Errors.Add("Incompartible Excel Document. Please use MSExcel 2007 and Above!");
                }
                else
                {
                    var worksheet = excelPackage.Workbook.Worksheets[1];
                    if (worksheet == null)
                    {
                        model.Errors.Add("Wrong Excel Format!");
                    }
                    // return ImportResults.WrongFormat;
                    else
                    {
                        var lastRow = worksheet.Dimension.End.Row;
                        while (lastRow >= 1)
                        {
                            var range = worksheet.Cells[lastRow, 1, lastRow, 3];
                            if (range.Any(c => c.Value != null))
                            {
                                break;
                            }
                            lastRow--;
                        }
                        for (var row = 2; row <= lastRow; row++)
                        {
                            var s = new Ethinicity
                            {
                                Name = worksheet.Cells[row, 1].Value.ToString(),
                                Sequence = int.Parse(worksheet.Cells[row, 2].Value.ToString())
                            };

                            _unitOfWork.EthinicityRespository.Insert(s);

                            try
                            {
                                _unitOfWork.SaveChanges();
                            }

                            catch (Exception ex)
                            {
                                model.Errors.Add(ex.Message);
                            }
                        }


                    }

                }

            }

            return model;
        }
        #endregion


        #region Specialities
        public IEnumerable<SpecialityDto> GetAllSpecialities()
        {
            var e = _unitOfWork
                        .SpecialitiesRespository
                        .GetUsingNoTracking(
                );

            var dtos = e
                       .Select(b => ObjectMapper.GetSpecialityDto(b))
                       .ToList().AsQueryable<SpecialityDto>();
            if (dtos != null)
                return dtos;

            return null;
        }

        public async Task<IEnumerable<SpecialityDto>> GetAllSpecialitiesAsync()
        {
            await Task.Run(() =>
            {
                var e = _unitOfWork
                        .SpecialitiesRespository
                        .GetUsingNoTracking(
                );

                var dtos = e
                           .Select(b => ObjectMapper.GetSpecialityDto(b))
                           .ToList().AsQueryable<SpecialityDto>();
                if (dtos != null)
                    return dtos;

                return null;

            });

            return null;
        }



        public SpecialityDto GetSpecialityById(int id)
        {
            var e = _unitOfWork
                        .SpecialitiesRespository
                        .GetByID(id);

            var dto = ObjectMapper.GetSpecialityDto(e);

            if (dto != null)
                return dto;

            return null;
        }

        public async Task<SpecialityDto> GetSpecialityByIdAsync(int id)
        {
            await Task.Run(() =>
            {
                var e = _unitOfWork
                        .SpecialitiesRespository
                        .GetByID(id);

                var dto = ObjectMapper.GetSpecialityDto(e);

                if (dto != null)
                    return dto;

                return null;
            });

            return null;
        }

        public void CreateSpeciality(SpecialityDto dto)
        {

            var e = ObjectMapper.GetSpecialityEntity(dto);

            if (e != null)
            {
                _unitOfWork.SpecialitiesRespository.Insert(e);
                _unitOfWork.SaveChanges();
            }
        }



        public async Task CreateSpecialityAsync(SpecialityDto dto)
        {
            await Task.Run(() =>
            {
                var e = ObjectMapper.GetSpecialityEntity(dto);
                if (e != null)
                {
                    _unitOfWork.SpecialitiesRespository.Insert(e);
                    _unitOfWork.SaveChanges();
                }
            });
        }


        public void EditSpeciality(SpecialityDto dto)
        {

            var e = ObjectMapper.GetSpecialityEntity(dto);

            if (e != null)
            {
                _unitOfWork.SpecialitiesRespository.Update(e);
                _unitOfWork.SaveChanges();
            }
        }


        public async Task EditSpecialityAsync(SpecialityDto dto)
        {
            await Task.Run(() =>
            {
                var e = ObjectMapper.GetSpecialityEntity(dto);
                if (e != null)
                {
                    _unitOfWork.SpecialitiesRespository.Update(e);
                    _unitOfWork.SaveChanges();
                }
            });
        }

        public void DeleteSpeciality(int? id)
        {
            if (id.HasValue)
            {
                _unitOfWork.EthinicityRespository.Delete(id.Value);
                _unitOfWork.SaveChanges();
            }
        }

        public async Task DeleteSpecialityAsync(int? id)
        {
            await Task.Run(() =>
            {
                if (id.HasValue)
                {
                    _unitOfWork.SpecialitiesRespository.Delete(id.Value);
                    _unitOfWork.SaveChanges();
                }
            });
        }


        public async Task<ModelStateDto> UploadSpecialities(HttpPostedFileBase file, string path)
        {
            ModelStateDto model = new ModelStateDto();
            file.SaveAs(path);

            FileInfo eFile = new FileInfo(path);

            using (var excelPackage = new OfficeOpenXml.ExcelPackage(eFile))
            {
                if (!eFile.Name.EndsWith("xlsx"))
                {
                    model.Errors.Add("Incompartible Excel Document. Please use MSExcel 2007 and Above!");
                }
                else
                {
                    var worksheet = excelPackage.Workbook.Worksheets[1];
                    if (worksheet == null)
                    {
                        model.Errors.Add("Wrong Excel Format!");
                    }
                    // return ImportResults.WrongFormat;
                    else
                    {
                        var lastRow = worksheet.Dimension.End.Row;
                        while (lastRow >= 1)
                        {
                            var range = worksheet.Cells[lastRow, 1, lastRow, 3];
                            if (range.Any(c => c.Value != null))
                            {
                                break;
                            }
                            lastRow--;
                        }
                        for (var row = 2; row <= lastRow; row++)
                        {
                            var s = new Specialty
                            {
                                Name = worksheet.Cells[row, 1].Value.ToString(),
                                Sequence = int.Parse(worksheet.Cells[row, 2].Value.ToString())
                            };

                            _unitOfWork.SpecialitiesRespository.Insert(s);

                            try
                            {
                                _unitOfWork.SaveChanges();
                            }

                            catch (Exception ex)
                            {
                                model.Errors.Add(ex.Message);
                            }
                        }


                    }

                }

            }

            return model;
        }
        #endregion

        #region TaskCategory



        #endregion

        #region TaskItems
        public IEnumerable<TaskItemDto> GetAllTaskItems()
        {
            var e = _unitOfWork
                        .TaskItemRespository
                        .GetUsingNoTracking(
                );

            var dtos = e
                       .Select(b => ObjectMapper.GetTaskItemDto(b))
                       .ToList().AsQueryable<TaskItemDto>();
            if (dtos != null)
                return dtos;

            return null;
        }

        public async Task<IEnumerable<TaskItemDto>> GetAllTaskItemsAsync()
        {
            await Task.Run(() =>
            {
                var e = _unitOfWork
                        .TaskItemRespository
                        .GetUsingNoTracking(
                );

                var dtos = e
                           .Select(b => ObjectMapper.GetTaskItemDto(b))
                           .ToList().AsQueryable<TaskItemDto>();
                if (dtos != null)
                    return dtos;

                return null;

            });

            return null;
        }



        public TaskItemDto GetTaskItemById(int id)
        {
            var e = _unitOfWork
                        .TaskItemRespository
                        .GetByID(id);

            var dto = ObjectMapper.GetTaskItemDto(e);

            if (dto != null)
                return dto;

            return null;
        }

        public async Task<TaskItemDto> GetTaskItemByIdAsync(int id)
        {
            await Task.Run(() =>
            {
                var e = _unitOfWork
                        .TaskItemRespository
                        .GetByID(id);

                var dto = ObjectMapper.GetTaskItemDto(e);

                if (dto != null)
                    return dto;

                return null;
            });

            return null;
        }

        public void CreateTaskItem(TaskItemDto dto)
        {

            var e = ObjectMapper.GetTaskItemEntity(dto);

            if (e != null)
            {
                _unitOfWork.TaskItemRespository.Insert(e);
                _unitOfWork.SaveChanges();
            }
        }



        public async Task CreateTaskItemAsync(TaskItemDto dto)
        {
            await Task.Run(() =>
            {
                var e = ObjectMapper.GetTaskItemEntity(dto);
                if (e != null)
                {
                    _unitOfWork.TaskItemRespository.Insert(e);
                    _unitOfWork.SaveChanges();
                }
            });
        }


        public void EditTaskItem(TaskItemDto dto)
        {

            var e = ObjectMapper.GetTaskItemEntity(dto);

            if (e != null)
            {
                _unitOfWork.TaskItemRespository.Update(e);
                _unitOfWork.SaveChanges();
            }
        }


        public async Task EditTaskItemAsync(TaskItemDto dto)
        {
            await Task.Run(() =>
            {
                var e = ObjectMapper.GetTaskItemEntity(dto);
                if (e != null)
                {
                    _unitOfWork.TaskItemRespository.Update(e);
                    _unitOfWork.SaveChanges();
                }
            });
        }

      

        public async void DeleteSurvey(int? id)
        {
            if (id.HasValue)
            {
                Survey survey = _unitOfWork.SurveyRespository.GetByID(id.Value);



                if (survey != null)
                {
                    // Delete Job Logs and Scheduled Jobs
                    await _jobService.RemoveShiftReminderJobs(survey.RosterItemId.Value);
                    await _jobService.RemoveCreateSurveyJobs(survey.RosterItemId.Value);
                    await _jobService.RemoveStartSurveyReminders(survey.RosterItemId.Value);
                    await _jobService.RemoveUpdateSurveyJob(survey.RosterItemId.Value);
                    await _jobService.RemoveCompleteSurveyReminders(survey.RosterItemId.Value);
                    await _jobService.RemoveExpiringSoonNotStartedReminder(survey.RosterItemId.Value);
                    await _jobService.RemoveExpiringSoonNotCompletedReminders(survey.RosterItemId.Value);
                }

                //Delete Survey
                _unitOfWork.SurveyRespository.Delete(id.Value);
                _unitOfWork.SaveChanges();



            }




        }


        public void DeleteTaskItem(int? id)
        {
            if (id.HasValue)
            {
                _unitOfWork.TaskItemRespository.Delete(id.Value);
                _unitOfWork.SaveChanges();
            }
        }

        public void DeleteProfileRoster(int? id)
        {
            if (id.HasValue)
            {
                _unitOfWork.ProfileRosterRespository.Delete(id.Value);
                _unitOfWork.SaveChanges();
            }
        }


        public async Task DeleteTaskItemAsync(int? id)
        {
            await Task.Run(() =>
            {
                if (id.HasValue)
                {
                    _unitOfWork.TaskItemRespository.Delete(id.Value);
                    _unitOfWork.SaveChanges();
                }
            });
        }


        public async Task<ModelStateDto> UploadTaskItems(HttpPostedFileBase file, string path)
        {
            string taskLongName = string.Empty;
            ModelStateDto model = new ModelStateDto();
            file.SaveAs(path);

            FileInfo eFile = new FileInfo(path);

            using (var excelPackage = new OfficeOpenXml.ExcelPackage(eFile))
            {
                if (!eFile.Name.EndsWith("xlsx"))
                {
                    model.Errors.Add("Incompartible Excel Document. Please use MSExcel 2007 and Above!");
                }
                else
                {
                    var worksheet = excelPackage.Workbook.Worksheets[1];
                    if (worksheet == null)
                    {
                        model.Errors.Add("Wrong Excel Format!");
                    }
                    // return ImportResults.WrongFormat;
                    else
                    {
                        var lastRow = worksheet.Dimension.End.Row;
                        while (lastRow >= 1)
                        {
                            var range = worksheet.Cells[lastRow, 1, lastRow, 4];
                            if (range.Any(c => c.Value != null))
                            {
                                break;
                            }
                            lastRow--;
                        }
                        for (var row = 2; row <= lastRow; row++)
                        {
                            if (worksheet.Cells[row, 2].Value == null)
                            { worksheet.Cells[row, 2].Value = string.Empty; taskLongName = worksheet.Cells[row, 2].Value.ToString(); }
                            else { taskLongName = worksheet.Cells[row, 2].Value.ToString(); }
                            
                            var s = new TaskItem
                            {
                                ShortName = worksheet.Cells[row, 1].Value.ToString(),
                                LongName = taskLongName,//worksheet.Cells[row, 2].Value.ToString(),
                                Type = worksheet.Cells[row, 3].Value.ToString(),
                                Sequence = int.Parse(worksheet.Cells[row, 4].Value.ToString()),
                                //WardRoundIndicator = (bool) worksheet.Cells[row, 1].Value,
                                //OtherTaskIndicator = (bool) worksheet.Cells[row, 1].Value,
                                //IsWardRoundTask = (bool) worksheet.Cells[row, 1].Value
                            };

                            _unitOfWork.TaskItemRespository.Insert(s);
                            try
                            {
                                _unitOfWork.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                model.Errors.Add(ex.Message);
                            }
                        }
                    }
                }
            }
            return model;
        }
        #endregion







        #endregion

        #region for ProfileAdmin

        public IEnumerable<ProfileDto> GetAllProfiles()
        {
            var profiles = _unitOfWork.ProfileRespository.GetUsingNoTracking(
            null, null, "User"
            );


            var profileDtos = profiles
                                .Select(b => ObjectMapper.GetProfileDto(b))
                                .OrderBy(b => b.Id)
                                .ToList().AsQueryable<ProfileDto>();
            if (profileDtos != null)
                return profileDtos;

            return null;
        }

        public IEnumerable<ProfileDto> GetRecurrentSurveys()
        {
            var profiles = _unitOfWork.ProfileRespository
                .GetUsingNoTracking(
            null, null, "User"
            );


            var profileDtos = profiles
                                .Select(b => ObjectMapper.GetProfileDto(b))
                                .OrderBy(b=>b.Id)
                                .ToList().AsQueryable<ProfileDto>();
            if (profileDtos != null)
                return profileDtos;

            return null;
        }


        #endregion

    }
}