using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;

namespace TangramCMS.Repositories
{
    public interface ICmsService
    {
        MongoDatabase Database { get; }
    }

    public class CmsService : ICmsService
    {
        public CmsService()
        {
            var connectionStr = Properties.Settings.Default.CmsDbConnectionString;
            var databaseName = Properties.Settings.Default.CmsDatabase;
            var client = new MongoClient(connectionStr);
            var server = client.GetServer();
            Database = server.GetDatabase(databaseName);
        }
        public MongoDatabase Database { get; private set; }
    }
}