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
    public class MedicalUniversitiesController : BaseController
    {
        //private ApplicationDbContext db = new ApplicationDbContext();
        private AdminService adminService;

        public MedicalUniversitiesController()
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
            return View(adminService.GetAllEthinicities());
            //return View(await db.Ethinicitys.ToListAsync());
        }


        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var dto = adminService.GetEthinicityById(id.Value);
            if (dto == null)
            {
                return HttpNotFound();
            }
            return View(dto);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EthinicityDto dto)
        {
            if (ModelState.IsValid)
            {
                await adminService.CreateEthinicitiesAsync(dto);
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

            var dto = adminService.GetEthinicityById(id.Value);
            if (dto == null)
            {
                return HttpNotFound();
            }
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EthinicityDto dto)
        {
            if (ModelState.IsValid)
            {
                await adminService.EditEthinicityAsync(dto);
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
            var dto = adminService.GetEthinicityById(id.Value);
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
            adminService.DeleteEthinicitiyAsync(id);
            return RedirectToAction("Index");
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
                var modelState = await adminService.UploadEthinicities(file, path);

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
    }
}