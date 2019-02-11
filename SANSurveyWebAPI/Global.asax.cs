using SANSurveyWebAPI.BLL;
using System;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SANSurveyWebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
      
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Stop()
        {
        }



        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception error = Server.GetLastError().GetBaseException();

            if (error == null)
            {
                return;
            }

            if (error is ThreadAbortException)
                return;

            NotifyErrorByEmail(error);
            //Logger.Error(LoggerType.Global, ex, "Exception");
            //Response.Redirect("unexpectederror.htm");
        }

        //TODO: remvamp
        public static void NotifyErrorByEmail(Exception error)
        {
            //MembershipUser user = Membership.GetUser();
            //string userName = String.Empty;

            //if (user != null)
            //{
            //    userName = user.UserName;
            //}

            // Construct email content        

            StringBuilder builder = new StringBuilder();

            builder.AppendLine("<html><head><title></title>");
            builder.AppendLine("</head><body>");
            builder.AppendLine("<div style='padding: 2px;'>");
            builder.AppendLine("<table cellpadding='0' cellspacing='0' width='100%' border='1'>");

            builder.AppendLine("<tr>");
            builder.AppendLine(String.Format("<td>{0}</td>", "Message"));
            builder.AppendLine(String.Format("<td>{0}</td>", error.Message));
            builder.AppendLine("</tr>");

            //builder.AppendLine("<tr>");
            //builder.AppendLine(String.Format("<td>{0}</td>", "Login ID"));
            //builder.AppendLine(String.Format("<td>{0}</td>", userName));
            //builder.AppendLine("</tr>");

            builder.AppendLine("<tr>");
            builder.AppendLine(String.Format("<td>{0}</td>", "URL"));
            builder.AppendLine(String.Format("<td>{0}</td>", HttpContext.Current.Request.Url.AbsoluteUri));
            builder.AppendLine("</tr>");
            builder.AppendLine("<tr>");
            builder.AppendLine(String.Format("<td>{0}</td>", "Source"));
            builder.AppendLine(String.Format("<td>{0}</td>", error.Source));
            builder.AppendLine("</tr>");
            builder.AppendLine("<tr>");
            builder.AppendLine(String.Format("<td>{0}</td>", "Target Site"));
            builder.AppendLine(String.Format("<td>{0}</td>", error.TargetSite));
            builder.AppendLine("</tr>");

            builder.AppendLine("<tr>");
            builder.AppendLine(String.Format("<td nowrap='nowrap'>{0}</td>", "Date Time"));
            builder.AppendLine(String.Format("<td>{0}</td>", DateTime.UtcNow.ToString("yyyy-MMM-dd hh:mm:ss tt")));
            builder.AppendLine("</tr>");


            builder.AppendLine("<tr>");
            builder.AppendLine(String.Format("<td>{0}</td>", "Detail"));
            builder.AppendLine(String.Format("<td>{0}</td>", error.ToString()));
            builder.AppendLine("</tr>");

            builder.AppendLine("</table>");
            builder.AppendLine("</div></body></html>");

            string recipientEmail = Constants.GlobalErrorNotficationEmail;
            string senderEmail = Constants.AdminEmail;
            string senderName = Constants.AppName;


            string ccEmail = String.Empty;
            string replyToEmail = String.Empty;

            string Subject = Constants.AppName + " - Error ";
            string message = builder.ToString();

            //error notification is for admin only
            SmtpClient client = new SmtpClient();
            MailMessage m = new MailMessage();
            m.From = new MailAddress(senderEmail);
            m.To.Add(new MailAddress(recipientEmail));

            //message = message.Replace(cr, "<br />");
            message = message.Replace("\n", "<br />");
            m.IsBodyHtml = true;
            m.Subject = Subject;
            m.Body = message;

            client.Send(m);
        }

    }

}
