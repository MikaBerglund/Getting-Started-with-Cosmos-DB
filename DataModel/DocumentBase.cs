using Newtonsoft.Json;
using System;

namespace DataModel
{
    /// <summary>
    /// The base class for classes that are stored JSON documents in a Cosmos DB collection.
    /// </summary>
    public abstract class DocumentBase
    {
        protected DocumentBase()
        {
            this.DocumentType = this.GetType().Name;
        }

        /// <summary>
        /// The unique ID of the document.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Use in Cosmos DB as partition key to distribute documents across multiple partitions.
        /// </summary>
        public virtual string Partition { get; protected set; }

        /// <summary>
        /// The class name of the document. Enables you to look for particular types of documents.
        /// Defaults to the name of the class, but you can override the property to set the type
        /// to something else.
        /// </summary>
        public virtual string DocumentType { get; protected set; }

    }
}
