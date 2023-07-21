using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IGenericService<T>
{
    Task Add(T obj);
    
    // Task AddUserToRole(T obj, string role);
    //
    // Task<bool> IsUserInRoleAsync(AppUser user, string role);

    Task Delete(Guid id);
    
    Task DeleteIdentity(string id);

    Task<T> GetById(Guid id);

    Task<List<T>> GetAll();

    Task<T> GetByPredicate(Func<T, bool> predicate);

    Task Update(Guid id, T obj);
    
    Task UpdateIdentity(string id, T obj);
    
    Task UpdateIdentityUserName(string id, T obj, string userName);

    string GetPasswordHash(string password);
    
    bool VerifyHashedPassword(string getUserPasswordHash, string password);

    Task<string> GetStringWithoutSpace(string input);
}