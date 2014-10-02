using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json.Linq;
using TangramCMS.App_LocalResources;
using TangramCMS.Infrastructure;
using TangramCMS.Repositories;

namespace TangramCMS.Controllers
{
    public class CollectionController : ApiController
    {
        private ICmsCollectionRepository _repository;

        public CollectionController(ICmsCollectionRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get all documents from collecion
        /// </summary>
        /// <param name="collectionId"></param>
        /// <returns></returns>
        [Route("Collection/GetAll/{collectionId}")]
        [HttpGet]
        public JToken GetAll(string collectionId)
        {
            return _repository.GetAll(Request.GetOwinContext(), collectionId);
        }

        /// <summary>
        /// Get document from collection by document Id
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [Route("Collection/Get/{collectionId}/{documentId}")]
        [HttpGet]
        public JToken Get(string collectionId, string documentId)
        {
            return _repository.GetById(Request.GetOwinContext(), collectionId, documentId);
        }

        /// <summary>
        /// Get parent of document
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [Route("Collection/GetParent/{collectionId}/{documentId}")]
        [HttpGet]
        public JToken GetParent(string collectionId, string documentId)
        {
            return _repository.GetParent(Request.GetOwinContext(), collectionId, documentId);
        }

        /// <summary>
        /// Get children of document
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [Route("Collection/GetChildren/{collectionId}/{documentId}")]
        [HttpGet]
        public JToken GetChildren(string collectionId, string documentId)
        {
            return _repository.GetChildren(Request.GetOwinContext(), collectionId, documentId);
        }

        /// <summary>
        /// Insert document into collection
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        [Route("Collection/Insert/{collectionId}")]
        [HttpPost]
        public JToken Insert(string collectionId, [FromBody]JObject document)
        {
            return _repository.Insert(Request.GetOwinContext(), collectionId, document);
        }

        /// <summary>
        /// Update document
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        [Route("Collection/Update/{collectionId}")]
        [HttpPut]
        public JToken Update(string collectionId, [FromBody]JObject document)
        {
            return _repository.Update(Request.GetOwinContext(), collectionId, document);
        }

        /// <summary>
        /// Delete document
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [Route("Collection/Delete/{collectionId}/{documentId}")]
        [HttpDelete]
        public JToken Delete(string collectionId, string documentId)
        {
            return _repository.Delete(Request.GetOwinContext(), collectionId, documentId);
        }

        /// <summary>
        /// Set parent of document
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="documentId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [Route("Collection/SetParent/{collectionId}/{documentId}/{parentId?}")]
        [HttpGet]
        public JToken SetParent(string collectionId, string documentId, string parentId = null)
        {
            return _repository.SetParent(Request.GetOwinContext(), collectionId, documentId, parentId);
        }

        [Route("Test/{collectionId}")]
        [HttpGet]
        public void Test(string collectionId)
        {
            _repository.Test(collectionId);
        }
    }
}
