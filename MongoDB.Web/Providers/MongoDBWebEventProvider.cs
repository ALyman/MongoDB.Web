﻿using System.Collections.Specialized;
using System.Linq;
using System.Web.Management;
using MongoDB.Driver;
using MongoDB.Web.Internal;

namespace MongoDB.Web.Providers
{
    public class MongoDBWebEventProvider : BufferedWebEventProvider
    {
        private MongoCollection mongoCollection;
        private readonly IMongoConnectionProvider provider;

        public MongoDBWebEventProvider() : this(new MongoConnectionProvider()) { }

        public MongoDBWebEventProvider(IMongoConnectionProvider provider)
        {
            this.provider = provider;
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            this.mongoCollection = provider.GetCollection(
                connectionString: config["connectionString"] ?? "mongodb://localhost",
                database: config["database"] ?? "ASPNETDB",
                collection: config["collection"] ?? "WebEvent"); 
            base.Initialize(name, config);
        }

        public override void ProcessEventFlush(WebEventBufferFlushInfo flushInfo)
        {
            this.mongoCollection.InsertBatch<WebEvent>(flushInfo.Events.Cast<WebBaseEvent>().ToList().ConvertAll<WebEvent>(WebEvent.FromWebBaseEvent));
        }
    }
}
