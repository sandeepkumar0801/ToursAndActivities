using CacheManager.Constants;
using Microsoft.Azure.Documents.Client;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using System;
using System.Net;
using Util;

namespace CacheManager.Helper
{
    public sealed class IsangoDocumentClient
    {
        private static string mongoDatabaseConnection = ConfigurationManagerHelper.GetValuefromAppSettings("MongoDataBaseConnection");
        private static string mongoDatabaseName = ConfigurationManagerHelper.GetValuefromAppSettings("MongoDatabaseName");

        //Constructor
        public IsangoDocumentClient()
        {
        }

        private DocumentClient client;

        private IMongoClient mongoClient;

        private IMongoDatabase mongoDatabase;

        public DocumentClient Client => client ?? GetDocumentClient();

        public IMongoClient MongoClient => mongoClient ?? GetMongoClient();

        public IMongoDatabase MongoDatabase => mongoDatabase ?? GetMongoDatabase();

        public static IsangoDocumentClient Instance => Nested.Instance;

        /// <summary>
        /// This method gets Cosmos DB Document Client reference.
        /// </summary>
        /// <returns></returns>
        private DocumentClient GetDocumentClient()
        {
            var endpointUrl = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CosmosEndPointUrl);
            var primaryKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CosmosPrimaryKey);
            var connectionPolicy = new ConnectionPolicy
            {
                ConnectionMode = Microsoft.Azure.Documents.Client.ConnectionMode.Direct,
                ConnectionProtocol = Protocol.Tcp
            };
            var localCosmosEmulator = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.LocalCosmosEmulator);
            if (localCosmosEmulator == "true")
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                connectionPolicy.EnableEndpointDiscovery = false;
                connectionPolicy.ConnectionMode = Microsoft.Azure.Documents.Client.ConnectionMode.Gateway;
            }
            client = new DocumentClient(new Uri(endpointUrl), primaryKey, connectionPolicy);
            //Call OpenAsync to avoid startup latency on first request
            client.OpenAsync().GetAwaiter();
            return client;
        }

        private IMongoClient GetMongoClient()
        {
            //var MongoConnectionString = "mongodb+srv://IsangoAPI:Isango123@isango-shared.rdfod.mongodb.net/bumblebee_shared?retryWrites=true&w=majority";
            var MongoConnectionString = mongoDatabaseConnection;
            try
            {
                mongoClient = new MongoClient(MongoConnectionString);
                var myConventions = new ConventionPack();
                myConventions.Add(new IgnoreExtraElementsConvention(true));
                ConventionRegistry.Register("IgnoreExtraElements", myConventions, x => true);

                ConventionRegistry.Register(
                            "DictionaryRepresentationConvention",
                            new ConventionPack { new DictionaryRepresentationConvention(DictionaryRepresentation.ArrayOfArrays) },
                            _ => true
                            );
            }
            catch(Exception ex)
            {
                //logging
            }
            return mongoClient;
        }

        private IMongoDatabase GetMongoDatabase()
        {
            var MongoDatabseString = mongoDatabaseName;
            try
            {
                mongoDatabase = MongoClient.GetDatabase(MongoDatabseString);
            }
            catch(Exception ex)
            {
                //logging
            }
            return mongoDatabase;
        }

        private class Nested
        {
            static Nested()
            {
            }

            internal static readonly IsangoDocumentClient Instance = new IsangoDocumentClient();
        }
    }
}