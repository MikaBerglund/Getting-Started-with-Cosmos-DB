# Part 4: Creating a Cosmos DB Database

[<- Part 3](Part03-readme.md) | [Part 5 ->](Part05-readme.md)

If comparing Cosmos DB to SQL Server and the containers you have there, Cosmos DB also has its similar containers. These can roughly be mapped as follows:

| SQL Server | Cosmos DB     |
|------------|---------------|
| Server     | Account       |
| Database   | Database      |
| Table      | Collection    |
| Row        | JSON Document |

There are of course a lot of differences between these, but maybe the biggest conceptual difference is in *tables* vs. *collections*. In SQL Server, each row in a table must follow the same schema, where as each JSON document in a Cosmos DB collection can be more or less anything, as long as it is a valid JSON document. There are a few things that apply to each JSON document that is stored in Cosmos DB, but I'll go through those more in detail in [part 4](Part04-readme.md).


## Creating a Database
In Cosmos DB you can create a database with or without shared provisioned throughput. Here, I'll talk mostly about databases with provisioned throughput, because that is becoming my #1 option actually.

As we speak (early days of dec. 2018), there is a major change being deployed to Cosmos DB. Previously, the minimum RU/s you could share across all your collections in a database was 10 000 RU/s, which would lead to a monthly cost of almost 500 EUR.

Now, the Cosmos DB team is bringing that minimum down to just 400 RU/s, which is the minimum you can have separately for collections, so there is actually no reason (except for maybe a few marginal ones) not to create databases with shared provisioned throughput. Read more about it from this [article](https://docs.microsoft.com/en-us/azure/cosmos-db/set-throughput) on the Azure documentation site.

This makes the throughput shared on a database level a very viable option for more or less any size of project or application. Very often you have many environments for one application like **development**, **testing** and **production**. Typically, your development and testing environment have get very small loads with maybe some occasional spikes while running performance testing for instance.

Sharing throughput across these environments allows you to start low and provision just 400 RU/s for all three environments. If you would provision the throughput for each environment separately, you would need to provision three times the same amount, which would also cost you three times more.

400 RU/s is the minimum throughput you can provision on a database level in a database with a maximum 4 collections. **For each additional collection the minimum goes up by 100 RU/s**, which is reasonable, I guess. So, if you have a database with 12 collections, the minimum provisioned throughput for that database would be 1200 RU/s.

## Creating Collections
The data you store in Cosmos DB is always stored in collections that you create in databases. If you are creating a collection in a database with provisioned throughput, then **all collections in that database are multi-partition collections**, meaning you have to specify the partition key. The partition key is covered in more detail in [part 2](Part02-readme.md) and also somewhat in [part 3](Part03-readme.md).

If you are creating a single-partition, then you don't have to, or actually, you can't, specify a partition key. Remember, **the maximum size for a single-partition collection is always 10 GB**. Storing more data requires multi-partition collections.

## Conclusion
Since the minimum throughput you can provision on a database level is [coming down to 400 RU/s](https://docs.microsoft.com/en-us/azure/cosmos-db/set-throughput#comparison-of-models), and let you share that throughput across all your collections in that database, I would always first consider provisioning the necessary throughput on a database level, because that gives you more flexible scaling, in my opinion. Of course there are situations where this is not the best option, and you need to consider them separately from your point of view.

If you want, you can always create a collection with dedicated provisioned throughput in a database with shared provisioned throughput, which might be an option for you in some cases.

## What's Next
In [part 5](Part05-readme.md) I'll go through some basics about reading and writing data in a Cosmos DB database using the data model that I covered in [part 3](Part03-readme.md). Also make sure to check out the [provisioning throughput](https://docs.microsoft.com/en-us/azure/cosmos-db/set-throughput) documentation for Cosmos DB.

[Go to Part 5 ->](Part05-readme.md)