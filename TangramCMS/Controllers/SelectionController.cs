using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using TangramService.Repositories;

namespace TangramService.Controllers
{
    //[Authorize(Roles = "Modelers")]
    [RoutePrefix("CmsSelection")]
    public class SelectionController : ApiController
    {
        private readonly ISelectionRepository _selectionRepository;

        public SelectionController(ISelectionRepository selectionRepository)
        {
            _selectionRepository = selectionRepository;
        }

        [Route("List")]
        [HttpGet]
        public IEnumerable<string> List()
        {
            try
            {
                return _selectionRepository.ListSelections();
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
                return _selectionRepository.Get(selectionId);
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
                return _selectionRepository.GetItemList(selectionId);
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
                return _selectionRepository.Create(selection);
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
                return _selectionRepository.Update(selection);
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
                return _selectionRepository.UpdateItemList(selectionId, items);
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
                return _selectionRepository.Delete(selectionId);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
        }
    }
}
