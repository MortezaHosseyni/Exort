using MongoDB.Driver;

namespace Domain.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetPaginatedAsync(int pageSize, int pageNumber);
        Task<IEnumerable<TEntity>> GetLimitedAsync(int limit);
        Task<IEnumerable<TEntity>> FindAsync(FilterDefinition<TEntity> filter);
        Task<TEntity> FindOneAsync(FilterDefinition<TEntity> filter);
        Task<bool> AnyAsync();
        Task<bool> AnyByPredicateAsync(FilterDefinition<TEntity> filter);
        Task<long> CountAsync();
        Task<long> CountByPredicateAsync(FilterDefinition<TEntity> filter);
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        Task UpdateAsync(FilterDefinition<TEntity> filter, TEntity entity);
        Task UpdateRangeAsync(FilterDefinition<TEntity> filter, IEnumerable<TEntity> entities);
        Task RemoveAsync(FilterDefinition<TEntity> filter);
        Task RemoveRangeAsync(FilterDefinition<TEntity> filter);
    }
    public class GenericRepository<TEntity>(IMongoCollection<TEntity> collection) : IGenericRepository<TEntity>
        where TEntity : class
    {
        protected readonly IMongoCollection<TEntity> Collection = collection;

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Collection.Find(Builders<TEntity>.Filter.Empty).ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetPaginatedAsync(int pageSize, int pageNumber)
        {
            int itemsToSkip = (pageNumber - 1) * pageSize;
            return await Collection.Find(Builders<TEntity>.Filter.Empty)
                                    .Skip(itemsToSkip)
                                    .Limit(pageSize)
                                    .ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetLimitedAsync(int limit)
        {
            return await Collection.Find(Builders<TEntity>.Filter.Empty)
                                    .Limit(limit)
                                    .ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(FilterDefinition<TEntity> filter)
        {
            return await Collection.Find(filter).ToListAsync();
        }

        public async Task<TEntity> FindOneAsync(FilterDefinition<TEntity> filter)
        {
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> AnyAsync()
        {
            return await Collection.Find(Builders<TEntity>.Filter.Empty).AnyAsync();
        }

        public async Task<bool> AnyByPredicateAsync(FilterDefinition<TEntity> filter)
        {
            return await Collection.Find(filter).AnyAsync();
        }

        public async Task<long> CountAsync()
        {
            return await Collection.CountDocumentsAsync(Builders<TEntity>.Filter.Empty);
        }

        public async Task<long> CountByPredicateAsync(FilterDefinition<TEntity> filter)
        {
            return await Collection.CountDocumentsAsync(filter);
        }

        public async Task AddAsync(TEntity entity)
        {
            await Collection.InsertOneAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await Collection.InsertManyAsync(entities);
        }

        public async Task UpdateAsync(FilterDefinition<TEntity> filter, TEntity entity)
        {
            await Collection.ReplaceOneAsync(filter, entity);
        }

        public async Task UpdateRangeAsync(FilterDefinition<TEntity> filter, IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                await Collection.ReplaceOneAsync(filter, entity);
            }
        }

        public async Task RemoveAsync(FilterDefinition<TEntity> filter)
        {
            await Collection.DeleteOneAsync(filter);
        }

        public async Task RemoveRangeAsync(FilterDefinition<TEntity> filter)
        {
            await Collection.DeleteManyAsync(filter);
        }
    }
}
