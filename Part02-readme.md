# Part 2: Scaling Considerations

[<- Part 1](Part01-readme.md) | [Part 3 ->](Part03-readme.md)

The first thing you have to know before creating databases and collections in Cosmos DB is how it is designed for scaling.

A quote from the [Azure documentation](https://docs.microsoft.com/en-us/azure/azure-subscription-service-limits#azure-cosmos-db-limits):
> Azure Cosmos DB is a global scale database in which throughput and storage can be scaled to handle whatever your application requires. If you have any questions about the scale Azure Cosmos DB provides, please send email to askcosmosdb@microsoft.com.

**Whatever your application requires** is a pretty bold statement. Let's look a little deeper into how this is achieved. The two main instruments for scaling are *partitioning* and *request units / second*, which are described below.

## Partitioning
Every collection is partitioned. When you create a collection you must select whether you create a single-partition collection or a collection with multiple partitions. Depending on how you create your database, your collections in that partion might be required to be multi-partition collections (see [part 4](Part04-readme.md) to find out more). The maximum amount of data you can store in one partition is 10 GB. You cannot increase that, nor can you change a collection from single-partition to multi-partition, so if you created a single-partion collection, you're stuck with 10 GB data storage.

If you find that you would need to scale from a single-partition to multi-partition, you have to create a new collection and migrate your data to the new collection. Luckily, that's pretty easy with for instance the [Cosmos DB Migration Tool](https://docs.microsoft.com/en-us/azure/cosmos-db/import-data) that I talked about in [part 1](Part01-readme.md).

If you are developing anything more than some quick mockup for something, then I would suggest that you **always use multi-partition collections**, because there are practically no drawbacks compared to single-partition collections. The only thing you have to do with multi-partition collections compared to single-partition, is that you need decide on a partition key for your documents. In practice that means deciding on which field in your JSON documents contain the value that detemines to which partition the document belongs to. To read more about creating your data model, see [part 3](Part03-readme.md).

> **Always** create your collections as **multi-partition collections** because they are no more expensive than single-partition collections, but offer you practically **unlimited scaling** options. If you provision throughput (RU/s) on the database level, then your collections are required to have a partition key specified, which means the collection becomes a multi-partition collection.

## Request Units / Second
Request Units and Request Units / Second (*RU/s*) is the mechanism for determining the throughput for your collections. There are ways to share throughput across multiple collections in a database, but here I'll focus on the throughput on a collection level.

When scaling your collection, you define the number of request units / second it should be capable of delivering. A Request Unit is defined as:

> It "costs" 1 RU to read a 1 kB document, and 5 RUs to write a 1 kB document.

This means that with the minimum 400 RU/s, you can read 400 1 kB documents ever second, or write 80 of those documents, or any combination of these. This is however just a theoretical maximum, because a small part of your RU/s goes to reading and writing indexes as well.

## Conclusion
If you ask me, I would always go with multi-partition collections. It is true that you have to plan your data model a bit more, because you need to consider partitioning, but that is not a bad thing at all, in my opinion anyway.

Since the RU/s is the only thing that costs money (OK, storage also costs something, but that cost is quite marginal), and the minimum throughput for both single-partition and multi-partition collections is the same, 400 RU/s, then I don't see any point in running single-partition collections, except for maybe some quick mockups etc.

## What's Next
In [part 3](Part03-readme.md) I'll go over some basic principles for creating a data model for your application. Most of those principles have nothing to do with Cosmos DB, but are things that I've found very useful over the years when creating data models for different kinds of data storages.

[Go to Part 3 ->](Part03-readme.md)
