using System.Linq.Expressions;
using Newtonsoft.Json;
using Core.Models;
using DAL.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Services
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private AppContext _context;
        private Microsoft.EntityFrameworkCore.DbSet<T> _dbSet;

        public Repository(AppContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<Result<List<T>>> GetAllAsync(int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var data = await _dbSet.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
                return new Result<List<T>>(true, data);
            }
            catch (Exception ex)
            {
                return new Result<List<T>>(false, ex.Message);
            }
        }

        public async Task<Result<T?>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                return new Result<T?>(true, entity);
            }
            catch (Exception ex)
            {
                return new Result<T?>(false, ex.Message);
            }
        }

        public async Task<Result<bool>> CreateAsync(T entity)
        {
            try
            {
                _dbSet.Add(entity);
                await _context.SaveChangesAsync();
                return new Result<bool>(true);
            }
            catch (Exception ex)
            {
                return new Result<bool>(false, ex.InnerException.Message);
            }
        }

        public async Task<Result<bool>> UpdateAsync(Guid id, T updatedObj)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                {
                    return new Result<bool>(false, $"Entity with Id {id} not found.");
                }

                _context.Entry(entity).CurrentValues.SetValues(updatedObj);
                await _context.SaveChangesAsync();
                return new Result<bool>(true);
            }
            catch (Exception ex)
            {
                return new Result<bool>(false, ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();
                }

                return new Result<bool>(true);
            }
            catch (Exception ex)
            {
                return new Result<bool>(false, ex.Message);
            }
        }

        public async Task<Result<T>> GetByPredicateAsync(Func<T, bool> predicate)
        {
            try
            {
                var data = _dbSet.FirstOrDefault(predicate);
                if (data != null)
                {
                    return new Result<T>(true, data);
                }
                else
                {
                    return new Result<T>(false, $"No {typeof(T).Name} found matching the predicate.");
                }
            }
            catch (Exception ex)
            {
                return new Result<T>(false, ex.Message);
            }
        }
    }
}