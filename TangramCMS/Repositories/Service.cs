using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;

namespace TangramService.Repositories
{
    public interface IService
    {
        MongoDatabase Database { get; }
    }

    public class Service : IService
    {
        public Service()
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