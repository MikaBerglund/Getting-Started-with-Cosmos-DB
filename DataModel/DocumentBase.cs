using Newtonsoft.Json;
using System;

namespace DataModel
{
    /// <summary>
    /// The base class for classes that are stored JSON documents in a Cosmos DB collection.
    /// </summary>
    public abstract class DocumentBase
    {

        [JsonProperty("id")]
        public string Id { get; set; }

        public virtual string Partition { get; set; }

    }
}
