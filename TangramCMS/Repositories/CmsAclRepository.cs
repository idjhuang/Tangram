using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AspNet.Identity.MongoDB;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using TangramCMS.App_LocalResources;
using TangramCMS.Models;

namespace TangramCMS.Repositories
{
    public interface ICmsAclRepository
    {
        IEnumerable<CmsAclModel> GetByCollection(string collectionId);
        IEnumerable<CmsAclModel> GetByRole(string roleName);
        IEnumerable<CmsCollection> GetAvailableCmsCollections(List<string> roleList);
        CmsResultModel Create(CmsAclModel acl);
        CmsResultModel Update(CmsAclModel acl);
        CmsResultModel Delete(string collectionId, string roleName);
        CmsResultModel DeleteByCmsCollection(string collectionId);
        CmsResultModel DeleteByRole(string roleName);
        bool CheckRight(string collectionId, List<string> roleList, string right);
    }

    public class CmsAclRepository : ICmsAclRepository
    {
        private readonly MongoCollection<CmsAclModel> _cmsAcls;
        private readonly MongoCollection<CmsCollection> _cmsCollections; 
        private readonly MongoCollection<IdentityRole> _roles;

        public CmsAclRepository(ICmsService service)
        {
            _cmsAcls = service.Database.GetCollection<CmsAclModel>("cms_acl");
            _cmsCollections = service.Database.GetCollection<CmsCollection>("cms_collection");
            _roles = service.Database.GetCollection<IdentityRole>("cms_roles");
        }

        public IEnumerable<CmsAclModel> GetByCollection(string collectionId)
        {
            var cursor = _cmsAcls.Find(Query<CmsAclModel>.EQ(d => d.CollectionId, collectionId));
            return cursor.ToList();
        }

        public IEnumerable<CmsAclModel> GetByRole(string roleName)
        {
            var cursor = _cmsAcls.Find(Query<CmsAclModel>.EQ(d => d.RoleName, roleName));
            return cursor.ToList();            
        }

        public IEnumerable<CmsCollection> GetAvailableCmsCollections(List<string> roleList)
        {
            var collectionNames = new List<string>();
            foreach (var role in roleList)
            {
                collectionNames.AddRange(
                    _cmsAcls.AsQueryable()
                        .Where(d => d.RoleName.Equals(role) && d.Rights.Contains("w"))
                        .Select(d => d.CollectionId));
            }
            collectionNames = collectionNames.Distinct().ToList();
            return _cmsCollections.AsQueryable().Where(c => collectionNames.Contains(c.CollectionId));
        }

        public CmsResultModel Create(CmsAclModel acl)
        {
            // check existence of collectionId of acl
            if (_cmsCollections.Count(Query<CmsCollection>.EQ(c => c.CollectionId, acl.CollectionId)) == 0)
                return new CmsResultModel
                {
                    IsSuccess = false,
                    Message = string.Format(CmsResource.CollectionNotExist, acl.CollectionId)
                };
            // check existence of role of acl
            if (_roles.Count(Query<IdentityRole>.EQ(r => r.Name, acl.RoleName)) == 0)
                return new CmsResultModel
                {
                    IsSuccess = false,
                    Message = string.Format(CmsResource.RoleNotExist, acl.RoleName)
                };
            // check for existence of acl
            var query =
                _cmsAcls.AsQueryable()
                    .Where(d => d.CollectionId.Equals(acl.CollectionId) && d.RoleName.Equals(acl.RoleName));
            if (query.Any())
            {
                return new CmsResultModel { IsSuccess = false, Message = CmsResource.AclExist };
            }

            var userName = GetUserName();
            var now = string.Format("{0:yyyy/MM/dd hh:mm:ss}", DateTime.Now);
            acl.CreatedBy = userName;
            acl.ModifiedBy = userName;
            acl.CreatedDate = now;
            acl.LastModified = now;
            _cmsAcls.Insert(acl);
            return new CmsResultModel { IsSuccess = true, Message = CmsResource.AclSaved };
        }

        public CmsResultModel Update(CmsAclModel acl)
        {
            // check for existence of acl
            var query =
                _cmsAcls.AsQueryable()
                    .Where(d => d.CollectionId.Equals(acl.CollectionId) && d.RoleName.Equals(acl.RoleName));
            if (!query.Any())
            {
                return new CmsResultModel { IsSuccess = false, Message = CmsResource.AclNotExist };
            }

            acl.ModifiedBy = GetUserName();
            acl.LastModified = string.Format("{0:yyyy/MM/dd hh:mm:ss}", DateTime.Now);
            _cmsAcls.Save(acl);
            return new CmsResultModel { IsSuccess = true, Message = CmsResource.AclSaved };
        }

        public CmsResultModel Delete(string collectionId, string roleName)
        {
            // check for existence of acl
            var query =
                _cmsAcls.AsQueryable()
                    .Where(d => d.CollectionId.Equals(collectionId) && d.RoleName.Equals(roleName));
            if (!query.Any())
            {
                return new CmsResultModel { IsSuccess = false, Message = CmsResource.AclNotExist };
            }
            _cmsAcls.Remove(Query<CmsAclModel>.Where(d => d.CollectionId.Equals(collectionId) && d.RoleName.Equals(roleName)));
            return new CmsResultModel { IsSuccess = true, Message = CmsResource.AclDeleted };
        }

        public CmsResultModel DeleteByCmsCollection(string collectionId)
        {
            // check existence of cms collection
            if (_cmsCollections.Count(Query<CmsCollection>.EQ(c => c.CollectionId, collectionId)) == 0)
                return new CmsResultModel
                {
                    IsSuccess = false,
                    Message = string.Format(CmsResource.CollectionNotExist, collectionId)
                };
            // delete acls of deleted cms collection
            _cmsAcls.Remove(Query<CmsAclModel>.EQ(d => d.CollectionId, collectionId));
            return new CmsResultModel { IsSuccess = true, Message = CmsResource.AclDeleted };
        }

        public CmsResultModel DeleteByRole(string roleName)
        {
            // check existence of role
            if (_roles.Count(Query<IdentityRole>.EQ(r => r.Name, roleName)) == 0)
                return new CmsResultModel
                {
                    IsSuccess = false,
                    Message = string.Format(CmsResource.RoleNotExist, roleName)
                };
            // delete acls of deleted role
            _cmsAcls.Remove(Query<CmsAclModel>.EQ(d => d.RoleName, roleName));
            return new CmsResultModel { IsSuccess = true, Message = CmsResource.AclDeleted };
        }

        public bool CheckRight(string collectionId, List<string> roleList, string right)
        {
            var query =
                _cmsAcls.AsQueryable()
                    .Where(
                        d =>
                            d.CollectionId.Equals(collectionId) && roleList.Contains(d.RoleName) &&
                            d.Rights.Contains(right));
            return query.Any();
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