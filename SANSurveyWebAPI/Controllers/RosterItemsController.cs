using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.Models.Api;

namespace SANSurveyWebAPI.Controllers
{

    /*
        NOTE:

        This controller is deprecated
        Used in conjunction with telerik Scheduler, currently not used
          */

    #region deprecated
        
    //public class RosterItemsController : BaseController
    //{



    //    private ApplicationDbContext db = new ApplicationDbContext();

    //    public RosterItemsController() { }

    //    protected override void Dispose(bool disposing)
    //    {
    //        if (disposing)
    //        {
    //            db.Dispose();
    //        }
    //        base.Dispose(disposing);
    //    }

    //    public async Task<ActionResult> Index()
    //    {
    //        //return View(await db.RosterItems.ToListAsync());
    //        return View(await db.ProfileRosters.ToListAsync());
    //    }

    //    public async Task<ActionResult> Details(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //        }

    //        //RosterItem rosterItem = await db.RosterItems.FindAsync(id);
    //        ProfileRoster rosterItem = await db.ProfileRosters.FindAsync(id);

    //        if (rosterItem == null)
    //        {
    //            return HttpNotFound();
    //        }
    //        return View(rosterItem);
    //    }

    //    public ActionResult Create()
    //    {
    //        return View();
    //    }

    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public async Task<ActionResult> Create([Bind(Include = "Id,Name,IsAllDay,Start,End,RecurrenceRule,RecurrenceID,RecurrenceException,ProfileId,Description,StartTimezone,EndTimezone")] ProfileRoster rosterItem)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            //db.RosterItems.Add(rosterItem);
    //            db.ProfileRosters.Add(rosterItem);

    //            await db.SaveChangesAsync();
    //            return RedirectToAction("Index");
    //        }

    //        return View(rosterItem);
    //    }

    //    public async Task<ActionResult> Edit(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //        }

    //        //RosterItem rosterItem = await db.RosterItems.FindAsync(id);
    //        ProfileRoster rosterItem = await db.ProfileRosters.FindAsync(id);

    //        if (rosterItem == null)
    //        {
    //            return HttpNotFound();
    //        }
    //        return View(rosterItem);
    //    }

    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public async Task<ActionResult> Edit([Bind(Include = "Id,Name,IsAllDay,Start,End,RecurrenceRule,RecurrenceID,RecurrenceException,ProfileId,Description,StartTimezone,EndTimezone")] ProfileRoster rosterItem)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            db.Entry(rosterItem).State = EntityState.Modified;
    //            await db.SaveChangesAsync();
    //            return RedirectToAction("Index");
    //        }
    //        return View(rosterItem);
    //    }

    //    public async Task<ActionResult> Delete(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //        }

    //        //RosterItem rosterItem = await db.RosterItems.FindAsync(id);
    //        ProfileRoster rosterItem = await db.ProfileRosters.FindAsync(id);

    //        if (rosterItem == null)
    //        {
    //            return HttpNotFound();
    //        }
    //        return View(rosterItem);
    //    }

    //    [HttpPost, ActionName("Delete")]
    //    [ValidateAntiForgeryToken]
    //    public async Task<ActionResult> DeleteConfirmed(int id)
    //    {
    //        //RosterItem rosterItem = await db.RosterItems.FindAsync(id);
    //        ProfileRoster rosterItem = await db.ProfileRosters.FindAsync(id);

    //        //db.RosterItems.Remove(rosterItem);
    //        db.ProfileRosters.Remove(rosterItem);


    //        await db.SaveChangesAsync();
    //        return RedirectToAction("Index");
    //    }


    //}

    #endregion

}
