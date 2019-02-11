using System;
using System.Linq;
using System.Web;
using System.Data;
using SANSurveyWebAPI.Models.Api;
using System.Data.Entity;
using System.Web.Mvc;
using SANSurveyWebAPI.Models;


using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace SANSurveyWebAPI.BLL
{
    public class PageStatService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public PageStatService(ApplicationDbContext context)
        {
            db = context;
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public PageStatService()
            : this(new ApplicationDbContext())
        {
        }

        public virtual async Task<int> Insert(int? surveyId, int? taskId, DateTime? taskStartDateTime, bool? wholepageIndicator, Constants.PageName pageName, Constants.PageAction pageAction, Constants.PageType pageType, int? profileId = null, string remark = null)
        {
            try
            {
                PageStat stat = new PageStat();

                stat.PageDateTimeUtc = DateTime.UtcNow;

                stat.TaskId = taskId;

                stat.SurveyId = surveyId;
                stat.WholePageIndicator = wholepageIndicator;
                stat.PageName = pageName.ToString();
                stat.PageType = pageType.ToString();
                stat.PageAction = pageAction.ToString();

                if (taskStartDateTime.HasValue)
                {
                    stat.TaskStartDateTime = taskStartDateTime.Value;
                }
                else
                {
                    stat.TaskStartDateTime = null;
                }


                stat.Remark = remark;

                if (!profileId.HasValue)
                {
                    string user = HttpContext.Current.User.Identity.GetUserName();


                    var profile = await db.Profiles
                        .Where(m => m.LoginEmail == user)
                        .Select(m => m.Id)
                        .SingleOrDefaultAsync();

                    stat.ProfileId = profile;

                }
                else
                {
                    stat.ProfileId = profileId.Value;
                }

                db.PageStats.Add(stat);
                await db.SaveChangesAsync();


                return stat.Id;
            }
            catch (Exception  ex)
            {
                throw;
            }

        }
    }
}