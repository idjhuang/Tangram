using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json.Linq;
using TangramCMS.Models;

namespace TangramCMS.Repositories
{
    public interface ICmsCollectionRepository
    {
        JToken GetAll(IOwinContext context, string collectionId);
        JToken GetById(IOwinContext context, string collectionId, string documentId);
        JToken GetParent(IOwinContext context, string collectionId, string documentId);
        JToken GetChildren(IOwinContext context, string collectionId, string documentId);
        JToken Insert(IOwinContext context, string collectionId, JObject document);
        JToken Update(IOwinContext context, string collectionId, JObject document);
        JToken Delete(IOwinContext context, string collectionId, string documentId);
        JToken SetParent(IOwinContext context, string collectionId, string documentId, string parentId);
        void Test(string collectionId);
    }
}
