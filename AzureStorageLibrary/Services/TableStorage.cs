using Azure;
using Azure.Data.Tables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzureStorageLibrary.Services
{
    public class TableStorage<TEntity> : INoSqlStorage<TEntity> where TEntity :class, ITableEntity, new()
    {
        private readonly TableClient _tableClient;

        public TableStorage()
        {
            var connectionString=ConnectionString.AzureStorageConnectionString;
            var tableName=typeof(TEntity).Name;
            var serviceClient = new TableServiceClient(connectionString);
            
            _tableClient = serviceClient.GetTableClient(tableName);
            _tableClient.CreateIfNotExists();
        }

        public async Task<TEntity> Add(TEntity entity)
        {
         await _tableClient.AddEntityAsync(entity);
            return await Get(entity.RowKey, entity.PartitionKey);
        }

        public async Task<IQueryable<TEntity>> All()
        {
            var entities=new List<TEntity>();
           var result= _tableClient.QueryAsync<TEntity>();
            await foreach (var entity in result)
            {
                entities.Add(entity);
            }
            return entities.AsQueryable();  
        }

        public async Task Delete(string rowKey, string partitionKey)
        {
            await _tableClient.DeleteEntityAsync(rowKey, partitionKey);
        }

        public async Task<TEntity> Get(string rowKey, string partitionKey)
        {
            try
            {
                var response = await _tableClient.GetEntityAsync<TEntity>(partitionKey, rowKey);
                return response.Value;

            }
            catch (RequestFailedException ex) when (ex.Status==404) {
                return null;
            }
        }

        public async Task<IQueryable<TEntity>> Query(Expression<Func<TEntity, bool>> query)
        {
            var entities = new List<TEntity>();
            var result = _tableClient.QueryAsync<TEntity>(query);
            await foreach (var entity in result)
            {
                entities.Add(entity);
            }
            return entities.AsQueryable();
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            await _tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace);
            return await Get(entity.RowKey, entity.PartitionKey);
        }
    }
}
