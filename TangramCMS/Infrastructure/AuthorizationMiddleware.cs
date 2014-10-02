using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace TangramCMS.Infrastructure
{
    public class AuthorizationMiddleware : OwinMiddleware
    {
        public AuthorizationMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            var userManager = context.GetUserManager<ApplicationUserManager>();
            var userId = context.Authentication.User.Identity.GetUserId();
            Models.ApplicationUser currentUser;
            var roles = new List<string>();
            var isAuthorized = true;
            if (userId != null)
            {
                currentUser = userManager.FindById(userId);
                if (currentUser != null) roles = currentUser.Roles;
            }
            if (isAuthorized)
                await Next.Invoke(context);
            else
            {
                context.Response.StatusCode = 401;
            }
        }
    }
}