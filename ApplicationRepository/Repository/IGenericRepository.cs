using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationRepository.Repository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        public Task Create(TEntity entity);
        public Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>>? filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, string? includeProperties = null);
        public Task Update(TEntity entity, params object[] keys);
        public Task Delete(params object[] keys);
    }
}
