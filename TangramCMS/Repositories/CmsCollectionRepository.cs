using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json.Linq;
using TangramCMS.App_LocalResources;
using TangramCMS.Controllers;
using TangramCMS.Infrastructure;
using TangramCMS.Models;

namespace TangramCMS.Repositories
{
    public class CmsCollectionRepository : ICmsCollectionRepository
    {
        private MongoDatabase _database;

        public CmsCollectionRepository(ICmsService service)
        {
            _database = service.Database;
            var convensionPak = new ConventionPack();
            //convensionPak.Add(new StringObjectIdIdGeneratorConvention());
            //ConventionRegistry.Register("CMS", convensionPak, t => t.Name.Equals("BsonDocument") || t.Name.Equals("CmsDocument"));
        }
        public JToken GetAll(IOwinContext context, string collectionId)
        {
            try
            {
                var collection = _database.GetCollection<CmsDocument>(collectionId);
                if (collection == null) return Result(string.Format(CmsResource.CollectionNotExist, collectionId));
                var jsonStr = collection.FindAll().ToJson().ConvertObjectId().ConvertIsoDate();
                return JToken.Parse(jsonStr);
            }
            catch (Exception e)
            {
                return Result(e.Message);
            }
        }

        public JToken GetById(IOwinContext context, string collectionId, string documentId)
        {
            try
            {
                var collection = _database.GetCollection<CmsDocument>(collectionId);
                if (collection == null) return Result(string.Format(CmsResource.CollectionNotExist, collectionId));
                var document = collection.FindOne(Query<CmsDocument>.EQ(d => d.Id, documentId));
                if (document == null) return Result(string.Format(CmsResource.DocumentNotExist, documentId));
                var jsonStr = document.ToJson().ConvertObjectId().ConvertIsoDate();
                return JToken.Parse(jsonStr);
            }
            catch (Exception e)
            {
                return Result(e.Message);
            }
        }

        public JToken GetParent(IOwinContext context, string collectionId, string documentId)
        {
            try
            {
                var collection = _database.GetCollection<CmsDocument>(collectionId);
                if (collection == null) return Result(string.Format(CmsResource.CollectionNotExist, collectionId));
                var document = collection.FindOne(Query<CmsDocument>.EQ(d => d.Id, documentId));
                if (document == null) return Result(string.Format(CmsResource.DocumentNotExist, documentId));
                //var collection1 = _database.GetCollection<BsonDocument>(collectionId);
                //var parent = collection1.FindOne(Query<CmsDocument>.EQ(d => d.Id, document.ParentId));
                var parent = collection.FindOne(Query<CmsDocument>.EQ(d => d.Id, document.ParentId));
                var jsonStr = (parent == null) ? "{}" : parent.ToJson().ConvertObjectId().ConvertIsoDate();
                return JToken.Parse(jsonStr);
            }
            catch (Exception e)
            {
                return Result(e.Message);
            }
        }

        public JToken GetChildren(IOwinContext context, string collectionId, string documentId)
        {
            try
            {
                var collection = _database.GetCollection<CmsDocument>(collectionId);
                if (collection == null) return Result(string.Format(CmsResource.CollectionNotExist, collectionId));
                var document = collection.FindOne(Query<CmsDocument>.EQ(d => d.Id, documentId));
                if (document == null) return Result(string.Format(CmsResource.DocumentNotExist, documentId));
                //var collection1 = _database.GetCollection<BsonDocument>(collectionId);
                //var children = collection1.Find(Query<CmsDocument>.EQ(d => d.ParentId, document.Id));
                var children = collection.Find(Query<CmsDocument>.EQ(d => d.ParentId, document.Id));
                var jsonStr = children.ToJson().ConvertObjectId().ConvertIsoDate();
                return JToken.Parse(jsonStr);
            }
            catch (Exception e)
            {
                return Result(e.Message);
            }
        }

        public JToken Insert(IOwinContext context, string collectionId, JObject document)
        {
            try
            {
                var collection = _database.GetCollection<CmsDocument>(collectionId);
                if (collection == null) return Result(string.Format(CmsResource.CollectionNotExist, collectionId));
                // check existence of document
                var idProperty = document["_id"];
                if (idProperty != null)
                {
                    var documentId = idProperty.Value<string>();
                    if (!string.IsNullOrWhiteSpace(documentId))
                    {
                        var origDocument = collection.FindOne(Query<CmsDocument>.EQ(d => d.Id, documentId));
                        if (origDocument != null) return Result(string.Format(CmsResource.DocumentAlreadyExist, documentId));
                    }
                }
                // append cms properties
                var now = DateTime.Now;
                var userId = context.Authentication.User.Identity.GetUserId();
                document.Merge(JObject.FromObject(
                    new
                    {
                        CreatedDate = string.Format("{0:yyyy/MM/dd hh:mm:ss}", now),
                        LastModified = string.Format("{0:yyyy/MM/dd hh:mm:ss}", now),
                        CreatedBy = userId,
                        ModifiedBy = userId
                    }));

                collection.Insert(BsonSerializer.Deserialize<CmsDocument>(document.ToString()));
                return Result(CmsResource.DocumentSaved, true);
            }
            catch (Exception e)
            {
                return Result(e.Message);
            }
        }

        public JToken Update(IOwinContext context, string collectionId, JObject document)
        {
            try
            {
                var collection = _database.GetCollection<CmsDocument>(collectionId);
                if (collection == null) return Result(string.Format(CmsResource.CollectionNotExist, collectionId));
                // check existence of document
                var idProperty = document["_id"];
                if (idProperty != null)
                {
                    var documentId = idProperty.Value<string>();
                    if (string.IsNullOrWhiteSpace(documentId)) return Result(CmsResource.DocumentMissingId);
                    var origDocument = collection.FindOne(Query<CmsDocument>.EQ(d => d.Id, documentId));
                    if (origDocument == null) return Result(string.Format(CmsResource.DocumentNotExist, documentId));                    
                }
                // append cms properties
                var now = DateTime.Now;
                var userId = context.Authentication.User.Identity.GetUserId();
                document.Merge(JObject.FromObject(
                    new
                    {
                        LastModified = string.Format("{0:yyyy/MM/dd hh:mm:ss}", now),
                        ModifiedBy = userId
                    }));

                collection.Save(BsonSerializer.Deserialize<CmsDocument>(document.ToString()));
                return Result(CmsResource.DocumentSaved, true);
            }
            catch (Exception e)
            {
                return Result(e.Message);
            }
        }

        public JToken Delete(IOwinContext context, string collectionId, string documentId)
        {
            try
            {
                var collection = _database.GetCollection<CmsDocument>(collectionId);
                if (collection == null) return Result(string.Format(CmsResource.CollectionNotExist, collectionId));
                // check existence of document
                var origDocument = collection.FindOne(Query<CmsDocument>.EQ(d => d.Id, documentId));
                if (origDocument == null) return Result(string.Format(CmsResource.DocumentNotExist, documentId));
                collection.Remove(Query<CmsDocument>.EQ(d => d.Id, documentId));
                return Result(CmsResource.DocumentDeleted, true);
            }
            catch (Exception e)
            {
                return Result(e.Message);
            }
        }

        public JToken SetParent(IOwinContext context, string collectionId, string documentId, string parentId)
        {
            try
            {
                var collection = _database.GetCollection<CmsDocument>(collectionId);
                if (collection == null) return Result(string.Format(CmsResource.CollectionNotExist, collectionId));
                // check existence of document
                var query = Query<CmsDocument>.EQ(d => d.Id, documentId);
                var origDocument = collection.FindOne(query);
                if (origDocument == null) return Result(string.Format(CmsResource.DocumentNotExist, documentId));

                // check existence of parent document
                if (!parentId.IsNullOrWhiteSpace())
                {
                    var parentDocument = collection.FindOne(Query<CmsDocument>.EQ(d => d.Id, parentId));
                    if (parentDocument == null) return Result(CmsResource.ParentNotExist);                    
                }
                // update document
                var now = DateTime.Now;
                var userId = context.Authentication.User.Identity.GetUserId();
                var update =
                    Update<CmsDocument>.Set(d => d.ParentId, parentId)
                        .Set(d => d.LastModified, string.Format("{0:yyyy/MM/dd hh:mm:ss}", now))
                        .Set(d => d.ModifiedBy, userId);
                collection.Update(query, update);
                return Result(CmsResource.DocumentSaved, true);
            }
            catch (Exception e)
            {
                return Result(e.Message);
            }
        }

        public void Test(string collectionId)
        {
            var collection = _database.GetCollection<CmsDocument>(collectionId);
            var cursor = collection.FindAll();
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