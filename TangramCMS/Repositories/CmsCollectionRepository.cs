using System;
using System.Collections.Generic;
using System.Web;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using TangramCMS.App_LocalResources;
using TangramCMS.Models;

namespace TangramCMS.Repositories
{
    public interface ICmsCollectionRepository
    {
        IEnumerable<CmsCollection> GatAll(string documentType = null);
        CmsCollection Get(string collectionId);
        CmsResultModel Create(CmsCollection cmsCollection);
        CmsResultModel Delete(string collectionId);
    }

    public class CmsCollectionRepository : ICmsCollectionRepository
    {
        private readonly MongoCollection<CmsCollection> _cmsCollections;
        private readonly MongoCollection<CmsDocumentType> _cmsDocumentTypes;
        private readonly ICmsAclRepository _aclRepository;

        public CmsCollectionRepository(ICmsService service, ICmsAclRepository aclRepository)
        {
            _cmsCollections = service.Database.GetCollection<CmsCollection>("cms_collection");
            _cmsDocumentTypes = service.Database.GetCollection<CmsDocumentType>("cms_document_type");
            _aclRepository = aclRepository;
        }

        public IEnumerable<CmsCollection> GatAll(string documentType = null)
        {
            return string.IsNullOrWhiteSpace(documentType)
                ? _cmsCollections.FindAll()
                : _cmsCollections.Find(Query<CmsCollection>.EQ(c => c.DocumentType, documentType));
        }

        public CmsCollection Get(string collectionId)
        {
            return _cmsCollections.FindOne(Query<CmsCollection>.EQ(c => c.CollectionId, collectionId));
        }

        public CmsResultModel Create(CmsCollection cmsCollection)
        {
            // check existence of collection
            if (_cmsCollections.Count(Query<CmsCollection>.EQ(c => c.CollectionId, cmsCollection.CollectionId)) != 0)
                return new CmsResultModel
                {
                    IsSuccess = false,
                    Message = string.Format(CmsResource.CollectionAlreadyExist, cmsCollection.CollectionId)
                };
            // check existence of document type
            if (_cmsDocumentTypes.Count(Query<CmsDocumentType>.EQ(t => t.DocumentType, cmsCollection.DocumentType)) == 0)
                return new CmsResultModel
                {
                    IsSuccess = false,
                    Message = string.Format(CmsResource.DocumentTypeNotExist, cmsCollection.DocumentType)
                };

            _cmsCollections.Database.CreateCollection(cmsCollection.CollectionId);
            var now = string.Format("{0:yyyy/MM/dd hh:mm:ss}", DateTime.Now);
            var userName = GetUserName();
            cmsCollection.CreatedDate = now;
            cmsCollection.LastModified = now;
            cmsCollection.CreatedBy = userName;
            cmsCollection.ModifiedBy = userName;
            _cmsCollections.Insert(cmsCollection);
            return new CmsResultModel
            {
                IsSuccess = true,
                Message = string.Format(CmsResource.CollectionCreated, cmsCollection.CollectionId)
            };
        }

        public CmsResultModel Delete(string collectionId)
        {
            // check existence of collection
            if (_cmsCollections.Count(Query<CmsCollection>.EQ(c => c.CollectionId, collectionId)) == 0)
                return new CmsResultModel
                {
                    IsSuccess = false,
                    Message = string.Format(CmsResource.CollectionNotExist, collectionId)
                };

            var result = _aclRepository.DeleteByCmsCollection(collectionId);
            if (!result.IsSuccess) return result;
            _cmsCollections.Remove(Query<CmsCollection>.EQ(c => c.CollectionId, collectionId));
            return new CmsResultModel
            {
                IsSuccess = true,
                Message = string.Format(CmsResource.CollectionDeleted, collectionId)
            };
        }

        private string GetUserName()
        {
            var request = HttpContext.Current.Request;
            var identity = request.GetOwinContext().Authentication.User.Identity;
            return identity.IsAuthenticated
                ? identity.Name
                : string.Format(CmsResource.AnonymousName, request.UserHostAddress);
        }
    }
}