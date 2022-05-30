using Application.Interfaces.Repositories;
using Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class EfEntityRepository<TEntity, TContext, TKey> : IRepository<TEntity, TKey> where TEntity : BaseEntity<TKey> where TContext : DbContext
    {

        protected readonly TContext _context;
        public EfEntityRepository(TContext context)
        {
            _context = context;
        }
        public TEntity Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            return entity;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            return entity;
        }

        public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().AddRange(entities);
            return entities;
        }

        public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _context.Set<TEntity>().AddRangeAsync(entities);
            return entities;
        }
        
        public TEntity Get(Expression<Func<TEntity, bool>> expression, bool noTracking = false)
        {
            if (noTracking)
            {
                return _context.Set<TEntity>().AsNoTracking().FirstOrDefault(expression);
            }
            else
            {
                return _context.Set<TEntity>().FirstOrDefault(expression);
            }
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> expression = null, bool noTracking = true)
        {
            if (noTracking)
            {
                if (expression == null)
                {
                    return _context.Set<TEntity>().AsNoTracking().ToList();
                }
                else
                {
                    return _context.Set<TEntity>().AsNoTracking().Where(expression).ToList();
                }
            }
            else
            {
                if (expression == null)
                {
                    return _context.Set<TEntity>().ToList();
                }
                else
                {
                    return _context.Set<TEntity>().Where(expression).ToList();
                }
            }
        }
        
        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression = null, bool noTracking = true)
        {
            if (noTracking)
            {
                if (expression == null)
                {
                    return await _context.Set<TEntity>().AsNoTracking().ToListAsync();
                }
                else
                {
                    return await _context.Set<TEntity>().AsNoTracking().Where(expression).ToListAsync();
                }
            }
            else
            {
                if (expression == null)
                {
                    return await _context.Set<TEntity>().ToListAsync();
                }
                else
                {
                    return await _context.Set<TEntity>().Where(expression).ToListAsync();
                }
            }
        }
        
        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression, bool noTracking = false)
        {
            if (noTracking)
            {
                return  await _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(expression);
            }
            else
            {
                return await _context.Set<TEntity>().FirstOrDefaultAsync(expression);
            }
        }

        public TEntity GetById(TKey id)
        {
            return _context.Set<TEntity>().Find(id);
        }

        public async Task<TEntity> GetByIdAsync(TKey id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public int GetCount(Expression<Func<TEntity, bool>> expression = null)
        {
            if (expression == null)
            {
                return _context.Set<TEntity>().Count();
            }
            else
            {
                return  _context.Set<TEntity>().Count(expression);
            }
        }

        public async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> expression = null)
        {
            if (expression == null)
            {
                return await _context.Set<TEntity>().CountAsync();
            }
            else
            {
                return await _context.Set<TEntity>().CountAsync(expression);
            }
        }

        public IQueryable<TEntity> Queryable()
        {
            return _context.Set<TEntity>().AsQueryable();
        }

        public void Remove(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().RemoveRange(entities);
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().UpdateRange(entities);
        }
    }
}
