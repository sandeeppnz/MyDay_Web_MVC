using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using SANSurveyWebAPI.Models.Api;
using SANSurveyWebAPI.ViewModels.Web;
using System.Data.Entity;
using Kendo.Mvc.UI;
using System.Web.Mvc;
using SANSurveyWebAPI.Models;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using SANSurveyWebAPI.DTOs;

namespace SANSurveyWebAPI.BLL
{
    public class RosterItemService : ISchedulerEventService<RosterItemViewModel>
    {
        private static bool UpdateDatabase = true;
        private ApplicationDbContext db;

        public RosterItemService(ApplicationDbContext context)
        {
            db = context;
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public RosterItemService()
            : this(new ApplicationDbContext())
        {
        }

        public virtual IList<RosterItemViewModel> GetAllByProfileId(int profileId)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<RosterItemViewModel> result = null;

            if (!IsWebApiRequest)
            {
                result = HttpContext.Current.Session["SchedulerTasks"] as IList<RosterItemViewModel>;
            }

            if (result == null || UpdateDatabase)
            {
                try
                {
                    var profile = db.Profiles.SingleOrDefault(m => m.Id == profileId);

                    result = db.ProfileRosters.Where(i => i.ProfileId == profile.Id).ToList().Select(rosterItem => new RosterItemViewModel
                    {
                        TaskID = rosterItem.Id,
                        Title = rosterItem.Name,
                        Start = DateTime.SpecifyKind(rosterItem.Start, DateTimeKind.Utc),
                        End = DateTime.SpecifyKind(rosterItem.End, DateTimeKind.Utc),
                        StartTimezone = rosterItem.StartTimezone,
                        EndTimezone = rosterItem.EndTimezone,
                        Description = rosterItem.Description,
                        IsAllDay = rosterItem.IsAllDay,
                        RecurrenceRule = rosterItem.RecurrenceRule,
                        RecurrenceException = rosterItem.RecurrenceException,
                        RecurrenceID = rosterItem.RecurrenceID,
                        OwnerID = rosterItem.ProfileId
                    }).ToList();
                }
                catch (Exception)
                {
                    throw;
                }

                if (!IsWebApiRequest)
                {
                    HttpContext.Current.Session["SchedulerTasks"] = result;
                }
            }

            return result;
        }



        public virtual IList<RosterItemViewModel> GetAll()
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<RosterItemViewModel> result = null;

            if (!IsWebApiRequest)
            {
                result = HttpContext.Current.Session["SchedulerTasks"] as IList<RosterItemViewModel>;
            }

            if (result == null || UpdateDatabase)
            {
                string user = HttpContext.Current.User.Identity.GetUserName();

                try
                {
                    var profile = db.Profiles.SingleOrDefault(m => m.LoginEmail == user);

                    result = db.ProfileRosters.Where(i => i.ProfileId == profile.Id).ToList().Select(rosterItem => new RosterItemViewModel
                    {
                        TaskID = rosterItem.Id,
                        Title = rosterItem.Name,
                        Start = DateTime.SpecifyKind(rosterItem.Start, DateTimeKind.Utc),
                        End = DateTime.SpecifyKind(rosterItem.End, DateTimeKind.Utc),
                        StartTimezone = rosterItem.StartTimezone,
                        EndTimezone = rosterItem.EndTimezone,
                        Description = rosterItem.Description,
                        IsAllDay = rosterItem.IsAllDay,
                        RecurrenceRule = rosterItem.RecurrenceRule,
                        RecurrenceException = rosterItem.RecurrenceException,
                        RecurrenceID = rosterItem.RecurrenceID,
                        OwnerID = rosterItem.ProfileId
                    }).ToList();
                }
                catch (Exception)
                {
                    throw;
                }

                if (!IsWebApiRequest)
                {
                    HttpContext.Current.Session["SchedulerTasks"] = result;
                }
            }

            return result;
        }


        public virtual void Insert(RosterItemViewModel rosterVM, ModelStateDictionary modelState)
        {
            if (ValidateModel(rosterVM, modelState))
            {
                if (string.IsNullOrEmpty(rosterVM.Title))
                {
                    rosterVM.Title = "On Shift";
                }

                int offsetFromUTC = 0;

                string user = HttpContext.Current.User.Identity.GetUserName();
                var profile = db.Profiles.SingleOrDefault(m => m.LoginEmail == user);
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

                //Create a new Task entity and set its properties from the posted TaskViewModel.


                //var entity = new RosterItem
                //{
                //    Id = rosterVM.TaskID,
                //    Name = rosterVM.Title,

                //    //UTC
                //    Start = rosterVM.Start,
                //    End = rosterVM.End,


                //    //2017-01-28 UTC
                //    StartUtc = rosterVM.Start.AddHours(offsetFromUTC),
                //    EndUtc = rosterVM.End.AddHours(offsetFromUTC),

                //    //Description = rosterVM.Description,
                //    Description = rosterVM.Start.ToString("dd MMM yyyy hh:mm tt") + " -  " + rosterVM.End.ToString("dd MMM yyyy hh:mm tt"),
                //    RecurrenceRule = rosterVM.RecurrenceRule,
                //    RecurrenceException = rosterVM.RecurrenceException,
                //    RecurrenceID = rosterVM.RecurrenceID,
                //    IsAllDay = rosterVM.IsAllDay,
                //    ProfileId = rosterVM.OwnerID.Value
                //};

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


                //Add the entity.
                db.ProfileRosters.Add(entity);
                
                //Insert the entity in the database.
                db.SaveChanges();
                //Get the TaskID generated by the database.
                rosterVM.TaskID = entity.Id;
             
                //string offSet = string.Empty;
                // if (sign=='+')
                //{
                //    offSet = rosterVM.OffsetFromUTC.Substring(1, rosterVM.OffsetFromUTC.Length);
                //}
                //else
                //{
                //    offSet = rosterVM.OffsetFromUTC.Substring(1, rosterVM.OffsetFromUTC.Length);
                //}
            }
        }



        public virtual void Update(RosterItemViewModel rosterVM, ModelStateDictionary modelState)
        {
            if (ValidateModel(rosterVM, modelState))
            {

                //string user = HttpContext.Current.User.Identity.GetUserName();
                int offsetFromUTC = db.Profiles.SingleOrDefault(m => m.Id == rosterVM.OwnerID).OffSetFromUTC;
                offsetFromUTC = offsetFromUTC * -1;

                //if (Double.TryParse(rosterVM.OffsetFromUTC, out offSet))
                //{
                //Create a new Task entity and set its properties from the posted TaskViewModel.
                //var entity = new RosterItem
                //{
                //    Id = rosterVM.TaskID,
                //    Name = rosterVM.Title,
                //    Start = rosterVM.Start,
                //    End = rosterVM.End,
                //    //2017-01-28 UTC
                //    StartUtc = rosterVM.Start.AddHours(offsetFromUTC),
                //    EndUtc = rosterVM.End.AddHours(offsetFromUTC),
                //    //StartUtc = rosterVM.Start.ToUniversalTime(),
                //    //EndUtc = rosterVM.End.ToUniversalTime(),
                //    Description = rosterVM.Start.ToString("dd MMM yyyy hh:mm tt") + " -  " + rosterVM.End.ToString("dd MMM yyyy hh:mm tt"),
                //    //Description = rosterVM.Description,
                //    RecurrenceRule = rosterVM.RecurrenceRule,
                //    RecurrenceException = rosterVM.RecurrenceException,
                //    RecurrenceID = rosterVM.RecurrenceID,
                //    IsAllDay = rosterVM.IsAllDay,
                //    ProfileId = rosterVM.OwnerID.Value
                //};

                var entity = new ProfileRoster
                {
                    Id = rosterVM.TaskID,
                    Name = rosterVM.Title,
                    Start = rosterVM.Start,
                    End = rosterVM.End,
                    //2017-01-28 UTC
                    StartUtc = rosterVM.Start.AddHours(offsetFromUTC),
                    EndUtc = rosterVM.End.AddHours(offsetFromUTC),
                    //StartUtc = rosterVM.Start.ToUniversalTime(),
                    //EndUtc = rosterVM.End.ToUniversalTime(),
                    Description = rosterVM.Start.ToString("dd MMM yyyy hh:mm tt") + " -  " + rosterVM.End.ToString("dd MMM yyyy hh:mm tt"),
                    //Description = rosterVM.Description,
                    RecurrenceRule = rosterVM.RecurrenceRule,
                    RecurrenceException = rosterVM.RecurrenceException,
                    RecurrenceID = rosterVM.RecurrenceID,
                    IsAllDay = rosterVM.IsAllDay,
                    ProfileId = rosterVM.OwnerID.Value
                };


                //Attach the entity.
                //db.RosterItems.Attach(entity);
                db.ProfileRosters.Attach(entity);


                //Change its state to Modified so the Entity Framework can update the existing task instead of creating a new one.
                //sampleDB.Entry(entity).State = EntityState.Modified;
                //Or use ObjectStateManager if using a previous version of Entity Framework.

                var manager = ((IObjectContextAdapter) db).ObjectContext.ObjectStateManager;
                manager.ChangeObjectState(entity, EntityState.Modified);
                //Update the entity in the database
                db.SaveChanges();

                //}
            }
        }

        public virtual void Delete(RosterItemViewModel rosterVM, ModelStateDictionary modelState)
        {

            //var sign = rosterVM.OffsetFromUTC[0];
            //string offSet = string.Empty;
            //if (sign == '+')
            //{
            //    offSet = rosterVM.OffsetFromUTC.Substring(1, rosterVM.OffsetFromUTC.Length);
            //}
            //else
            //{
            //    offSet = rosterVM.OffsetFromUTC.Substring(1, rosterVM.OffsetFromUTC.Length);
            //}


            //Create a new Task entity and set its properties from the posted TaskViewModel.
            //var entity = new RosterItem
            //{
            //    Id = rosterVM.TaskID,
            //    Name = rosterVM.Title,
            //    Start = rosterVM.Start,
            //    End = rosterVM.End,
            //    Description = rosterVM.Description,
            //    RecurrenceRule = rosterVM.RecurrenceRule,
            //    RecurrenceException = rosterVM.RecurrenceException,
            //    RecurrenceID = rosterVM.RecurrenceID,
            //    IsAllDay = rosterVM.IsAllDay,
            //    ProfileId = rosterVM.OwnerID.Value
            //};

            var entity = new ProfileRoster
            {
                Id = rosterVM.TaskID,
                Name = rosterVM.Title,
                Start = rosterVM.Start,
                End = rosterVM.End,
                Description = rosterVM.Description,
                RecurrenceRule = rosterVM.RecurrenceRule,
                RecurrenceException = rosterVM.RecurrenceException,
                RecurrenceID = rosterVM.RecurrenceID,
                IsAllDay = rosterVM.IsAllDay,
                ProfileId = rosterVM.OwnerID.Value
            };

            //Attach the entity.
            //db.RosterItems.Attach(entity);
            db.ProfileRosters.Attach(entity);


            //Delete the entity.
            //sampleDB.Tasks.Remove(entity);
            //Or use DeleteObject if using a previous versoin of Entity Framework.
            //db.RosterItems.Remove(entity);
            db.ProfileRosters.Remove(entity);


            //Delete the entity in the database.
            db.SaveChanges();

        }

        private bool ValidateModel(RosterItemViewModel rosterVM, ModelStateDictionary modelState)
        {
            if (rosterVM.Start > rosterVM.End)
            {
                modelState.AddModelError("errors", "End date must be greater or equal to Start date.");
                return false;
            }

            return true;
        }

        public RosterItemViewModel One(Func<RosterItemViewModel, bool> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }




        public int UpdateRosterValidation(int profileId)
        {

            double numDays = 14;
            DateTime nextWeekDate = DateTime.UtcNow.AddDays(numDays);

            //int count = db.RosterItems
            //    .Where(p => p.ProfileId == profileId)
            //    .Where(d => d.EndUtc <= nextWeekDate)
            //    .Count();

            int count = db.ProfileRosters
                .Where(p => p.ProfileId == profileId)
                .Where(d => d.EndUtc <= nextWeekDate)
                .Count();

            return count;
        }



        public async Task<int> UpdateRosterValidationAsync(int profileId)
        {
            double numDays = 14;
            DateTime nextWeekDate = DateTime.UtcNow.AddDays(numDays);

            //int count = await db.RosterItems
            //    .Where(p => p.ProfileId == profileId)
            //    .Where(d => d.EndUtc <= nextWeekDate)
            //    .CountAsync();

            int count = await db.ProfileRosters
               .Where(p => p.ProfileId == profileId)
               .Where(d => d.EndUtc <= nextWeekDate)
               .CountAsync();

            return count;
        }

        //public RosterItemDto MapToDto(RosterItem p)
        //{
        //    return new RosterItemDto
        //    {
        //        Id = p.Id,
        //        Name = p.Name,
        //        IsAllDay = p.IsAllDay,
        //        Start = p.Start,
        //        End = p.End,
        //        StartUtc = p.StartUtc,
        //        EndUtc = p.EndUtc,
        //        RecurrenceRule = p.RecurrenceRule,
        //        RecurrenceID = p.RecurrenceID,
        //        RecurrenceException = p.RecurrenceException,
        //        ProfileId = p.ProfileId,
        //        Description = p.Description,
        //        StartTimezone = p.StartTimezone,
        //        EndTimezone = p.EndTimezone,
        //        ShiftReminderEmailJobId = p.ShiftReminderEmailJobId,
        //        ShiftReminderSmsJobId = p.ShiftReminderSmsJobId,
        //        CreateSurveyJobId = p.CreateSurveyJobId
        //    };
        //}

        public ProfileRosterDto MapToDto(ProfileRoster p)
        {
            return new ProfileRosterDto
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
                //ShiftReminderEmailJobId = p.ShiftReminderEmailJobId,
                //ShiftReminderSmsJobId = p.ShiftReminderSmsJobId,
                //CreateSurveyJobId = p.CreateSurveyJobId
            };
        }

    }
}