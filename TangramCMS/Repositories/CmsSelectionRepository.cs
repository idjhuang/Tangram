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
using TangramCMS.App_LocalResources;
using TangramCMS.Infrastructure;
using TangramCMS.Models;

namespace TangramCMS.Repositories
{
    public interface ICmsSelectionRepository
    {
        IEnumerable<string> ListSelections();
        JToken Get(string selectionId);
        JToken GetItemList(string selectionId);
        JToken Create(JObject selection);
        JToken Update(JObject selection);
        JToken UpdateItemList(string selectionId, JToken itemList);
        JToken Delete(string selectionId);
    }

    public class CmsSelectionRepository : ICmsSelectionRepository
    {
        private readonly MongoCollection<CmsSelection> _selectionCollection;

        public CmsSelectionRepository(ICmsService service)
        {
            _selectionCollection = service.Database.GetCollection<CmsSelection>("cms_selectons");
        }

        public IEnumerable<string> ListSelections()
        {
            return _selectionCollection.AsQueryable().Select(s => s.SelectionId);
        }

        public JToken Get(string selectionId)
        {
            // check existence of selection
            var selection = _selectionCollection.FindOne(Query<CmsSelection>.EQ(s => s.SelectionId, selectionId));
            if (selection == null) return Result(string.Format(CmsResource.SelectionNotExist, selectionId));
            var jsonStr = selection.ToJson().ConvertObjectId().ConvertIsoDate();
            return JToken.Parse(jsonStr);
        }

        public JToken GetItemList(string selectionId)
        {
            // check existence of selection
            var selection = _selectionCollection.FindOne(Query<CmsSelection>.EQ(s => s.SelectionId, selectionId));
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
            if (_selectionCollection.Count(Query<CmsSelection>.EQ(s => s.SelectionId, selectionId)) != 0)
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

            _selectionCollection.Insert(BsonSerializer.Deserialize<CmsSelection>(selection.ToString()));
            return Result(CmsResource.SelectionSaved, true);
        }

        public JToken Update(JObject selection)
        {
            // check existence of selection
            var idProperty = selection["_id"];
            if (idProperty == null) return Result(CmsResource.SelectionMissingId);
            var id = idProperty.Value<string>();
            if (_selectionCollection.Count(Query<CmsSelection>.EQ(s => s.Id, id)) == 0)
                return Result(CmsResource.SelectionNotExist);
            var selectionIdProperty = selection["SelectionId"];
            if (selectionIdProperty == null) return Result(CmsResource.SelectionMissingSelectionId);
            var selectionId = selectionIdProperty.Value<string>();
            if (_selectionCollection.Count(Query<CmsSelection>.EQ(s => s.SelectionId, selectionId)) == 0)
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

            _selectionCollection.Save(BsonSerializer.Deserialize<CmsSelection>(selection.ToString()));
            return Result(CmsResource.SelectionSaved, true);
        }

        public JToken UpdateItemList(string selectionId, JToken itemList)
        {
            // check existence of selection
            var selection = _selectionCollection.FindOne(Query<CmsSelection>.EQ(s => s.SelectionId, selectionId));
            if (selection == null) return Result(CmsResource.SelectionNotExist);

            // append cms properties
            var userName = GetUserName();
            var now = string.Format("{0:yyyy/MM/dd hh:mm:ss}", DateTime.Now);
            selection.ModifiedBy = userName;
            selection.LastModified = now;
            selection.ItemList = BsonSerializer.Deserialize<List<CmsSelectionItem>>(itemList.ToString());

            _selectionCollection.Save(selection);
            return Result(CmsResource.SelectionSaved, true);
        }

        public JToken Delete(string selectionId)
        {
            // check existence of selection
            if (_selectionCollection.Count(Query<CmsSelection>.EQ(s => s.SelectionId, selectionId)) == 0)
                return Result(string.Format(CmsResource.SelectionNotExist, selectionId));

            _selectionCollection.Remove(Query<CmsSelection>.EQ(s => s.SelectionId, selectionId));
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
            var result = new CmsResultModel
            {
                IsSuccess = isSuccess,
                Message = message
            };
            return JToken.FromObject(result);
        }
    }
}