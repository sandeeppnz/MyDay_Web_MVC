using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.Models.Api;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace SANSurveyWebAPI.Controllers.Api
{
    public class ProximiVisitorsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/ProximiVisitors
        public IQueryable<ProximiVisitor> GetProximiVisitors()
        {
            return db.ProximiVisitors;
        }

        // GET: api/ProximiVisitors/5
        [ResponseType(typeof(ProximiVisitor))]
        public IHttpActionResult GetProximiVisitor(int id)
        {
            ProximiVisitor proximiVisitor = db.ProximiVisitors.Find(id);
            if (proximiVisitor == null)
            {
                return NotFound();
            }

            return Ok(proximiVisitor);
        }

        // PUT: api/ProximiVisitors/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProximiVisitor(int id, ProximiVisitor proximiVisitor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != proximiVisitor.ID)
            {
                return BadRequest();
            }

            db.Entry(proximiVisitor).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProximiVisitorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ProximiVisitors
        [ResponseType(typeof(ProximiVisitor))]
        public IHttpActionResult PostProximiVisitor(ProximiVisitor proximiVisitor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ProximiVisitors.Add(proximiVisitor);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = proximiVisitor.ID }, proximiVisitor);
        }

        // DELETE: api/ProximiVisitors/5
        [ResponseType(typeof(ProximiVisitor))]
        public IHttpActionResult DeleteProximiVisitor(int id)
        {
            ProximiVisitor proximiVisitor = db.ProximiVisitors.Find(id);
            if (proximiVisitor == null)
            {
                return NotFound();
            }

            db.ProximiVisitors.Remove(proximiVisitor);
            db.SaveChanges();

            return Ok(proximiVisitor);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProximiVisitorExists(int id)
        {
            return db.ProximiVisitors.Count(e => e.ID == id) > 0;
        }

    }
}
