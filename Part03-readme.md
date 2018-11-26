# Part 3: Designing a Data Model

[<- Part 2](Part02-readme.md)

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
public abstract class DocumentBase
{
    [JsonProperty("id")]
    public string Id { get; set; }

    public virtual string Partition { get; set; }
}
```

The `Id` property is decorated with the [`JsonProperty`](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonPropertyAttribute.htm) attribute and specifies that the value should be serialized with a lowercase name, to meet the requirement I described above. I could have named the property `id` and forget about the `JsonProperty` attribute, but following the convention for .NET class libraries, public members in classes begin with a capital letter.

The `Partition` property is used in derived classes to provide a value that defines the partition the document will be stored in. By having a generic `Partition` property, and marking it as `virtual`, we allow derived classes to fully control the value the property returns, and use the class's other properties in the value.

### The Company Class
One example of how you could override the `Partition` property is the [`Company`](DataModel/Company.cs) class. It uses the `Country` and `City` properties to create the `Partition` property. You may need to adjust this logic to better suit the geographical distribution of the companies in your system.

The `Partition` implementation looks like this.

``` C#
public override string Partition
{
    get => $"location:{this.Country}/{this.City}";
    set => { /** Deliberately empty */ }
}

```
