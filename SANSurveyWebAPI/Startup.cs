using System.Collections.Generic;
using Microsoft.Owin;
using Owin;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using SANSurveyWebAPI.BLL;

[assembly: OwinStartup(typeof(SANSurveyWebAPI.Startup))]

namespace SANSurveyWebAPI
{

    public class MyRestrictiveAuthorizationFilter : IAuthorizationFilter
    {
        public bool Authorize(IDictionary<string, object> owinEnvironment)
        {
            // In case you need an OWIN context, use the next line,
            // `OwinContext` class is the part of the `Microsoft.Owin` package.
            var context = new OwinContext(owinEnvironment);

            // Allow all authenticated users to see the Dashboard (potentially dangerous).

            //if (Request.IsAuthenticated && User.IsInRole("Admin"))
            //{
            //}
            //return context.Authentication.User.IsInRole("Admin");
            return context.Authentication.User.Identity.IsAuthenticated;
        }
    }

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            #region Office Dev
            ConfigureAuth(app);


            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");
            app.UseHangfireDashboard("/hangfire"
                , new DashboardOptions
                {
                    AuthorizationFilters = new[] { new MyRestrictiveAuthorizationFilter() }
                }

                );
            app.UseHangfireServer();

            //SchedulerService.StartKeepAliveJob();


            #endregion



            #region Local Dev
            ////GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");
            //var dashboardOptions = new DashboardOptions
            //{
            //    AuthorizationFilters = new[] { new MyRestrictiveAuthorizationFilter() }
            //};

            //GlobalConfiguration.Configuration
            //        // Use connection string name defined in `web.config` or `app.config`
            //        .UseSqlServerStorage("DefaultConnectionHangfire");
            //// Use custom connection string
            ////.UseSqlServerStorage(@"Server=.\sqlexpress; Database=Hangfire; Integrated Security=SSPI;");

            ////app.UseHangfireDashboard("/Scheduler/hangfire", new DashboardOptions
            ////{
            ////    AuthorizationFilters = new[] { new MyRestrictiveAuthorizationFilter() }
            ////});

            ////app.UseHangfireDashboard("/Scheduler/hangfire", options);

            //var sqlStorage = new SqlServerStorage("DefaultConnectionHangfire");
            //var oldOptions = new BackgroundJobServerOptions
            //{
            //    //ServerName = "OldQueueServer" // Pass this to differentiate this server from the next one
            //};

            //app.UseHangfireDashboard("/Scheduler/hangfire", new DashboardOptions(), sqlStorage);


            //app.UseHangfireServer();

            ////app.UseHangfireServer(sqlStorage, oldOptions);



            //ConfigureAuth(app);


            ////app.UseHangfireDashboard();
            ////app.UseHangfireServer();

            #endregion

        }
    }
}
