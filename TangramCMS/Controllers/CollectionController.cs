using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Practices.Unity;
using TangramService.Infrastructure;
using TangramService.Models;
using TangramService.Repositories;

namespace TangramService.Controllers
{
    [Authorize(Roles = "Modelers")]
    [RoutePrefix("CmsCollection")]
    public class CollectionController : ApiController
    {
        private readonly ICollectionRepository _collectionRepository;

        public CollectionController(ICollectionRepository collectionRepository)
        {
            _collectionRepository = collectionRepository;
        }

        [Route("GetAll/{documentType?}")]
        [HttpGet]
        public IEnumerable<Collection> GetAll(string documentType = null)
        {
            try
            {
                return _collectionRepository.GatAll(documentType);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [AllowAnonymous]
        [Route("GetAvailable/{right?}")]
        [HttpGet]
        public IEnumerable<Collection> GetAvailable(string right = "w")
        {
            try
            {
                var aclRepository = UnityConfig.Container.Resolve<IAuthorizationRepository>();
                return
                    aclRepository.GetAvailableCmsCollections(AuthorizationMiddleware.GetUserRoles(Request.GetOwinContext()), right);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("Get/{collectionId}")]
        [HttpGet]
        public Collection Get(string collectionId)
        {
            try
            {
                return _collectionRepository.Get(collectionId);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("Create")]
        [HttpPost]
        public ResultModel Create([FromBody] Collection collection)
        {
            try
            {
                return _collectionRepository.Create(collection);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }            
        }

        [Route("Delete/{collectionId}")]
        [HttpDelete]
        public ResultModel Delete(string collectionId)
        {
            try
            {
                return _collectionRepository.Delete(collectionId);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }
    }
}
