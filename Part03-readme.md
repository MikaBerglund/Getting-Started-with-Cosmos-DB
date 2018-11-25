# Part 3: Designing a Data Model

[<- Part 2](Part02-readme.md)

Designing the data model for your application is perhaps the most time-consuming task you have to take care of before actually staring to build your application. You don't have to completely design your data model, but there are certain things you must consider beforehand. This part focuses on those.

## JSON Document Requirements
As I wrote in [Part 2](Part02-readme.md), there are very few requirements set on a JSON document stored in Cosmos DB, as long as it is a valid JSON document. The following requirements also apply:

- **`id` attribute**: Every document **must** have an `id` attribute. No exceptions allowed. Please note that according to the [JSON PRC spec](https://jsonrpc.org/historical/json-rpc-1-1-alt.html#service-procedure-and-parameter-names) all names in a JSON document must be treated as case-sensitive, meaning that the attribute must be `id`, not `Id` or `ID`, but `id`. Keep this in mind.
- **Partition key**: Although not required for single-partition collections, I strongly recommend (see [scaling considerations](Part02-scaling.md)) that you only create multi-partition collections. For multi-partition collections, you must know the path to the attribute that is used as partition key. All documents that you store in the same collection must have this attribute. The value however should vary as much as possible to spread out your documents across multiple partitions. If you store documents without the attribute specified as partition key, then those documents will all be stored in the same partition with an empty partition key.

## Classes and Serialization
