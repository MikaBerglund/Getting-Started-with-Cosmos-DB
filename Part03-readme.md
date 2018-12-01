# Part 3: Designing a Data Model

[<- Part 2](Part02-readme.md) | [Part 4 ->](Part04-readme.md)

Designing the data model for your application is perhaps the most time-consuming task you have to take care of before actually staring to build your application. You don't have to completely design your data model, but there are certain things you must consider beforehand. This part focuses on those.

## JSON Document Requirements
As I wrote in [Part 2](Part02-readme.md), there are very few requirements set on a JSON document stored in Cosmos DB, as long as it is a valid JSON document. The following requirements also apply:

- **`id` attribute**: Every document **must** have an `id` attribute. No exceptions allowed. Please note that according to the [JSON PRC spec](https://jsonrpc.org/historical/json-rpc-1-1-alt.html#service-procedure-and-parameter-names) all names in a JSON document must be treated as case-sensitive, meaning that the attribute must be `id`, not `Id` or `ID`, but `id`. Keep this in mind.
- **Partition key**: Although not required for single-partition collections, I strongly recommend (see [scaling considerations](Part02-scaling.md)) that you only create multi-partition collections. For multi-partition collections, you must know the path to the attribute that is used as partition key. All documents that you store in the same collection must have this attribute. The value however should vary as much as possible to spread out your documents across multiple partitions. If you store documents without the attribute specified as partition key, then those documents will all be stored in the same partition with an empty partition key.

## Classes and Serialization
When writing code that accesses a data store, I always try to access the data store in a typed fashion. Creating your data model as a class library allows your code to work with class instances and not JSON documents. Class libraries also allow you to encapsulate shared functionality in base classes and subclass them.

### The DataModel Class Library
For the data model, I've created a .NET Standard class library, the [`DataModel`](DataModel/DataModel.csproj) class library. This class library defines classes that represent the JSON documents I want to store in my collection.

The base class [`DocumentBase`](DataModel/DocumentBase.cs) takes care of the requirements described above that each document must meet. The class looks like this.

``` C#
/// <summary>
/// The base class for classes that are stored JSON documents
/// in a Cosmos DB collection.
/// </summary>
public abstract class DocumentBase
{
    protected DocumentBase()
    {
        this.Id = Guid.NewGuid().ToString();
        this.DocumentType = this.GetType().Name;
    }

    /// <summary>
    /// The unique ID of the document.
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Use in Cosmos DB as partition key to distribute documents
    /// across multiple partitions.
    /// </summary>
    public virtual string Partition { get; protected set; }

    /// <summary>
    /// The class name of the document. Enables you to look for
    /// particular types of documents.
    /// Defaults to the name of the class, but you can override
    /// the property to set the type to something else.
    /// </summary>
    public virtual string DocumentType { get; protected set; }

}
```

The `Id` property is decorated with the [`JsonProperty`](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonPropertyAttribute.htm) attribute and specifies that the value should be serialized with a lowercase name, to meet the requirement I described above. I could have named the property `id` and forget about the `JsonProperty` attribute, but following the convention for .NET class libraries, public members in classes begin with a capital letter.

The `Partition` property is used in derived classes to provide a value that defines the partition the document will be stored in. By having a generic `Partition` property, and marking it as `virtual`, we allow derived classes to fully control the value the property returns, and use the class's other properties in the value. This is more or less the same thing as a [synthetic partition key](https://docs.microsoft.com/en-us/azure/cosmos-db/synthetic-partition-keys).

The `DocumentType` property will by default hold the name of the class. This allows you to query your collection for documents that contain the same kind of information, regardless of how they are partitioned. This allows you store all of your documents in the same collection, which in turn makes it easier to manager the application storage, since you only have one container, the *collection*.

### The Company Class
One example of how you could override the `Partition` property is the [`Company`](DataModel/Company.cs) class. It uses the `Country` and `City` properties to create the `Partition` property. You may need to adjust this logic to better suit the geographical distribution of the companies in your system.

The `Partition` implementation looks like this.

``` C#
public override string Partition
{
    get => $"location:{this.Country}/{this.City}";
}

```

The `Partition` property is a read-only property, because it's value is created from other properties in the class.

### The Project Class
The [`Project`](DataModel/Project.cs) class demonstrates how you can create associations between different types of documents (entities). Each project refers to a company that the project is associated with through the `CompanyId` property. Please note that Cosmos DB has no mechanisms for enforcing referential integrity, so you have to take care of that in your
data access layer.

#### The `CompanyId` Property
The first thing to note in the `Project` class is the property that references the company that the project is associated with.

``` C#
// Comments are removed for brevity.
private string _CompanyId;
public string CompanyId
{
    get => _CompanyId;
    set
    {
        _CompanyId = value;
        this.Partition = value;
    }
}
```

When the `CompanyId` property is set, we also set the `Partition` to the same value. This way, all projects of one particular company will be stored in their own partition. So, the more companies we have, the more partitions we get. This strategy could be used for other entities that are associated with a single company. Then every company would have their own "store" that would contain their information.

Now, whether this is a good strategy for your solution, I cannot say. It depends on many factors that you have to take into consideration.

#### The `Company` Property
The second property I'd like to dig into is the `Company` property.

``` C#
    private Company _Company;
    [JsonIgnore]
    public Company Company
    {
        get => _Company;
        set
        {
            _Company = value;
            this.CompanyId = this.Company?.Id;
        }
    }
```

The [`JsonIgnore`](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonIgnoreAttribute.htm) attribute is used leave that property out of JSON serialization. We don't want to store the entire `Company` object inside of the `Project` document when storing it in the database, since we already have the `Company` entity stored as a separate document. The reference using the `CompanyId` property is enough. You can then include the `Company` entity in your data access layer when querying for projects.

### Serialization
When you write your entity classes like this, you will have a lot of control in your code over how your entities are serialized and stored in your Cosmos DB database. You don't have to anything for the serialization to happen. However, if you want to serialize your objects to JSON strings, you can easily do it with the [`JsonConvert.SerializeObject`](https://www.newtonsoft.com/json/help/html/Overload_Newtonsoft_Json_JsonConvert_SerializeObject.htm) method, which is part of the very popular [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) library. Check out [this sample](https://www.newtonsoft.com/json/help/html/SerializeObject.htm) for details.

## Conclusion
While there is nothing that stops you from working with low-level types like the [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.documents.document) class to read and write all of your data in a Cosmos DB database, I strongly recommend that you create data model classes to provide your application with a more meaningful data structure. It might take a while to plan and create it, but I promise you that it will save a huge amount of time when developing your application.