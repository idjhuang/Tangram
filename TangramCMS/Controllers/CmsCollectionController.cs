using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Practices.Unity;
using TangramCMS.Infrastructure;
using TangramCMS.Models;
using TangramCMS.Repositories;

namespace TangramCMS.Controllers
{
    [Authorize(Roles = "Modelers")]
    [RoutePrefix("CmsCollection")]
    public class CmsCollectionController : ApiController
    {
        private readonly ICmsCollectionRepository _cmsCollectionRepository;

        public CmsCollectionController(ICmsCollectionRepository cmsCollectionRepository)
        {
            _cmsCollectionRepository = cmsCollectionRepository;
        }

        [Route("GetAll/{documentType?}")]
        [HttpGet]
        public IEnumerable<CmsCollection> GetAll(string documentType = null)
        {
            try
            {
                return _cmsCollectionRepository.GatAll(documentType);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [AllowAnonymous]
        [Route("GetAvailable")]
        [HttpGet]
        public IEnumerable<CmsCollection> GetAvailable()
        {
            try
            {
                var aclRepository = UnityConfig.Container.Resolve<ICmsAclRepository>();
                return
                    aclRepository.GetAvailableCmsCollections(AuthorizationMiddleware.GetUserRoles(Request.GetOwinContext()));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("Get/{collectionId}")]
        [HttpGet]
        public CmsCollection Get(string collectionId)
        {
            try
            {
                return _cmsCollectionRepository.Get(collectionId);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("Create")]
        [HttpPost]
        public CmsResultModel Create([FromBody] CmsCollection cmsCollection)
        {
            try
            {
                return _cmsCollectionRepository.Create(cmsCollection);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }            
        }

        [Route("Delete/{collectionId}")]
        [HttpDelete]
        public CmsResultModel Delete(string collectionId)
        {
            try
            {
                return _cmsCollectionRepository.Delete(collectionId);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }
    }
}
