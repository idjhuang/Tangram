using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AspNet.Identity.MongoDB;
using MongoDB.Driver;

namespace TangramCMS
{
    public class ApplicationIdentityContext : IdentityContext, IDisposable
    {
        public ApplicationIdentityContext(MongoCollection users, MongoCollection roles)
            : base(users, roles)
        {
        }

        public static ApplicationIdentityContext Create()
        {
            var connectionStr = Properties.Settings.Default.CmsDbConnectionString;
            var databaseName = Properties.Settings.Default.CmsDatabase;
            var client = new MongoClient(connectionStr);
            var database = client.GetServer().GetDatabase(databaseName);
            var users = database.GetCollection<IdentityUser>("cms_users");
            var roles = database.GetCollection<IdentityRole>("cms_roles");
            return new ApplicationIdentityContext(users, roles);
        }

        public void Dispose()
        {
        }
    }
}