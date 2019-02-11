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

namespace SANSurveyWebAPI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MasterDataController : BaseController
    {
        private AdminService adminService;
        private ApplicationDbContext db = new ApplicationDbContext();
        public MasterDataController()
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
            //List<MasterData> objMaster = new List<MasterData>();
            //objMaster = db.MasterDataS.Select(s => s).ToList();
            GetAllMasterData();
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> Edit()
        {
            GetAllMasterData();
            return View();
        }
        public async Task<ActionResult> GetAllMasterData()
        {
            var mdf = db.MasterDataS.Select(s => s).ToList();
            MasterData masterDataFile = new MasterData();
            masterDataFile.Id = mdf[0].Id;
            masterDataFile.RecurrentSurveyTimeSlot = mdf[0].RecurrentSurveyTimeSlot;
            masterDataFile.RecurrentSurveyTaskSelectionLimit = mdf[0].RecurrentSurveyTaskSelectionLimit;
            masterDataFile.NoOfSurveyPerParticipant = mdf[0].NoOfSurveyPerParticipant;
            
            return View(masterDataFile);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,RecurrentSurveyTimeSlot,RecurrentSurveyTaskSelectionLimit,NoOfSurveyPerParticipant")] MasterData masterDataFile)
        {
            if (ModelState.IsValid)
            {
                var mdf = db.MasterDataS.Where(x => x.Id == masterDataFile.Id).SingleOrDefault();
                mdf.RecurrentSurveyTimeSlot = masterDataFile.RecurrentSurveyTimeSlot;
                mdf.RecurrentSurveyTaskSelectionLimit = masterDataFile.RecurrentSurveyTaskSelectionLimit;
                mdf.NoOfSurveyPerParticipant = masterDataFile.NoOfSurveyPerParticipant;          

                db.Entry(mdf).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }           
            return View(masterDataFile);
        }
    }
}