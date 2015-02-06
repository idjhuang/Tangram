using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using TangramService.Infrastructure;
using TangramService.Models;
using TangramService.Repositories;

namespace TangramService.Controllers
{
    [Authorize(Roles = "Modelers")]
    [RoutePrefix("CmsAcl")]
    public class AuthorizationController : ApiController
    {
        private readonly IAuthorizationRepository _authorizationRepository;

        public AuthorizationController(IAuthorizationRepository authorizationRepository)
        {
            _authorizationRepository = authorizationRepository;
        }

        [Route("GetByCollection/{collectionId}")]
        [HttpGet]
        public IEnumerable<AuthorizationModel> GetByCollection(string collectionId)
        {
            try
            {
                return _authorizationRepository.GetByCollection(collectionId);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("GetByRole/{roleName}")]
        [HttpGet]
        public IEnumerable<AuthorizationModel> GetByRole(string roleName)
        {
            try
            {
                return _authorizationRepository.GetByRole(roleName);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [AllowAnonymous]
        [Route("GetAvailableCmsCollections/{right?}")]
        [HttpGet]
        public IEnumerable<Collection> GetAvailableCmsCollections(string right = "w")
        {
            try
            {
                return
                    _authorizationRepository.GetAvailableCmsCollections(AuthorizationMiddleware.GetUserRoles(Request.GetOwinContext()), right);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("Create")]
        [HttpPost]
        public ResultModel Create([FromBody] AuthorizationModel authorization)
        {
            try
            {
                return _authorizationRepository.Create(authorization);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("Update")]
        [HttpPut]
        public ResultModel Update([FromBody] AuthorizationModel authorization)
        {
            try
            {
                return _authorizationRepository.Update(authorization);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("Delete/{collectionId}/{roleName}")]
        [HttpDelete]
        public ResultModel Delete(string collectionId, string roleName)
        {
            try
            {
                return _authorizationRepository.Delete(collectionId, roleName);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("DeleteByRole/{roleName}")]
        [HttpDelete]
        public ResultModel DeleteByRole(string roleName)
        {
            try
            {
                return _authorizationRepository.DeleteByRole(roleName);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }
    }
}
