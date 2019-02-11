using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.Models.Api;

namespace SANSurveyWebAPI.Controllers
{

    #region deprecated

    //public class PageStatsController : BaseController
    //{
    //    private ApplicationDbContext db = new ApplicationDbContext();


    //    public PageStatsController()
    //    {
    //    }

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
    //        return View(await db.PageStats.ToListAsync());
    //    }

    //    public async Task<ActionResult> Details(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //        }
    //        PageStat pageStat = await db.PageStats.FindAsync(id);
    //        if (pageStat == null)
    //        {
    //            return HttpNotFound();
    //        }
    //        return View(pageStat);
    //    }

    //    public ActionResult Create()
    //    {
    //        return View();
    //    }

    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public async Task<ActionResult> Create([Bind(Include = "Id,PageName,PageType,PageAction,TaskStartDateTime,ProfileId,SurveyId,TaskId,WholePageIndicator,PageDateTime,Remark")] PageStat pageStat)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            db.PageStats.Add(pageStat);
    //            await db.SaveChangesAsync();
    //            return RedirectToAction("Index");
    //        }

    //        return View(pageStat);
    //    }

    //    public async Task<ActionResult> Edit(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //        }
    //        PageStat pageStat = await db.PageStats.FindAsync(id);
    //        if (pageStat == null)
    //        {
    //            return HttpNotFound();
    //        }
    //        return View(pageStat);
    //    }

    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public async Task<ActionResult> Edit([Bind(Include = "Id,PageName,PageType,PageAction,TaskStartDateTime,ProfileId,SurveyId,TaskId,WholePageIndicator,PageDateTime,Remark")] PageStat pageStat)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            db.Entry(pageStat).State = EntityState.Modified;
    //            await db.SaveChangesAsync();
    //            return RedirectToAction("Index");
    //        }
    //        return View(pageStat);
    //    }

    //    public async Task<ActionResult> Delete(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //        }
    //        PageStat pageStat = await db.PageStats.FindAsync(id);
    //        if (pageStat == null)
    //        {
    //            return HttpNotFound();
    //        }
    //        return View(pageStat);
    //    }

    //    [HttpPost, ActionName("Delete")]
    //    [ValidateAntiForgeryToken]
    //    public async Task<ActionResult> DeleteConfirmed(int id)
    //    {
    //        PageStat pageStat = await db.PageStats.FindAsync(id);
    //        db.PageStats.Remove(pageStat);
    //        await db.SaveChangesAsync();
    //        return RedirectToAction("Index");
    //    }

    
    //}


    #endregion
}
