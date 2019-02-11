using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.Models.Api;
using System.IO;
using SANSurveyWebAPI.BLL;
using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.Controllers;
using System.Data.Entity;
using SANSurveyWebAPI.ViewModels;

namespace SANSurveyWebAPI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TaskCategoryController : BaseController
    {
        private AdminService adminService;
        private ApplicationDbContext db = new ApplicationDbContext();
        public TaskCategoryController()
        {
            this.adminService = new AdminService();
        }
        protected override void Dispose(bool disposing)
        {
            adminService.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Index()
        {
            TaskCategoryViewVM objTaskCatViewVM = new TaskCategoryViewVM();
            objTaskCatViewVM.TaskCategoryObj = new List<TaskCategory>();
            objTaskCatViewVM.TaskCategoryObj = db.TaskCategory.Select(s => s).ToList();
            objTaskCatViewVM.totalRowCount = objTaskCatViewVM.TaskCategoryObj.Count();

            return View(objTaskCatViewVM);
        }
        [HttpGet]
        public async Task<ActionResult> Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(TaskCategory tc)
        {
            //if (ModelState.IsValid)
            {
                try
                {
                    db.TaskCategory.Add(tc);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex) { }
            }
            return View(tc);
        }
        public async Task<ActionResult> Edit(int? id)
        {

            TaskCategory t = new TaskCategory();
            if (ModelState.IsValid)
            {
                try
                {
                    var tc = db.TaskCategory
                                .Where(s => s.Id == id)
                                .Select(s => s).First();

                    t.Category = tc.Category;
                    t.TaskType = tc.TaskType;
                    t.IsDeleted = tc.IsDeleted;
                    return RedirectToAction("Edit");
                }
                catch { }
            }           
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit(TaskCategory tc)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
                    
        //        }
        //        catch { }
        //    }
        //    return View(tc);
        //}
    }
}