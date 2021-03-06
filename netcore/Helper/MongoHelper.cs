﻿using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Arthur_Clive.Helper
{
    /// <summary>Helper method for MongoDB</summary>
    public class MongoHelper
    {
        /// <summary></summary>
        public static IMongoDatabase _mongodb;

        /// <summary>Client for MongoDB</summary>
        public static MongoClient _client = GetClient();

        /// <summary>Get client for MongoDB</summary>
        public static MongoClient GetClient()
        {
            var ip = GlobalHelper.ReadXML().Elements("mongo").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("ip").First().Value;
            var user = GlobalHelper.ReadXML().Elements("mongo").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("user").First().Value;
            var password = GlobalHelper.ReadXML().Elements("mongo").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("password").First().Value;
            var db = GlobalHelper.ReadXML().Elements("mongo").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("db").First().Value;
            var connectionString = "mongodb://"+user+":"+password+"@"+ip+":27017/"+ db;
            var mongoClient = new MongoClient(connectionString);
            return mongoClient;
        }

        /// <summary>Get single object from MongoDB</summary>
        /// <param name="filter"></param>
        /// <param name="dbName"></param>
        /// <param name="collectionName"></param>
        public static async Task<BsonDocument> GetSingleObject(FilterDefinition<BsonDocument> filter, string dbName, string collectionName)
        {
            _mongodb = _client.GetDatabase(dbName);
            var collection = _mongodb.GetCollection<BsonDocument>(collectionName);
            IAsyncCursor<BsonDocument> cursor = await collection.FindAsync(filter);
            return cursor.FirstOrDefault();
        }
        
        /// <summary>Update single object in MongoDB </summary>
        /// <param name="filter"></param>
        /// <param name="dbName"></param>
        /// <param name="collectionName"></param>
        /// <param name="update"></param>
        public static async Task<bool> UpdateSingleObject(FilterDefinition<BsonDocument> filter, string dbName, string collectionName, UpdateDefinition<BsonDocument> update)
        {
            _mongodb = _client.GetDatabase(dbName);
            var collection = _mongodb.GetCollection<BsonDocument>(collectionName);
            var cursor = await collection.UpdateOneAsync(filter, update);
            return cursor.ModifiedCount > 0;
        }

        /// <summary>Delete single object from MongoDB</summary>
        /// <param name="filter"></param>
        /// <param name="dbName"></param>
        /// <param name="collectionName"></param>
        public static bool DeleteSingleObject(FilterDefinition<BsonDocument> filter, string dbName, string collectionName)
        {
            var data = GetSingleObject(filter, dbName, collectionName).Result;
            var collection = _mongodb.GetCollection<BsonDocument>(collectionName);
            var response = collection.DeleteOneAsync(data);
            return response.Result.DeletedCount > 0;
        }

        /// <summary>Check if a data is present in MongoDB</summary>
        /// <param name="filterField1"></param>
        /// <param name="filterData1"></param>
        /// <param name="filterField2"></param>
        /// <param name="filterData2"></param>
        /// <param name="dbName"></param>
        /// <param name="collectionName"></param>
        public static BsonDocument CheckForDatas(string filterField1, dynamic filterData1, string filterField2, dynamic filterData2, string dbName, string collectionName)
        {
            FilterDefinition<BsonDocument> filter;
            if (filterField2 == null)
            {
                filter = Builders<BsonDocument>.Filter.Eq(filterField1, filterData1);
            }
            else
            {
                filter = Builders<BsonDocument>.Filter.Eq(filterField1, filterData1) & Builders<BsonDocument>.Filter.Eq(filterField2, filterData2);
            }
            return GetSingleObject(filter, dbName, collectionName).Result;
        }
    }
}
