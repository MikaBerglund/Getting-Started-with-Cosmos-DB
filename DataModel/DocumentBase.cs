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
            // Set the ID to a new guid. This way we have a unique ID
            // in case it is not explicitly set.
            this.Id = Guid.NewGuid().ToString();

            // Set the document type to match the name of the class.
            this.DocumentType = this.GetType().Name;

            // Set the partition by default to match the document type
            // to have some kind ofdistribution in case the partition
            // is not set in a derived class.
            this.Partition = this.DocumentType;
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
