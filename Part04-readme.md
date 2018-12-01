# Part 04: Querying Data Using the Data Model

[<- Part 3](Part03-readme.md)

The last part in this tutorial is just a brief overview of how you can use your data model you created in [Part 3](Part03-readme.md) to read and write data in your Cosmos DB database.

I will not go into deep details on data access in this tutorial because there will be another tutorial about how query data in a Cosmos DB database using [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/), which I think has matured into a viable option for data access.

## The `ClientConsole` Project
To simulate a client application that works with data in a Cosmos DB database, I've created the [`ClientConsole`](ClientConsole/ClientConsole.csproj) console application. It is a .NET Core Console application, so you can run it more or less on any OS. While you can access data in a Cosmos DB database using its REST APIs, it is more advisable to use the [`Microsoft.Azure.DocumentDB.Core`](https://www.nuget.org/packages/Microsoft.Azure.DocumentDB.Core/) library.

The majority of the code in this sample client application is in the [`Program`](ClientConsole/Program.cs) class.

### Connecting to Cosmos DB
The first thing you need to do when connecting to a Cosmos DB database is to have a [`DocumentClient`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.documents.client.documentclient) class instance.

``` C#
static DocumentClient CreateClient(string connectionString)
{
    var builder = new System.Data.Common.DbConnectionStringBuilder()
    {
        ConnectionString = connectionString
    };

    return new DocumentClient(
        new Uri($"{builder["AccountEndpoint"]}"), 
        $"{builder["AccountKey"]}"
    );
}
```

The connection string to a Cosmos DB database, which actually is just the connection string to the Cosmos DB account, contains two properties which we need to extract, because the `DocumentClient` class wants these properties separately. To parse these properties, we use an [`DbConnectionStringBuilder`](https://docs.microsoft.com/en-us/dotnet/api/system.data.common.dbconnectionstringbuilder?view=netcore-2.1) instance.

### Writing to the Database
Now you are ready to write your first document to the database. The code below describes what you need to do (assuming you have the `CreateClient` method from above available).

``` C#
var connectionString = "[Your connection string]";
var collectionLink = UriFactory.CreateDocumentCollectionUri(
    "[Database name]",
    "[Collection name]"
);
var client = CreateClient(connectionString);

var company = new Company()
{
    City = "Helsinki",
    Country = "Finland",
    Name = "My Company"
};
await client.UpsertDocumentAsync(collectionLink, company);
```

See more detailed code samples in the [`Program`](ClientConsole/Program.cs) class.