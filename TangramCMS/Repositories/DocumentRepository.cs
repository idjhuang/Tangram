using System;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
//using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json.Linq;
using TangramService.Infrastructure;
using TangramService.App_LocalResources;
using TangramService.Models;

namespace TangramService.Repositories
{
    public interface IDocumentRepository
    {
        JToken GetAll(string collectionId, int limit = 0, int skip = 0, bool randomOrder = false);
        JToken GetByQuery(string collectionId, string queryStr, string orderStr, int limit = 0,
            int skip = 0, bool randomOrder = false);
        JToken GetById(string collectionId, string documentId);
        JToken GetParent(string collectionId, string documentId);
        JToken GetChildren(string collectionId, string documentId);
        JToken Create(string collectionId, JObject document);
        JToken Update(string collectionId, JObject document);
        JToken Delete(string collectionId, string documentId);
        JToken SetParent(string collectionId, string documentId, string parentId);
    }

    public class DocumentRepository : IDocumentRepository
    {
        private MongoDatabase _database;

        public DocumentRepository(IService service)
        {
            _database = service.Database;
            //var convensionPak = new ConventionPack();
            //convensionPak.Add(new StringObjectIdIdGeneratorConvention());
            //ConventionRegistry.Register("CMS", convensionPak, t => t.Name.Equals("BsonDocument") || t.Name.Equals("CmsDocument"));
        }
        public JToken GetAll(string collectionId, int limit = 0, int skip = 0, bool randomOrder = false)
        {
            var collection = _database.GetCollection<Document>(collectionId);
            if (collection == null) return Result(string.Format(CmsResource.CollectionNotExist, collectionId));
            var cursor = collection.FindAll();
            if (limit == 0) limit = (int)cursor.Count();
            var rng = new Random((int)DateTime.Now.Ticks);
            var jsonStr = randomOrder
                ? cursor.OrderBy(d => rng.Next()).Skip(skip).Take(limit).ToJson().ConvertObjectId().ConvertIsoDate()
                : cursor.Skip(skip).Take(limit).ToJson().ConvertObjectId().ConvertIsoDate();
            return JToken.Parse(jsonStr);
        }

        public JToken GetByQuery(string collectionId, string queryStr, string orderStr, int limit = 0,
            int skip = 0, bool randomOrder = false)
        {
            var collection = _database.GetCollection<Document>(collectionId);
            if (collection == null) return Result(string.Format(CmsResource.CollectionNotExist, collectionId));
            var list = collection.Query<Document>(queryStr, orderStr);
            if (limit == 0) limit = list.Count();
            var rng = new Random((int)DateTime.Now.Ticks);
            var jsonStr = randomOrder
                ? list.OrderBy(d => rng.Next()).Skip(skip).Take(limit).ToJson().ConvertObjectId().ConvertIsoDate()
                : list.Skip(skip).Take(limit).ToJson().ConvertObjectId().ConvertIsoDate();
            return JToken.Parse(jsonStr);
        }

        public JToken GetById(string collectionId, string documentId)
        {
            var collection = _database.GetCollection<Document>(collectionId);
            if (collection == null) return Result(string.Format(CmsResource.CollectionNotExist, collectionId));
            var document = collection.FindOne(Query<Document>.EQ(d => d.Id, documentId));
            if (document == null) return Result(string.Format(CmsResource.DocumentNotExist, documentId));
            var jsonStr = document.ToJson().ConvertObjectId().ConvertIsoDate();
            return JToken.Parse(jsonStr);
        }

        public JToken GetParent(string collectionId, string documentId)
        {
            var collection = _database.GetCollection<Document>(collectionId);
            if (collection == null) return Result(string.Format(CmsResource.CollectionNotExist, collectionId));
            var document = collection.FindOne(Query<Document>.EQ(d => d.Id, documentId));
            if (document == null) return Result(string.Format(CmsResource.DocumentNotExist, documentId));
            var parent = collection.FindOne(Query<Document>.EQ(d => d.Id, document.ParentId));
            var jsonStr = (parent == null) ? "{}" : parent.ToJson().ConvertObjectId().ConvertIsoDate();
            return JToken.Parse(jsonStr);
        }

        public JToken GetChildren(string collectionId, string documentId)
        {
            var collection = _database.GetCollection<Document>(collectionId);
            if (collection == null) return Result(string.Format(CmsResource.CollectionNotExist, collectionId));
            var document = collection.FindOne(Query<Document>.EQ(d => d.Id, documentId));
            if (document == null) return Result(string.Format(CmsResource.DocumentNotExist, documentId));
            var children = collection.Find(Query<Document>.EQ(d => d.ParentId, document.Id));
            var jsonStr = children.ToJson().ConvertObjectId().ConvertIsoDate();
            return JToken.Parse(jsonStr);
        }

        public JToken Create(string collectionId, JObject document)
        {
            var collection = _database.GetCollection<Document>(collectionId);
            if (collection == null) return Result(string.Format(CmsResource.CollectionNotExist, collectionId));
            // check existence of document
            var idProperty = document["_id"];
            if (idProperty != null)
            {
                var documentId = idProperty.Value<string>();
                if (!string.IsNullOrWhiteSpace(documentId))
                {
                    var origDocument = collection.FindOne(Query<Document>.EQ(d => d.Id, documentId));
                    if (origDocument != null) return Result(string.Format(CmsResource.DocumentAlreadyExist, documentId));
                }
            }
            // append cms properties
            var userName = GetUserName();
            var now = string.Format("{0:yyyy/MM/dd hh:mm:ss}", DateTime.Now);
            document.Merge(JObject.FromObject(
                new
                {
                    CreatedDate = now,
                    LastModified = now,
                    CreatedBy = userName,
                    ModifiedBy = userName
                }));

            collection.Insert(BsonSerializer.Deserialize<Document>(document.ToString()));
            return Result(CmsResource.DocumentSaved, true);
        }

        public JToken Update(string collectionId, JObject document)
        {
            var collection = _database.GetCollection<Document>(collectionId);
            if (collection == null) return Result(string.Format(CmsResource.CollectionNotExist, collectionId));
            // check existence of document
            var idProperty = document["_id"];
            if (idProperty != null)
            {
                var documentId = idProperty.Value<string>();
                if (string.IsNullOrWhiteSpace(documentId)) return Result(CmsResource.DocumentMissingId);
                var origDocument = collection.FindOne(Query<Document>.EQ(d => d.Id, documentId));
                if (origDocument == null) return Result(string.Format(CmsResource.DocumentNotExist, documentId));
            }
            else
            {
                return Result(CmsResource.DocumentMissingId);
            }
            // append cms properties
            document.Merge(JObject.FromObject(
                new
                {
                    LastModified = string.Format("{0:yyyy/MM/dd hh:mm:ss}", DateTime.Now),
                    ModifiedBy = GetUserName()
                }));

            collection.Save(BsonSerializer.Deserialize<Document>(document.ToString()));
            return Result(CmsResource.DocumentSaved, true);
        }

        public JToken Delete(string collectionId, string documentId)
        {
            var collection = _database.GetCollection<Document>(collectionId);
            if (collection == null) return Result(string.Format(CmsResource.CollectionNotExist, collectionId));
            // check existence of document
            var origDocument = collection.FindOne(Query<Document>.EQ(d => d.Id, documentId));
            if (origDocument == null) return Result(string.Format(CmsResource.DocumentNotExist, documentId));
            collection.Remove(Query<Document>.EQ(d => d.Id, documentId));
            return Result(CmsResource.DocumentDeleted, true);
        }

        public JToken SetParent(string collectionId, string documentId, string parentId)
        {
            var collection = _database.GetCollection<Document>(collectionId);
            if (collection == null) return Result(string.Format(CmsResource.CollectionNotExist, collectionId));
            // check existence of document
            var query = Query<Document>.EQ(d => d.Id, documentId);
            var origDocument = collection.FindOne(query);
            if (origDocument == null) return Result(string.Format(CmsResource.DocumentNotExist, documentId));

            // check existence of parent document
            if (!parentId.IsNullOrWhiteSpace())
            {
                var parentDocument = collection.FindOne(Query<Document>.EQ(d => d.Id, parentId));
                if (parentDocument == null) return Result(CmsResource.ParentNotExist);
            }
            // update document
            var update =
                Update<Document>.Set(d => d.ParentId, parentId)
                    .Set(d => d.LastModified, string.Format("{0:yyyy/MM/dd hh:mm:ss}", DateTime.Now))
                    .Set(d => d.ModifiedBy, GetUserName());
            collection.Update(query, update);
            return Result(CmsResource.DocumentSaved, true);
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