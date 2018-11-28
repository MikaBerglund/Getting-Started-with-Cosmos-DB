# Part 1: The Development Environment

[<- Home](README.md) | [Part 2 ->](Part02-readme.md)

Before starting to dig in to working with Cosmos DB, I'd like to add a few words about your development environment. The tools described below are tools that I've found most useful when working with Cosmos DB solutions.

## Development Environment
I primarily use [Visual Studio 2017](https://docs.microsoft.com/en-us/visualstudio/install/install-visual-studio?view=vs-2017) for development work, and also for working on this tutorial. However, this tutorial does not contain anything that you cannot work on with [Visual Studio Code](https://code.visualstudio.com/), because I use only .NET Standard and .NET Core for all code in this tutorial. My preference for **Visual Studio 2017** over **Visual Studio Code** is purely my personal preference. You can achieve the same result either development environment.

## Local Cosmos DB Emulator
If you are running Windows (Server 2012 R2, Server 2016 or Windows 10), you can run Cosmos DB locally using the [Cosmos DB Emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator). With the emulator, you don't need to have an Azure subscription with Cosmos DB deployed. I always start my development with the local emulator.

## Azure Storage Explorer
Although the Cosmos emulator comes with a browser based data explorer that you can use manage your Cosmos DB emulator and query data contained within it, you can also use [Azure Storage Explorer](https://azure.microsoft.com/en-us/features/storage-explorer/). It is capable of connecting to Cosmos DB collections running both in *Azure* as well as your local emulator. I find myself using the *Storage Explorer* especially when designing queries, because you can write your SQL-like queries and see what they return as well as the cost of the query in terms of `RU/s` (If you want to know more about *RU/s*, you can skip ahead to [*Scaling Considerations*](Part02-scaling.md) in Part 2). The Storage Explorer is available for Windows, macOS and Linux.

## Cosmos DB Migration Tool
At some point during your development work, you may need to move content between collections. The [Cosmos DB Migration Tool](https://docs.microsoft.com/en-us/azure/cosmos-db/import-data) is a handy tool for moving data. You can move data between the service running in Azure, local emulator or JSON files on your local disk. Dumping content to a local disk is also handy for backing up. Unfortunately you can't schedule the tool to do that regularly.

Scroll down to the [Installation](https://docs.microsoft.com/en-us/azure/cosmos-db/import-data#Install) section to find a link to the source code or to download the latest precompiled version.