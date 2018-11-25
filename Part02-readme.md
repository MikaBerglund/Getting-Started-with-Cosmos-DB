# Part 2: Creating a Cosmos DB Database

If comparing Cosmos DB to SQL Server and the containers you have there, Cosmos DB also has its similar containers. These can roughly be mapped as follows:

| SQL Server | Cosmos DB     |
|------------|---------------|
| Server     | Account       |
| Database   | Database      |
| Table      | Collection    |
| Row        | JSON Document |

There are of course a lot of difference between these, but maybe the biggest conceptual difference is in *tables* vs. *collections*. In SQL Server, each row in a SQL Server table must follow the same schema, where as each JSON document in a Cosmos DB collection can be more or less anything, as long as it is a valid JSON document. There are a few things that apply to each JSON document that is stored in Cosmos DB, but I'll go through those more in detail in [part 3](Part03-readme.md).

## Scaling Considereations
Before creating a new collection read [this document](Part02-scaling.md) that describes the considerations you have to make. Some of those settings cannot be changed after a collection has been created.

## Creating a Collection
Now that you know about the very basics, it is time to create a collection. You can create new collections on the [Azure portal](https://portal.azure.com/), or the data explorer that comes with your local Cosmos DB emulator, but I'll use the [Azure Storage Explorer](https://azure.microsoft.com/en-us/features/storage-explorer/) that I wrote about in [Part 1](Part01-readme.md).

