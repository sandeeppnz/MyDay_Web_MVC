using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;
using SANSurveyWebAPI.Models.Api;

namespace SANSurveyWebAPI.BLL
{
    public class ProfileAdminService : IDisposable
    {
        private ApplicationDbContext db;

        public ProfileAdminService(ApplicationDbContext context)
        {
            db = context;
        }

        public void Dispose()
        {
            db.Dispose();
        }


        public ProfileAdminService()
            : this(new ApplicationDbContext())
        {
        }

        public IList<ProfileAdminVM> GetAll()
        {
            IList<ProfileAdminVM> result = new List<ProfileAdminVM>();

            result = db.Profiles.Select(p => new ProfileAdminVM
            {
                Id = p.Id,
                Name = p.Name,
                MobileNumber = p.MobileNumber,
                EmailAddress = p.LoginEmail,
                RegistrationProgressNext = p.RegistrationProgressNext,
                Uid = p.Uid,
                UserId = p.UserId,
                User = new IdentityUser
                {
                    Id = p.User.Id,
                    UserName = p.User.UserName
                },
                RegisteredDateTime = p.RegisteredDateTimeUtc,
                CreatedDateTimeUtc = p.CreatedDateTimeUtc
            }).ToList();
            return result;
        }

        public IEnumerable<ProfileAdminVM> Read()
        {
            return GetAll();
        }


        public void Create(ProfileAdminVM v)
        {

            var e = new Profile();

            e.Name = v.Name;
            e.MobileNumber = v.MobileNumber;

            if (e.UserId == null)
            {
                e.UserId = null;
            }

            if (v.User != null)
            {
                e.UserId = v.User.Id;
            }

            e.CreatedDateTimeUtc = DateTime.UtcNow;
            e.MaxStep = 0;
            //e.Speciality = null;

            db.Profiles.Add(e);
            db.SaveChanges();

            v.Id = e.Id;

        }

        public void Update(ProfileAdminVM v)
        {
            var e = new Profile();

            e.Id = v.Id;
            e.Name = v.Name;
            e.MobileNumber = v.MobileNumber;
            e.LoginEmail = v.EmailAddress;

            if (e.UserId == null)
            {
                e.UserId = null;
            }

            if (v.User != null)
            {
                e.UserId = v.User.Id;
            }

            db.Profiles.Attach(e);
            db.Entry(e).State = EntityState.Modified;
            db.SaveChanges();
        }


        public void Delete(ProfileAdminVM v)
        {

            var e = new Profile();

            e.Id = v.Id;

            db.Profiles.Attach(e);

            db.Profiles.Remove(e);

            //var orderDetails = entities.Order_Details.Where(pd => pd.ProductID == e.ProductID);

            //foreach (var orderDetail in orderDetails)
            //{
            //    entities.Order_Details.Remove(orderDetail);
            //}

            db.SaveChanges();

        }




        public ProfileAdminVM One(Func<ProfileAdminVM, bool> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

     



    }
}