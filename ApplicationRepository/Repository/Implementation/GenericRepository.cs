using ApplicationContext;
using ApplicationCore.IType;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationRepository.Repository.Implementation
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        internal readonly TableReservationContext _context;
        internal readonly DbSet<TEntity> _dbSet;
        public GenericRepository(TableReservationContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }
        public virtual Task Create(TEntity entity)
        {
            if (entity is IAuditEntity)
            {
                ((IAuditEntity)entity).Created = DateTime.Now;
                ((IAuditEntity)entity).Modified = DateTime.Now;
            }
            if (entity is ISoftDeleteEntity)
            {
                ((ISoftDeleteEntity)entity).IsDeleted = false;
            }
            _dbSet.Add(entity);
            return Task.CompletedTask;
        }

        public Task Delete(params object[] keys)
        {
            var entity = _dbSet.Find(keys);
            if (entity == null) throw new ArgumentNullException("entity");
            if (entity is ISoftDeleteEntity)
            {
                ((ISoftDeleteEntity)entity).IsDeleted = true;
                _dbSet.Update(entity);
            }
            else
            {
                _dbSet.Remove(entity);
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, string? includeProperties = null)
        {
            var query = _dbSet.AsQueryable();
            if (includeProperties != null)
            {
                foreach (string property in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            return Task.FromResult(query.AsEnumerable());
        }

        public Task Update(TEntity entity, params object[] keys)
        {
            var find = _dbSet.Find(keys);
            if(find != null)
            {
                if (entity is IAuditEntity)
                {
                    ((IAuditEntity)entity).Modified = DateTime.Now;
                }
                _context.Entry(find).CurrentValues.SetValues(entity);
                return Task.CompletedTask;
            } else
            {
                throw new KeyNotFoundException();
            }
        }
    }
}
