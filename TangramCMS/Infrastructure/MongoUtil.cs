using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Wrappers;

namespace TangramCMS.Infrastructure
{
    public static class MongoUtil
    {
        public static List<T> Query<T>(this MongoCollection collection,
            string queryString, string orderString, int? limit = null, int? skip = null) where T : class
        {
            if (string.IsNullOrWhiteSpace(queryString)) queryString = "{}";
            if (string.IsNullOrWhiteSpace(orderString)) orderString = "{}";
            var queryDoc = BsonSerializer.Deserialize<BsonDocument>(queryString);
            var orderDoc = BsonSerializer.Deserialize<BsonDocument>(orderString);

            var query = new QueryWrapper(queryDoc);
            var order = new SortByWrapper(orderDoc);

            var cursor = collection.FindAs<T>(query);
            cursor.SetSortOrder(order);
            if (limit != null) cursor.SetLimit((int)limit);
            if (skip != null) cursor.SetSkip((int)skip);

            return cursor.ToList();
        }
    }
}