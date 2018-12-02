# Part 2: Creating a Cosmos DB Database

[<- Part 1](Part01-readme.md) | [Part 3 ->](Part03-readme.md)

- [Scaling considerations](Part02-scaling.md)

If comparing Cosmos DB to SQL Server and the containers you have there, Cosmos DB also has its similar containers. These can roughly be mapped as follows:

| SQL Server | Cosmos DB     |
|------------|---------------|
| Server     | Account       |
| Database   | Database      |
| Table      | Collection    |
| Row        | JSON Document |

There are of course a lot of differences between these, but maybe the biggest conceptual difference is in *tables* vs. *collections*. In SQL Server, each row in a table must follow the same schema, where as each JSON document in a Cosmos DB collection can be more or less anything, as long as it is a valid JSON document. There are a few things that apply to each JSON document that is stored in Cosmos DB, but I'll go through those more in detail in [part 3](Part03-readme.md).

## Scaling Considereations
Before creating new databases and collections read [this document](Part02-scaling.md) that describes the considerations you have to make before creating databases and collections. Some of those settings cannot be changed after a collection has been created.

## Creating a Database
In Cosmos DB you can create a database and provision throughput (RU/s) that will be shared across all your collections created in that database. However, the minimum throughput you could previously share on a database level was 10 000 RU/s, which would cost almost 500 EUR / month, so that was a viable option only for larger systems.

Luckily this is being changed as we speak (early days of Dec. 2018), and the minimum throughput that you can provision on a database level is going down to 400 RU/s, as you can read from this [Azure documentation article](https://docs.microsoft.com/en-us/azure/cosmos-db/set-throughput).

This makes the throughput shared on a database level a very viable option for more or less any size of project or application. Very often you have many environments for one application like **development**, **testing** and **production**. Typically, your development and testing environment have get very small loads with maybe some occasional spikes while running performance testing for instance.

Sharing throughput across these environments allows you to start low and provision just 400 RU/s for all three environments. If you would provision the throughput for each environment separately, you would need to provision three times the same amount, which would also cost you three times more.

## Creating Collections
The data you store in Cosmos DB is always stored in collections that you create in databases.