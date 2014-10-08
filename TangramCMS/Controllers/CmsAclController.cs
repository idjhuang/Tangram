using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using TangramCMS.Infrastructure;
using TangramCMS.Models;
using TangramCMS.Repositories;

namespace TangramCMS.Controllers
{
    [Authorize(Roles = "Modelers")]
    [RoutePrefix("CmsAcl")]
    public class CmsAclController : ApiController
    {
        private readonly ICmsAclRepository _repository;

        public CmsAclController(ICmsAclRepository repository)
        {
            _repository = repository;
        }

        [Route("GetByCollection/{collectionId}")]
        [HttpGet]
        public IEnumerable<CmsAclModel> GetByCollection(string collectionId)
        {
            try
            {
                return _repository.GetByCollection(collectionId);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("GetByRole/{roleName}")]
        [HttpGet]
        public IEnumerable<CmsAclModel> GetByRole(string roleName)
        {
            try
            {
                return _repository.GetByRole(roleName);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [AllowAnonymous]
        [Route("GetAvailableCmsCollections")]
        [HttpGet]
        public IEnumerable<CmsCollection> GetAvailableCmsCollections()
        {
            try
            {
                return
                    _repository.GetAvailableCmsCollections(AuthorizationMiddleware.GetUserRoles(Request.GetOwinContext()));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("Create")]
        [HttpPost]
        public CmsResultModel Create([FromBody] CmsAclModel acl)
        {
            try
            {
                return _repository.Create(acl);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("Update")]
        [HttpPut]
        public CmsResultModel Update([FromBody] CmsAclModel acl)
        {
            try
            {
                return _repository.Update(acl);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("Delete/{collectionId}/{roleName}")]
        [HttpDelete]
        public CmsResultModel Delete(string collectionId, string roleName)
        {
            try
            {
                return _repository.Delete(collectionId, roleName);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("DeleteByRole/{roleName}")]
        [HttpDelete]
        public CmsResultModel DeleteByRole(string roleName)
        {
            try
            {
                return _repository.DeleteByRole(roleName);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }
    }
}
