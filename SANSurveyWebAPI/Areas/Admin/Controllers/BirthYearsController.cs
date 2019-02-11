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
    public class BirthYearsController : BaseController
    {
        private AdminService adminService;

        public BirthYearsController()
        {
            this.adminService = new AdminService();

        }

        protected override void Dispose(bool disposing)
        {
            adminService.Dispose();
            base.Dispose(disposing);
        }

        public async Task<ActionResult> Index()
        {
            return View(adminService.GetAllBirthYears());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BirthYearDto dto = adminService.GetBirthYearById(id.Value);
            //BirthYear birthYear = await db.BirthYears.FindAsync(id);
            if (dto == null)
            {
                return HttpNotFound();
            }
            return View(dto);
        }

        public ActionResult Create()
        {
            //var dto = new BirthYear();
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BirthYearDto dto)
        {
            if (ModelState.IsValid)
            {
                await adminService.CreateBirthYearAsync(dto);
                return RedirectToAction("Index");
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


                var modelState = await adminService.UploadBirthYears(file, path);

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

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var dto = adminService.GetBirthYearById(id.Value);
            //BirthYear birthYear = await db.BirthYears.FindAsync(id);
            if (dto == null)
            {
                return HttpNotFound();
            }
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(BirthYearDto dto)
        {
            if (ModelState.IsValid)
            {
                await adminService.EditBirthYearAsync(dto);
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

            var dto = adminService.GetBirthYearById(id.Value);
            //BirthYear birthYear = await db.BirthYears.FindAsync(id);

            if (dto == null)
            {
                return HttpNotFound();
            }
            return View(dto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {

            adminService.DeleteBirthYearAsync(id);

            return RedirectToAction("Index");
        }

    }
}