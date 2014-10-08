using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using AspNet.Identity.MongoDB;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using TangramCMS.App_LocalResources;
using TangramCMS.Models;
using TangramCMS.Repositories;

namespace TangramCMS.Controllers
{
    [Authorize(Roles = "Administrators")]
    [RoutePrefix("UserAdmin")]
    public class UserAdminController : ApiController
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationRoleManager _roleManager;
        private readonly ICmsAclRepository _aclRepository;

        public UserAdminController(ICmsAclRepository aclRepository)
        {
            _aclRepository = aclRepository;
            _userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            _roleManager = Request.GetOwinContext().Get<ApplicationRoleManager>();
        }
        /*
        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        public ApplicationRoleManager RoleManager
        {
            get { return _roleManager ?? Request.GetOwinContext().Get<ApplicationRoleManager>(); }
            private set { _roleManager = value; }
        }
        */
        [Route("ListUsers/{roleName?}/{exclude?}")]
        [HttpGet]
        public IEnumerable<ApplicationUser> ListUsers(string roleName = null, bool exclude = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName)) return _userManager.Users;
                return exclude
                    ? _userManager.Users.Where(u => !u.Roles.Contains(roleName))
                    : _userManager.Users.Where(u => u.Roles.Contains(roleName));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("ListRoles")]
        [HttpGet]
        public IEnumerable<IdentityRole> ListRoles()
        {
            try
            {
                return _roleManager.Roles;
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("CreateUser")]
        [HttpPost]
        public async Task<CmsResultModel> CreateUser(RegisterBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid) return ModelStateErrors(ModelState);

                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded) return IdentityResultErrors(result);
                return new CmsResultModel
                {
                    IsSuccess = true,
                    Message = string.Format(CmsResource.UserCreated, model.UserName)
                };
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("SuspendUser/{userName}")]
        [HttpPut]
        public async Task<CmsResultModel> SuspendUser(string userName)
        {
            try
            {
                // check existence of user
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                    return new CmsResultModel
                    {
                        IsSuccess = false,
                        Message = string.Format(CmsResource.UserNotExist, userName)
                    };

                user.LockoutEnabled = true;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded) return IdentityResultErrors(result);
                return new CmsResultModel
                {
                    IsSuccess = true,
                    Message = string.Format(CmsResource.UserSuspended, userName)
                };
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("RestoreUser/{userName}")]
        [HttpPut]
        public async Task<CmsResultModel> RestoreUser(string userName)
        {
            try
            {
                // check existence of user
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                    return new CmsResultModel
                    {
                        IsSuccess = false,
                        Message = string.Format(CmsResource.UserNotExist, userName)
                    };

                user.LockoutEnabled = false;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded) return IdentityResultErrors(result);
                return new CmsResultModel
                {
                    IsSuccess = true,
                    Message = string.Format(CmsResource.UserRestored, userName)
                };
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("CreateRole/{roleName}")]
        [HttpPost]
        public async Task<CmsResultModel> CreateRole(string roleName)
        {
            try
            {
                // check existence of role
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                    return new CmsResultModel
                    {
                        IsSuccess = false,
                        Message = string.Format(CmsResource.RoleAlreadyExist, roleName)
                    };

                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

                if (!result.Succeeded) return IdentityResultErrors(result);
                return new CmsResultModel
                {
                    IsSuccess = true,
                    Message = string.Format(CmsResource.RoleCreated, roleName)
                };
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("DeleteRole/{roleName}")]
        [HttpDelete]
        public async Task<CmsResultModel> DeleteRole(string roleName)
        {
            try
            {
                // check existence of role
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                    return new CmsResultModel
                    {
                        IsSuccess = false,
                        Message = string.Format(CmsResource.RoleNotExist, roleName)
                    };
                // delete acls of deleted role
                var aclResult = _aclRepository.DeleteByRole(roleName);
                if (!aclResult.IsSuccess) return aclResult;
                // remove role from all users with it
                var users = _userManager.Users.Where(u => u.Roles.Contains(roleName));
                foreach (var user in users)
                {
                    user.RemoveRole(roleName);
                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded) return IdentityResultErrors(updateResult);
                }

                var result = await _roleManager.DeleteAsync(role);

                if (!result.Succeeded) return IdentityResultErrors(result);
                return new CmsResultModel
                {
                    IsSuccess = true,
                    Message = string.Format(CmsResource.RoleDeleted, roleName)
                };
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("AddRole/{userName}/{roleName}")]
        [HttpPut]
        public async Task<CmsResultModel> AddRole(string userName, string roleName)
        {
            try
            {
                // check existence of user
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                    return new CmsResultModel
                    {
                        IsSuccess = false,
                        Message = string.Format(CmsResource.UserNotExist, userName)
                    };
                // check role already added to user
                if (user.Roles.Contains(roleName))
                    return new CmsResultModel
                    {
                        IsSuccess = false,
                        Message = string.Format(CmsResource.RoleAlreadyAdded, roleName, userName)
                    };
                // check existence of role
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                    return new CmsResultModel
                    {
                        IsSuccess = false,
                        Message = string.Format(CmsResource.RoleNotExist, roleName)
                    };

                user.AddRole(roleName);
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded) return IdentityResultErrors(result);
                return new CmsResultModel
                {
                    IsSuccess = true,
                    Message = string.Format(CmsResource.RoleAdded, userName, roleName)
                };
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("AddRole/{roleName}")]
        [HttpPut]
        public async Task<CmsResultModel> AddRole([FromBody]List<string> userNameList, string roleName)
        {
            try
            {
                // check existence of role
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                    return new CmsResultModel
                    {
                        IsSuccess = false,
                        Message = string.Format(CmsResource.RoleNotExist, roleName)
                    };
                var errorMsg = new StringBuilder();
                var isSuccess = true;
                foreach (var userName in userNameList)
                {
                    // check existence of user
                    var user = await _userManager.FindByNameAsync(userName);
                    if (user == null)
                    {
                        isSuccess = false;
                        errorMsg.AppendLine(string.Format(CmsResource.UserNotExist, userName));
                        continue;
                    }
                    // check role already added to user
                    if (user.Roles.Contains(roleName))
                    {
                        isSuccess = false;
                        errorMsg.AppendLine(string.Format(CmsResource.RoleAlreadyAdded, roleName, userName));
                        continue;
                    }

                    user.AddRole(roleName);
                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded) continue;
                    isSuccess = false;
                    errorMsg.Append(IdentityResultErrors(result).Message);
                }
                return new CmsResultModel
                {
                    IsSuccess = isSuccess,
                    Message = errorMsg.ToString()
                };
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("RemoveRole/{roleName}")]
        [HttpPut]
        public async Task<CmsResultModel> RemoveRole([FromBody]List<string> userNameList, string roleName)
        {
            try
            {
                var errorMsg = new StringBuilder();
                var isSuccess = true;
                foreach (var userName in userNameList)
                {
                    // check existence of user
                    var user = await _userManager.FindByNameAsync(userName);
                    if (user == null)
                    {
                        isSuccess = false;
                        errorMsg.AppendLine(string.Format(CmsResource.UserNotExist, userName));
                        continue;
                    }
                    // check existence of role
                    if (!user.Roles.Contains(roleName))
                    {
                        isSuccess = false;
                        errorMsg.AppendLine(string.Format(CmsResource.RoleNotExist, roleName));
                        continue;
                    }

                    user.RemoveRole(roleName);
                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded) continue;
                    isSuccess = false;
                    errorMsg.Append(IdentityResultErrors(result).Message);
                }
                return new CmsResultModel
                {
                    IsSuccess = isSuccess,
                    Message = errorMsg.ToString()
                };
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [AllowAnonymous]
        [Route("Initialize")]
        [HttpPost]
        public async Task<CmsResultModel> Initialize(RegisterBindingModel model)
        {
            try
            {
                // validate initialization
                if (_userManager.Users.Any() || _roleManager.Roles.Any())
                    return new CmsResultModel
                    {
                        IsSuccess = false,
                        Message = CmsResource.InvalidInitialization
                    };
                // insert role Administrators
                var result = await _roleManager.CreateAsync(new IdentityRole("Administrators"));
                if (!result.Succeeded) return IdentityResultErrors(result);
                // insert specific administrator
                if (!ModelState.IsValid) return ModelStateErrors(ModelState);
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                user.Roles.Add("Administrators");
                result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded) return IdentityResultErrors(result);
                return new CmsResultModel
                {
                    IsSuccess = true,
                    Message = string.Format(CmsResource.InitializeSucceeded)
                };
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [AllowAnonymous]
        [Route("RecoverAdministrators/{adminName}")]
        [HttpPost]
        public async Task<CmsResultModel> RecoverAdministrators(string adminName)
        {
            try
            {
                // validate administrators recovery
                if (_userManager.Users.Any(user => user.Roles.Contains("Administrators")))
                    return new CmsResultModel
                    {
                        IsSuccess = false,
                        Message = CmsResource.InvalidAdministratorsRecovery
                    };
                // check existence of role Administrators, insert if not exist
                if (!_roleManager.Roles.Any(role => role.Name.Equals("Administrators")))
                {
                    var addRoleResult = await _roleManager.CreateAsync(new IdentityRole("Administrators"));
                    if (!addRoleResult.Succeeded) return IdentityResultErrors(addRoleResult);
                }
                // check existence of specific administrator
                var admin = await _userManager.FindByNameAsync(adminName);
                if (admin == null)
                    return new CmsResultModel
                    {
                        IsSuccess = false,
                        Message = string.Format(CmsResource.UserNotExist, adminName)
                    };
                // add role Administrators to administrator
                admin.Roles.Add("Administrators");
                var result = await _userManager.UpdateAsync(admin);
                if (!result.Succeeded) return IdentityResultErrors(result);
                return new CmsResultModel
                {
                    IsSuccess = true,
                    Message = CmsResource.RecoverAdministratorsSucceeded
                };
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        private CmsResultModel ModelStateErrors(ModelStateDictionary modelState)
        {
            var errorMsg = new StringBuilder();
            foreach (var error in modelState.Values.SelectMany(s => s.Errors))
            {
                errorMsg.AppendLine(error.ErrorMessage);
            }
            return new CmsResultModel
            {
                IsSuccess = false,
                Message = errorMsg.ToString()
            };            
        }

        private CmsResultModel IdentityResultErrors(IdentityResult result)
        {
            var errorMsg = new StringBuilder();
            foreach (var error in result.Errors)
            {
                errorMsg.AppendLine(error);
            }
            return new CmsResultModel
            {
                IsSuccess = false,
                Message = errorMsg.ToString()
            };
        }
    }
}
