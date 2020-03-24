# Part 05: Reading and Writing Data Using the Data Model

[<- Part 4](Part04-readme.md)

> Note! This part is written for the v2 of the Cosmos DB SDK. There is now a newer, more simpler version of the SDK that you really should look into. I will update the documentation in this part to match [Cosmos DB SDK v3](https://github.com/Azure/azure-cosmos-dotnet-v3).

The last part in this tutorial is just a brief overview of how you can use your data model you created in [Part 3](Part03-readme.md) to read and write data in your Cosmos DB database.

I will not go into deep details on data access in this tutorial because there will be another tutorial about how query data in a Cosmos DB database using [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/), which I think has matured into a viable option for data access.

> DISCLAIMER. The code demonstrated in this tutorial should by no means be considered as production-ready. Its purpose is merely to demonstrate basics on querying data in a Cosmos DB database.

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

### Querying data
Let's have a look at a few samples on how to query for data. If you know the ID of the document you want to read, the you use the `ReadDocumentAsync` method like this:

``` C#
var docLink = UriFactory.CreateDocumentUri(
    DatabaseId, 
    CollectionId, 
    "c1" // The ID of the company
);
var company = await Client.ReadDocumentAsync(docLink);
```

That's pretty easy, isn't it? Now, if you don't know the ID or if you want to query for many items, then there's a bit more you need to do. Getting the data by querying is always a two-part task:

1. Create the Linq query with all the wheres, order bys and all that.
2. Execute the query in a do-while loop.

In the [`Program`](ClientConsole/Program.cs) class I've created a generic method for both of these parts to make it easier to use when querying. The methods are `CreateDocumentQuery` and `ExecuteDocumentQueryAsync` respectively. I will not list them here because you find them in the [source code](ClientConsole/Program.cs). I will just use them in the sample below.

In the sample code I've also created a few companies and associated a couple of projects with each of the companies. So, in the code below, I'll first query for all companies in *Finland*, and then I'll loop over the results and query for all projects for each of the companies. The code will then look something like this:

``` C#
var companyQuery = CreateDocumentQuery<Company>(
    c => c.Country == "Finland"
);
var companies = await ExecuteDocumentQueryAsync(companyQuery);

foreach(var company in companies)
{
    var projectsQuery = CreateDocumentQuery<Project>(
        p => p.CompanyGlobalId == company.GlobalId
    );
    var projects = await ExecuteDocumentQueryAsync(projectsQuery);
}
```

You probably get the hang of it already. See the full code sample in the [`Program`](ClientConsole/Program.cs) class.

## Conclusion
This was the last part in this tutorial. I hope you found it useful. Please feel free to fork this repo and make it your own. If you feel that something is missing or incorrect, I'm happy to take a pull request.

I will cover querying data more in detail in a coming tutorial that covers querying Cosmos DB using Entity Framework Core. I'll put a link to that tutorial here when it's available.

Thanks for reading!
