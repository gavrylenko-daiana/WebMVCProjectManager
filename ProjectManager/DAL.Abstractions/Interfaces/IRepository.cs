using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;

namespace DAL.Abstractions.Interfaces;

public interface IRepository<T>
{
    Task<Result<List<T>>> GetAllAsync(int pageNumber = 1, int pageSize = 20);
    
    Task<Result<T?>> GetByIdAsync(Guid id);
    
    Task<Result<bool>> CreateAsync(T entity);

    // Task<Result<bool>> AddToRoleAsync(T entity, string role);

    Task<Result<bool>> UpdateAsync(Guid id, T updatedObj);
    
    Task<Result<bool>> UpdateIdentityAsync(string id, T updatedObj);
    
    Task<Result<bool>> DeleteAsync(Guid id);
    
    Task<Result<bool>> DeleteIdentityAsync(string id);
    
    Task<Result<T>> GetByPredicateAsync(Func<T, bool> predicate);
}