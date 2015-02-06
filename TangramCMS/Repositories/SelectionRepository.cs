using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using Newtonsoft.Json.Linq;
using TangramService.Infrastructure;
using TangramService.App_LocalResources;
using TangramService.Models;

namespace TangramService.Repositories
{
    public interface ISelectionRepository
    {
        IEnumerable<string> ListSelections();
        JToken Get(string selectionId);
        JToken GetItemList(string selectionId);
        JToken Create(JObject selection);
        JToken Update(JObject selection);
        JToken UpdateItemList(string selectionId, JToken itemList);
        JToken Delete(string selectionId);
    }

    public class SelectionRepository : ISelectionRepository
    {
        private readonly MongoCollection<Selection> _cmsSelections;

        public SelectionRepository(IService service)
        {
            _cmsSelections = service.Database.GetCollection<Selection>("cms_selectons");
        }

        public IEnumerable<string> ListSelections()
        {
            return _cmsSelections.AsQueryable().Select(s => s.SelectionId);
        }

        public JToken Get(string selectionId)
        {
            // check existence of selection
            var selection = _cmsSelections.FindOne(Query<Selection>.EQ(s => s.SelectionId, selectionId));
            if (selection == null) return Result(string.Format(CmsResource.SelectionNotExist, selectionId));
            var jsonStr = selection.ToJson().ConvertObjectId().ConvertIsoDate();
            return JToken.Parse(jsonStr);
        }

        public JToken GetItemList(string selectionId)
        {
            // check existence of selection
            var selection = _cmsSelections.FindOne(Query<Selection>.EQ(s => s.SelectionId, selectionId));
            if (selection == null) return Result(string.Format(CmsResource.SelectionNotExist, selectionId));
            var jsonStr = selection.ItemList.ToJson().ConvertObjectId().ConvertIsoDate();
            return JToken.Parse(jsonStr);            
        }

        public JToken Create(JObject selection)
        {
            // check existence of selection
            var selectionIdProperty = selection["SelectionId"];
            if (selectionIdProperty == null) return Result(CmsResource.SelectionMissingSelectionId);
            var selectionId = selectionIdProperty.Value<string>();
            if (_cmsSelections.Count(Query<Selection>.EQ(s => s.SelectionId, selectionId)) != 0)
                return Result(CmsResource.SelectionAlreadyExist);

            // append cms properties
            var userName = GetUserName();
            var now = string.Format("{0:yyyy/MM/dd hh:mm:ss}", DateTime.Now);
            selection.Merge(JObject.FromObject(
                new
                {
                    CreatedDate = now,
                    LastModified = now,
                    CreatedBy = userName,
                    ModifiedBy = userName
                }));

            _cmsSelections.Insert(BsonSerializer.Deserialize<Selection>(selection.ToString()));
            return Result(CmsResource.SelectionSaved, true);
        }

        public JToken Update(JObject selection)
        {
            // check existence of selection
            var idProperty = selection["_id"];
            if (idProperty == null) return Result(CmsResource.SelectionMissingId);
            var id = idProperty.Value<string>();
            if (_cmsSelections.Count(Query<Selection>.EQ(s => s.Id, id)) == 0)
                return Result(CmsResource.SelectionNotExist);
            var selectionIdProperty = selection["SelectionId"];
            if (selectionIdProperty == null) return Result(CmsResource.SelectionMissingSelectionId);
            var selectionId = selectionIdProperty.Value<string>();
            if (_cmsSelections.Count(Query<Selection>.EQ(s => s.SelectionId, selectionId)) == 0)
                return Result(CmsResource.SelectionNotExist);

            // append cms properties
            var userName = GetUserName();
            var now = string.Format("{0:yyyy/MM/dd hh:mm:ss}", DateTime.Now);
            selection.Merge(JObject.FromObject(
                new
                {
                    LastModified = now,
                    ModifiedBy = userName
                }));

            _cmsSelections.Save(BsonSerializer.Deserialize<Selection>(selection.ToString()));
            return Result(CmsResource.SelectionSaved, true);
        }

        public JToken UpdateItemList(string selectionId, JToken itemList)
        {
            // check existence of selection
            var selection = _cmsSelections.FindOne(Query<Selection>.EQ(s => s.SelectionId, selectionId));
            if (selection == null) return Result(CmsResource.SelectionNotExist);

            // append cms properties
            var userName = GetUserName();
            var now = string.Format("{0:yyyy/MM/dd hh:mm:ss}", DateTime.Now);
            selection.ModifiedBy = userName;
            selection.LastModified = now;
            selection.ItemList = BsonSerializer.Deserialize<List<SelectionItem>>(itemList.ToString());

            _cmsSelections.Save(selection);
            return Result(CmsResource.SelectionSaved, true);
        }

        public JToken Delete(string selectionId)
        {
            // check existence of selection
            if (_cmsSelections.Count(Query<Selection>.EQ(s => s.SelectionId, selectionId)) == 0)
                return Result(string.Format(CmsResource.SelectionNotExist, selectionId));

            _cmsSelections.Remove(Query<Selection>.EQ(s => s.SelectionId, selectionId));
            return Result(string.Format(CmsResource.SelectionDeleted, selectionId), true);
        }

        private string GetUserName()
        {
            var request = HttpContext.Current.Request;
            var identity = request.GetOwinContext().Authentication.User.Identity;
            return identity.IsAuthenticated
                ? identity.Name
                : string.Format(CmsResource.AnonymousName, request.UserHostAddress);
        }

        private JToken Result(string message, bool isSuccess = false)
        {
            var result = new ResultModel
            {
                IsSuccess = isSuccess,
                Message = message
            };
            return JToken.FromObject(result);
        }
    }
}