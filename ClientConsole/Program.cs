﻿using DataModel;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ClientConsole
{
    class Program
    {
        // DISCLAIMER: Never store sensitive information in your source code. This is the connection string to the local Cosmos DB emulator, which
        // only supports a single fixed master key for authentication, so the connection string is the same on any computer running the local
        // Cosmos DB emulator.
        const string ConnectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        const string DatabaseId = "Preparing-for-Cosmos-DB";
        const string CollectionId = "MyCollection";

        static DocumentClient Client;
        static Uri CollectionLink;

        static void Main(string[] args)
        {
            Client = CreateClient(ConnectionString);
            CollectionLink = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);

            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            //await WriteCompaniesAsync();
            //await WriteProjectsAsync();

            var finnishCompanies = await ReadCompaniesByCountryAsync("Finland");
            foreach(var company in finnishCompanies)
            {
                var docLink = UriFactory.CreateDocumentUri(DatabaseId, CollectionId, company.Id);
                await Client.ReadDocumentAsync(docLink);

                var projects = await ReadProjectsForCompanyAsync(company);
            }
        }

        static async Task<IQueryable<Company>> ReadCompaniesByCountryAsync(string country)
        {
            var query = CreateDocumentQuery<Company>(x => x.Country == country);
            var companies = await ExecuteDocumentQueryAsync(query);
            return companies;
        }

        static async Task<IQueryable<Project>> ReadProjectsForCompanyAsync(Company company)
        {
            var query = CreateDocumentQuery<Project>(x => x.CompanyGlobalId == company.GlobalId);
            var projects = await ExecuteDocumentQueryAsync(query);
            return projects;
        }

        static async Task WriteCompaniesAsync()
        {
            await WriteEntityAsync(new Company() { City = "Helsinki", Country = "Finland", Name = "Company #1", Id = "c1" });
            await WriteEntityAsync(new Company() { City = "Jyväskylä", Country = "Finland", Name = "Company #2", Id = "c2" });
            await WriteEntityAsync(new Company() { City = "Montreal", Country = "Canada", Name = "Company #3", Id = "c3" });
            await WriteEntityAsync(new Company() { City = "Stockholm", Country = "Sweden", Name = "Company #4", Id = "c4" });
            await WriteEntityAsync(new Company() { City = "Oslo", Country = "Norway", Name = "Company #5", Id = "c5" });
        }

        static async Task WriteProjectsAsync()
        {
            //-------------------------------------------------------------------------------------
            // First, query the database for companies.
            var c = new Company();
            var query = Client
                .CreateDocumentQuery<Company>(
                    CollectionLink,
                    new FeedOptions()
                    {
                        // Since our companies are stored in various partitions, we need
                        // to enable cross-partition queries.
                        EnableCrossPartitionQuery = true
                    }
                )
                .Where(x => x.DocumentType == c.DocumentType)
                .AsDocumentQuery()
                ;

            var result = await query.ExecuteNextAsync<Company>();
            var companyList = result.ToList();
            //-------------------------------------------------------------------------------------

            //-------------------------------------------------------------------------------------
            // Then, go through each company, and write a set of projects for each company.
            foreach(var company in companyList)
            {
                await WriteProjectsForCompanyAsync(company);
            }
            //-------------------------------------------------------------------------------------

        }

        static async Task WriteProjectsForCompanyAsync(Company company)
        {
            // Note that we can use the same ID for multiple projects as long as they are for 
            // different companies, since each company will has its own partition where the
            // projects are stored, and IDs must be unique only within a partition in Cosmos DB.

            await WriteEntityAsync(new Project() { Company = company, Name = "Project #1", Id = "p1" });
            await WriteEntityAsync(new Project() { Company = company, Name = "Project #2", Id = "p2" });
            await WriteEntityAsync(new Project() { Company = company, Name = "Project #3", Id = "p3" });
        }

        static async Task WriteEntityAsync<TEntity>(TEntity entity) where TEntity : DocumentBase
        {
            await Client.UpsertDocumentAsync(CollectionLink, entity);
        }

        static DocumentClient CreateClient(string connectionString)
        {
            // Use a DBConnectionStringBuilder isntance to parse the properties
            // in the connection string for us.
            var builder = new System.Data.Common.DbConnectionStringBuilder()
            {
                ConnectionString = connectionString
            };

            return new DocumentClient(new Uri($"{builder["AccountEndpoint"]}"), $"{builder["AccountKey"]}");
        }

        /// <summary>
        /// Creates a document query for the given document type and given predicate that spans across multiple partitions.
        /// </summary>
        /// <typeparam name="TDocument">The type of documents to query.</typeparam>
        /// <param name="predicate">The predicate to use to match documents.</param>
        static IDocumentQuery<TDocument> CreateDocumentQuery<TDocument>(Expression<Func<TDocument, bool>> predicate) where TDocument : DocumentBase
        {
            var doc = Activator.CreateInstance<TDocument>();

            return Client
                .CreateDocumentQuery<TDocument>(
                    CollectionLink,
                    new FeedOptions()
                    {
                        EnableCrossPartitionQuery = true
                    }
                )
                .Where(x => x.DocumentType == doc.DocumentType)
                .Where(predicate)
                .AsDocumentQuery()
                ;
        }

        /// <summary>
        /// Executes the given document query and returns all results.
        /// </summary>
        /// <typeparam name="TDocument">The type of document.</typeparam>
        /// <param name="query">The query to execute.</param>
        /// <returns></returns>
        static async Task<IQueryable<TDocument>> ExecuteDocumentQueryAsync<TDocument>(IDocumentQuery<TDocument> query) where TDocument : DocumentBase
        {
            var list = new List<TDocument>();
            string continuation = null;

            do
            {
                var result = await query.ExecuteNextAsync<TDocument>();
                continuation = result.ResponseContinuation;
                list.AddRange(result);
            }
            while (null != continuation);

            return list.AsQueryable();
        }

    }
}
