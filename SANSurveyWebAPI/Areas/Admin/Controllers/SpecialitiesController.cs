using SANSurveyWebAPI.BLL;
using SANSurveyWebAPI.Controllers;
using SANSurveyWebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SANSurveyWebAPI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SpecialitiesController : BaseController
    {
        private AdminService adminService;


        //private ApplicationDbContext db = new ApplicationDbContext();

        public SpecialitiesController()
        {
            this.adminService = new AdminService();
        }


        protected override void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    db.Dispose();
            //}

            adminService.Dispose();


            base.Dispose(disposing);
        }

        public async Task<ActionResult> Index()
        {
            return View(adminService.GetAllSpecialities());
            //return View(await db.Specialitys.ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //Speciality speciality = await db.Specialitys.FindAsync(id);
            //if (speciality == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(speciality);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var dto = adminService.GetSpecialityById(id.Value);
            //BirthYear birthYear = await db.BirthYears.FindAsync(id);
            if (dto == null)
            {
                return HttpNotFound();
            }
            return View(dto);




        }

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase file)
        {

            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                // extract only the filename
                var fileName = Path.GetFileName(file.FileName);
                // store the file inside ~/App_Data/uploads folder
                var path = Path.Combine(Server.MapPath("~/App_Data/Uploads"), fileName);


                var modelState = await adminService.UploadSpecialities(file, path);

                if (modelState.Errors != null)
                {
                    if (modelState.Errors.Count > 0)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }



        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SpecialityDto dto)
        {
            if (ModelState.IsValid)
            {
                await adminService.CreateSpecialityAsync(dto);
                return RedirectToAction("Index");
            }

            return View(dto);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var dto = adminService.GetSpecialityById(id.Value);

            if (dto == null)
            {
                return HttpNotFound();
            }
            return View(dto);

            //Speciality speciality = await db.Specialitys.FindAsync(id);
            //if (speciality == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(speciality);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SpecialityDto dto)
        {
            if (ModelState.IsValid)
            {
                await adminService.EditSpecialityAsync(dto);
                return RedirectToAction("Index");
            }
            return View(dto);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var dto = adminService.GetSpecialityById(id.Value);
            //BirthYear birthYear = await db.BirthYears.FindAsync(id);

            if (dto == null)
            {
                return HttpNotFound();
            }
            return View(dto);


            //Speciality speciality = await db.Specialitys.FindAsync(id);
            //if (speciality == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(speciality);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await adminService.DeleteSpecialityAsync(id);
            return RedirectToAction("Index");
        }

    }
}