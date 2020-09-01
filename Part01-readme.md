# Part 1: The Development Environment

[<- Home](README.md) | [Part 2 ->](Part02-readme.md)

Before starting to dig in to working with Cosmos DB, I'd like to add a few words about your development environment. The tools described below are tools that I've found most useful when working with Cosmos DB solutions.

## Development Environment
I primarily use [Visual Studio 2017](https://docs.microsoft.com/en-us/visualstudio/install/install-visual-studio?view=vs-2017) for development work, and also for working on this tutorial. However, this tutorial does not contain anything that you cannot work on with [Visual Studio Code](https://code.visualstudio.com/), because I use only .NET Standard and .NET Core for all code in this tutorial. My preference for **Visual Studio 2017** over **Visual Studio Code** is purely my personal preference. You can achieve the same result either development environment.

## Local Cosmos DB Emulator
If you are running Windows (Server 2012 R2, Server 2016 or Windows 10), you can run Cosmos DB locally using the [Cosmos DB Emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator). With the emulator, you don't need to have an Azure subscription with Cosmos DB deployed. I always start my development with the local emulator.

## Azure Storage Explorer
> Cosmos DB support in Azure Storage Explorer has been deprecated in the August 2020 release, v1.15.0. The same functionality can be found in several different tools, but I would prefer the Cosmos Explorer, which is described below.

Although the Cosmos emulator comes with a browser based data explorer that you can use manage your Cosmos DB emulator and query data contained within it, you can also use [Azure Storage Explorer](https://azure.microsoft.com/en-us/features/storage-explorer/). It is capable of connecting to Cosmos DB collections running both in *Azure* as well as your local emulator. I find myself using the *Storage Explorer* especially when designing queries, because you can write your SQL-like queries and see what they return as well as the cost of the query in terms of `RU/s` (If you want to know more about *RU/s*, you can skip ahead to [*part 2: Scaling Considerations*](Part02-readme.md)). The Storage Explorer is available for Windows, macOS and Linux.

## Cosmos Explorer
The Cosmos Explorer is a browser based tool that you access on [cosmos.azure.com](https://cosmos.azure.com/). This tool gives you all of the features that you have in Azure Storage Explorer for Cosmos DB, and some more. This tool is developed by the Cosmos team, and available as [open source on GitHub](https://github.com/Azure/cosmos-explorer).

As with the Storage Explorer, this tool is not just for managing your databases and collections. It is very useful also when designing your queries. When you execute your SQL queries, not only do you get the results of that query, but you also get all the nice stats for that query, including the cost in terms of `RU/s`, which I talk more about in [*part 2: Scaling Considerations*](Part02-readme.md).

## Cosmos DB Migration Tool
At some point during your development work, you may need to move content between collections. The [Cosmos DB Migration Tool](https://docs.microsoft.com/en-us/azure/cosmos-db/import-data) is a handy tool for moving data. You can move data between the service running in Azure, local emulator or JSON files on your local disk. Dumping content to a local disk is also handy for backing up. The migration tool also comes with a command-line tool that you might find useful if you want to automate migrations, for instance for backup purposes.

Scroll down to the [Installation](https://docs.microsoft.com/en-us/azure/cosmos-db/import-data#Install) section to find a link to the source code or to download the latest precompiled version.

## What's Next
In [part 2](Part02-readme.md) I'll go through the mechanisms that give Cosmos DB practically unlimited scaling capabilities. Many of them are things you cannot change after you've created a database or collection, so it's important that you read that part through if you're not familiar with those.

[Go to Part 2 ->](Part02-readme.md)
