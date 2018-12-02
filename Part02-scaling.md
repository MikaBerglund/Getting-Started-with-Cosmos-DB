# Part 2: Scaling Considerations

[<- Part 2](Part02-readme.md)

The first thing you have to know before creating databases and collections in Cosmos DB is how it is designed for scaling.

A quote from the [Azure documentation](https://docs.microsoft.com/en-us/azure/azure-subscription-service-limits#azure-cosmos-db-limits):
> Azure Cosmos DB is a global scale database in which throughput and storage can be scaled to handle whatever your application requires. If you have any questions about the scale Azure Cosmos DB provides, please send email to askcosmosdb@microsoft.com.

**Whatever your application requires** is a pretty bold statement. Let's look a little deeper into this is achieved. The two main instruments for scaling are *partitioning* and *request units / second*, which are described below.

## Partitioning
Every collection is partitioned. When you create a collection you must select whether you create a single-partition collection or a collection with multiple partitions. The maximum amount of data you can store in one partition is 10 GB. You cannot increase that, nor can you change a collection from single-partition to multi-partition, so if you created a single-partion collection, you're stuck with 10 GB data storage.

If you find that you would need to scale from a single-partition to multi-partition, you have to create a new collection and migrate your data to the new collection. Luckily, that's pretty easy with for instance the [Cosmos DB Migration Tool](https://docs.microsoft.com/en-us/azure/cosmos-db/import-data) that I talked about in [Part 1](Part01-readme.md).

If you are developing anything more than some quick mockup for something, then I would suggest that you **always use multi-partitioned collections**, because there are practically no drawbacks compared to single-partition collections. The only thing you have to do with multi-partitioned collections compared to single-partitioned, is that you need decide on a partition key for your documents. In practice that means deciding on which field in your JSON documents contain the value that detemines to which partition the document belongs to. More on the considerations for creating a collection later in this document.

> **Always** create your collections as **multi-partition collections** because they are no more expensive than single-partition collections, but offer you practically **unlimited scaling** options. If you provision throughput (RU/s) on the database level, then your collections are required to have a partition key specified, which means the collection becomes a multi-partition collection.

## Request Units / Second
Request Units and Request Units / Second (*RU/s*) is the mechanism for determining the throughput for your collections. There are ways to share throughput across multiple collections in a database, but here I'll focus on the throughput on a collection level.

When scaling your collection, you define the number of request units / second it should be capable of delivering. A Request Unit is defined as:

> It "costs" 1 RU to read a 1 kB document, and 5 RUs to write a 1 kB document.

This means that with the minimum 400 RU/s that every collection has (apart from the setups where RUs are shared across multiple collections), you can read 400 1 kB documents ever second, or write 80 of those documents, or any combination of these. This is however just a theoretical maximum, because a small part of your RU/s goes to reading and writing indexes.

## Conclusion
If you ask me, I would always go with multi-partitioned collections. It is true that you cannot create a new multi-partitioned collection with less than 1000 RU/s, but after you've created one, you can go and drop the throughput down to 400 RU/s, which is the same minimum that you have with singe-partion collections.

Since the RU/s is the only thing that costs money (OK, storage also costs something, but that cost is quite marginal, and if you go over 10 GB, you need to run in multi-partition collections anyway), and you can have the same throughput for both single-partition and multi-partition collections, I don't see any point in running single-partition collections, except for maybe some quick mockups etc.