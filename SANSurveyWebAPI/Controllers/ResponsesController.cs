using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.Models.Api;

namespace SANSurveyWebAPI.Controllers
{

    #region deprecated


    //public class ResponsesController : BaseController
    //{
    //    private ApplicationDbContext db = new ApplicationDbContext();



    //    public ResponsesController()
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
    //        return View(await db.Responses.ToListAsync());
    //    }

    //    public async Task<ActionResult> Details(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //        }
    //        Response response = await db.Responses.FindAsync(id);
    //        if (response == null)
    //        {
    //            return HttpNotFound();
    //        }
    //        return View(response);
    //    }

    //    public ActionResult Create()
    //    {
    //        return View();
    //    }

    //        [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public async Task<ActionResult> Create([Bind(Include = "Id,SurveyId,ProfileId,TaskId,PageStatId,StartResponse,EndResponse,ShiftStartDateTime,ShiftEndDateTime,TaskStartDateTime,TaskEndDateTime,Question,Answer,SurveyStartDateTime,SurveyEndDateTime")] Response response)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            db.Responses.Add(response);
    //            await db.SaveChangesAsync();
    //            return RedirectToAction("Index");
    //        }

    //        return View(response);
    //    }

    //    // GET: Responses/Edit/5
    //    public async Task<ActionResult> Edit(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //        }
    //        Response response = await db.Responses.FindAsync(id);
    //        if (response == null)
    //        {
    //            return HttpNotFound();
    //        }
    //        return View(response);
    //    }

    //     [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public async Task<ActionResult> Edit([Bind(Include = "Id,SurveyId,ProfileId,TaskId,PageStatId,StartResponse,EndResponse,ShiftStartDateTime,ShiftEndDateTime,TaskStartDateTime,TaskEndDateTime,Question,Answer,SurveyStartDateTime,SurveyEndDateTime")] Response response)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            db.Entry(response).State = EntityState.Modified;
    //            await db.SaveChangesAsync();
    //            return RedirectToAction("Index");
    //        }
    //        return View(response);
    //    }

    //    public async Task<ActionResult> Delete(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //        }
    //        Response response = await db.Responses.FindAsync(id);
    //        if (response == null)
    //        {
    //            return HttpNotFound();
    //        }
    //        return View(response);
    //    }

    //    [HttpPost, ActionName("Delete")]
    //    [ValidateAntiForgeryToken]
    //    public async Task<ActionResult> DeleteConfirmed(int id)
    //    {
    //        Response response = await db.Responses.FindAsync(id);
    //        db.Responses.Remove(response);
    //        await db.SaveChangesAsync();
    //        return RedirectToAction("Index");
    //    }

   
    //}

    #endregion

}
