using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using TangramCMS.Repositories;

namespace TangramCMS.Controllers
{
    //[Authorize(Roles = "Modelers")]
    [RoutePrefix("CmsSelection")]
    public class CmsSelectionController : ApiController
    {
        private readonly ICmsSelectionRepository _cmsSelectionRepository;

        public CmsSelectionController(ICmsSelectionRepository cmsSelectionRepository)
        {
            _cmsSelectionRepository = cmsSelectionRepository;
        }

        [Route("List")]
        [HttpGet]
        public IEnumerable<string> List()
        {
            try
            {
                return _cmsSelectionRepository.ListSelections();
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("Get/{selectionId}")]
        [HttpGet]
        public JToken Get(string selectionId)
        {
            try
            {
                return _cmsSelectionRepository.Get(selectionId);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("GetItems/{selectionId}")]
        [HttpGet]
        public JToken GetItems(string selectionId)
        {
            try
            {
                return _cmsSelectionRepository.GetItemList(selectionId);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("Create")]
        [HttpPost]
        public JToken Create([FromBody] JObject selection)
        {
            try
            {
                return _cmsSelectionRepository.Create(selection);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }            
        }

        [Route("Update")]
        [HttpPut]
        public JToken Update([FromBody] JObject selection)
        {
            try
            {
                return _cmsSelectionRepository.Update(selection);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("UpdateItems/{selectionId}")]
        [HttpPut]
        public JToken UpdateItems(string selectionId, [FromBody] JToken items)
        {
            try
            {
                return _cmsSelectionRepository.UpdateItemList(selectionId, items);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }

        [Route("Delete/{selectionId}")]
        [HttpDelete]
        public JToken Delete(string selectionId)
        {
            try
            {
                return _cmsSelectionRepository.Delete(selectionId);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }
    }
}
