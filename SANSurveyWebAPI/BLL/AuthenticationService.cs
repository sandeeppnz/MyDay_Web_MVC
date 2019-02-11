using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using SANSurveyWebAPI.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace SANSurveyWebAPI.BLL
{

    public class AuthenticationService : IDisposable
    {
        /*
             Area account controller
        */

        readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public AuthenticationService()
        {
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }

        public async Task<bool> Logout(IOwinContext context)
        {
            var authenticationManager = context.Authentication;
            authenticationManager.SignOut();

            return true;
        }

        public int Login()
        {
            return 1;
        }




    }
}