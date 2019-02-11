using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.Models.Api;
using System.IO;
using SANSurveyWebAPI.BLL;
using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.Controllers;

namespace SANSurveyWebAPI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TaskItemsController : BaseController
    {
        private AdminService adminService;
        private ApplicationDbContext db = new ApplicationDbContext();

        public TaskItemsController()
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
            return View(adminService.GetAllTaskItems());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var dto = adminService.GetTaskItemById(id.Value);
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
                //var path = Path.Combine(Server.MapPath("~/App_Data/Uploads"), fileName);                
                var path = Path.Combine(Server.MapPath("~/Views/Emails"), fileName);

                var modelState = await adminService.UploadTaskItems(file, path);

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
        public async Task<ActionResult> Create(TaskItemDto dto)
        {
            if (ModelState.IsValid)
            {
                await adminService.CreateTaskItemAsync(dto);

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


            var dto = adminService.GetTaskItemById(id.Value);
            if (dto == null)
            {
                return HttpNotFound();
            }
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(TaskItemDto dto)
        {
            if (ModelState.IsValid)
            {

                await adminService.EditTaskItemAsync(dto);
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

            var dto = adminService.GetTaskItemById(id.Value);

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
            TaskItem taskItem = await db.TaskItems.FindAsync(id);
            db.TaskItems.Remove(taskItem);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

 
    }
}
