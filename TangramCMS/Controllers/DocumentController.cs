using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using TangramService.Repositories;

namespace TangramService.Controllers
{
    [RoutePrefix("CmsDocument")]
    public class DocumentController : ApiController
    {
        private readonly IDocumentRepository _documentRepository;

        public DocumentController(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        /// <summary>
        /// Get all documents from collecion
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="limit"></param>
        /// <param name="skip"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        [Route("GetAll/{collectionId}")]
        [HttpGet]
        public JToken GetAll(string collectionId, int limit = 0, int skip = 0, bool random = false)
        {
            try
            {
                return _documentRepository.GetAll(collectionId, limit, skip, random);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("Query/{collectionId}")]
        [HttpGet]
        public JToken Query(string collectionId, string query = null, string order = null, int limit = 0, int skip = 0, bool random = false)
        {
            try
            {
                return _documentRepository.GetByQuery(collectionId, query, order, limit, skip, random);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        /// <summary>
        /// Get document from collection by document Id
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [Route("Get/{collectionId}/{documentId}")]
        [HttpGet]
        public JToken Get(string collectionId, string documentId)
        {
            try
            {
                return _documentRepository.GetById(collectionId, documentId);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        /// <summary>
        /// Get parent of document
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [Route("GetParent/{collectionId}/{documentId}")]
        [HttpGet]
        public JToken GetParent(string collectionId, string documentId)
        {
            try
            {
                return _documentRepository.GetParent(collectionId, documentId);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        /// <summary>
        /// Get children of document
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [Route("GetChildren/{collectionId}/{documentId}")]
        [HttpGet]
        public JToken GetChildren(string collectionId, string documentId)
        {
            try
            {
                return _documentRepository.GetChildren(collectionId, documentId);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        /// <summary>
        /// Insert document into collection
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        [Route("Create/{collectionId}")]
        [HttpPost]
        public JToken Create(string collectionId, [FromBody]JObject document)
        {
            try
            {
                return _documentRepository.Create(collectionId, document);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        /// <summary>
        /// Update document
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        [Route("Update/{collectionId}")]
        [HttpPut]
        public JToken Update(string collectionId, [FromBody]JObject document)
        {
            try
            {
                return _documentRepository.Update(collectionId, document);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        /// <summary>
        /// Delete document
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [Route("Delete/{collectionId}/{documentId}")]
        [HttpDelete]
        public JToken Delete(string collectionId, string documentId)
        {
            try
            {
                return _documentRepository.Delete(collectionId, documentId);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        /// <summary>
        /// Set parent of document
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="documentId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [Route("SetParent/{collectionId}/{documentId}/{parentId?}")]
        [HttpPut]
        public JToken SetParent(string collectionId, string documentId, string parentId = null)
        {
            try
            {
                return _documentRepository.SetParent(collectionId, documentId, parentId);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }
    }
}
