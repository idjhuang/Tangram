using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Practices.Unity;
using TangramCMS.App_LocalResources;
using TangramCMS.Repositories;

namespace TangramCMS.Infrastructure
{
    public class AuthorizationMiddleware : OwinMiddleware
    {
        public AuthorizationMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            try
            {
                var isAuthorized = true;
                var roles = GetUserRoles(context);
                string action;
                string collectionId;
                if (ParsePath(context.Request.Path.ToString(), out action, out collectionId))
                {
                    // check acl
                    var repository = UnityConfig.Container.Resolve<ICmsAclRepository>();
                    var right = context.Request.Method.Equals("GET") ? "r" : "w";
                    isAuthorized = repository.CheckRight(collectionId, roles, right);
                }
                if (isAuthorized)
                    await Next.Invoke(context);
                else
                {
                    UnauthorizedResponse(context, CmsResource.NoAuthorization);
                }
            }
            catch (Exception e)
            {
                UnauthorizedResponse(context, e.Message);
            }
        }

        public static List<string> GetUserRoles(IOwinContext context)
        {
            var roles = new List<string> { "anonymous" };
            if (context.Authentication.User.Identity.IsAuthenticated) roles.Add("members");
            var userManager = context.GetUserManager<ApplicationUserManager>();
            var userId = context.Authentication.User.Identity.GetUserId();
            Models.ApplicationUser currentUser;
            if (userId != null)
            {
                currentUser = userManager.FindById(userId);
                if (currentUser != null) roles.AddRange(currentUser.Roles);
            }
            return roles;
        }

        private bool ParsePath(string path, out string action, out string collectionId)
        {
            action = null;
            collectionId = null;
            var tokens = path.Split('/');
            if (tokens.Length < 4) return false;
            if (!tokens[1].ToLower().Equals("cmsdocument")) return false;
            action = tokens[2].ToLower();
            collectionId = tokens[3];
            return true;
        }

        private void UnauthorizedResponse(IOwinContext context, string message)
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            context.Response.Write(string.Format("{{\"IsSuccess\": false, \"Message\": \"{0}\"}}", message));            
        }
    }
}