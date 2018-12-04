# Part 3: Designing a Data Model

[<- Part 2](Part02-readme.md) | [Part 4 ->](Part04-readme.md)

Designing the data model for your application is perhaps the most time-consuming task you have to take care of before actually staring to build your application. Of course you don't have to complete the data model design, but there are certain things you must consider beforehand. This part focuses on those.

> The data model created in this part is just to demonstrate various aspects of data model design and how you can use class libraries to encapsulate your data model in.

## JSON Document Requirements
There are very few requirements set on a JSON document stored in Cosmos DB, as long as it is a valid JSON document. The following requirements also apply:

- **`id` attribute**: Every document **must** have an `id` attribute. No exceptions allowed. Please note that according to the [JSON PRC spec](https://jsonrpc.org/historical/json-rpc-1-1-alt.html#service-procedure-and-parameter-names) all names in a JSON document must be treated as case-sensitive, meaning that the attribute must be `id`, not `Id` or `ID`, but `id`. Keep this in mind. The `id` attribute is how documents are uniquely identified, just like a *primary key* in a SQL Server database.
- **Partition key**: Although not required for single-partition collections, I strongly recommend (see [part 2](Part02-readme.md)) that you only create multi-partition collections. For multi-partition collections, you must know the path to the attribute that is used as partition key. All documents that you store in the same collection should have this attribute. The value should vary as much as possible to spread out your documents across multiple partitions. If you store documents without the attribute specified as partition key, then those documents will be treated as they had an empty value for that attribute, and will all be stored in the same partition with an empty partition key.

## Classes and Serialization
When writing code that accesses a data store, I always try to access the data store in a typed fashion. Creating your data model as a class library allows your code to work with class instances and not JSON documents. Class libraries also allow you to encapsulate shared functionality in base classes and subclass them.

### The DataModel Class Library
For the data model, I've created a .NET Standard class library, the [`DataModel`](DataModel/DataModel.csproj) class library. This class library defines classes that represent the JSON documents I want to store in my collection.

#### The DocumentBase Base Class
The base class [`DocumentBase`](DataModel/DocumentBase.cs) takes care of the requirements described above that each document must meet. The class looks like this.

``` C#
// Comments removed for brevity. See the source file for
// detailed comments.
public abstract class DocumentBase
{
    protected DocumentBase()
    {
        this.Id = Guid.NewGuid().ToString();
        this.DocumentType = this.GetType().Name;
        this.Partition = this.DocumentType;
    }

    [JsonProperty("id")]
    public string Id { get; set; }

    public virtual string Partition { get; protected set; }

    public virtual string DocumentType { get; protected set; }

}
```

##### The Id Property
The `Id` property is decorated with the [`JsonProperty`](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonPropertyAttribute.htm) attribute and specifies that the value should be serialized with a lowercase name, to meet the requirement I described above. I could have named the property `id` and forget about the `JsonProperty` attribute, but following the convention for .NET class libraries, public members in classes begin with a capital letter.

##### The Partition Property
The `Partition` property is used in derived classes to provide a value that defines the partition the document will be stored in. By having a generic `Partition` property, and marking it as `virtual`, we allow derived classes to fully control the value the property returns, and use the class's other properties when defining the value for the `Partition` property. This follows the [synthetic partition key](https://docs.microsoft.com/en-us/azure/cosmos-db/synthetic-partition-keys) design pattern described in the Cosmos DB documentation. The base class takes care of providing the property with a value, but also allows derived classes to override the property and provide their own implementation when more control is needed.

##### The DocumentType Property
The `DocumentType` property will by default hold the name of the class. This allows you to query your collection for documents that contain the same kind of information, regardless of how they are partitioned. One benefit of this is that it is easier and more manageable to store different types of documents in the same collection, which in turn simplifies managing your application's storage, since you have less collections, maybe just one.

#### The Company Class
One example of how you could override the `Partition` property is the [`Company`](DataModel/Company.cs) class. It uses the `Country` and `City` properties to create the `Partition` property. You may need to adjust this logic to better suit the geographical distribution of the companies in your system.

##### The Partition Property
The `Partition` implementation looks like this.

``` C#
public override string Partition
{
    get => $"location:{this.Country}/{this.City}";
}

```

The `Partition` property is a read-only property, because it's value is created from other properties in the class. It will result in a geographical distribution of your companies, since companies in different cities will be stored in separate partitions.

##### The GlobalId Property
Since we will be storing `Company` entities in multiple partitions, the `Id` property will not uniquely identify the company inside of the collection, because the `id` attribute on a JSON document stored in Cosmos DB is only unique within a single partition.

That's why I added the `GlobalId` property which will be set to a `Guid` in the constructor of the class.

``` C#
// Code snippet from the Company class
public Company()
{
    this.GlobalId = Guid.NewGuid().ToString();
}

public string GlobalId { get; set; }

```

Since the `GlobalId` property uniquely identifies a `Company` entity, also across multiple partitions, you can use that in other documents to reference a `Company`. Remember that Cosmos DB does not specify a schema for the data, nor does it enforce referential integrity, so it is up to you and your data model, how you reference other entities. You don't have to reference the primary keys. You can reference whatever you find useful.

#### The Project Class
The [`Project`](DataModel/Project.cs) class demonstrates how you can create associations between different types of documents (entities). Each project refers to a company that the project is associated with through the `CompanyGlobalId` property. Please note that Cosmos DB has no mechanisms for enforcing referential integrity, so you have to take care of that in your
data access layer ("*DAL*").

##### The `CompanyGlobalId` Property
The first thing to note in the `Project` class is the property that references the company that the project is associated with.

``` C#
// Comments are removed for brevity.
private string _CompanyGlobalId;
public string CompanyGlobalId
{
    get => _CompanyGlobalId;
    set
    {
        _CompanyGlobalId = value;
        this.Partition = $"company:{value}";
    }
}
```

When the `CompanyGlobalId` property is set, we also set the `Partition` to a value derived from `CompanyGlobalId`. This way, all projects of one particular company will be stored in their own partition. So, the more companies we have, the more partitions we get. This strategy could be used for other entities that are associated with a single company. Then every company would have their own "store" that would contain their information.

Now, whether this is a good strategy for your solution, I cannot say. It depends on many factors that you have to take into consideration.

##### The `Company` Property
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
            this.CompanyGlobalId = value?.GlobalId;
        }
    }
```

The [`JsonIgnore`](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonIgnoreAttribute.htm) attribute is used leave that property out of JSON serialization. We don't want to store the entire `Company` object inside of the `Project` document when storing it in the database, since we already have the `Company` entity stored as a separate document. The reference using the `CompanyGlobalId` property is enough. You can then include the `Company` entity in your data access layer when querying for projects, if you want.

### Serialization
When you write your entity classes like this, you will have a lot of control in your code over how your entities are serialized and stored in your Cosmos DB database. You don't have to do anything for the serialization to happen. There are numerous attributes in the `Newtonsoft.Json` package that allow you to control how the JSON serialization will be done, so you might want to have a look at [the documentation](https://www.newtonsoft.com/json/help).

It is an open-source project, so you also might want to check out its [repository on Github](https://github.com/JamesNK/Newtonsoft.Json).

## Conclusion
While there is nothing that stops you from working with low-level types like the [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.documents.document) class to read and write all of your data in a Cosmos DB database, I strongly recommend that you create data model classes to provide your application with a more meaningful data structure. It might take a while to plan and create it, but I promise you that it will save a huge amount of time when developing your application.

## What's Next
In [part 4](Part04-readme.md) we'll have a closer look at creating a Cosmos DB database and a collection. If you want to learn more about creating data models for Cosmos DB, I suggest that you also have a look at the [Modeling document data for NoSQL databases](https://docs.microsoft.com/en-us/azure/cosmos-db/modeling-data) article.

[Go to Part 4 ->](Part04-readme.md)