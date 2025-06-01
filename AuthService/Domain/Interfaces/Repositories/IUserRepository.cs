using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IUserRepository
{
    // CREATE
    Task<UserEntity> CreateAsync(UserEntity user);
    
    // READ
    Task<UserEntity?> GetByIdAsync(string id);
    Task<UserEntity?> GetByEmailAsync(string email);
    Task<IEnumerable<UserEntity>> GetAllAsync(bool includeInactive = false);
    
    // UPDATE
    Task<UserEntity?> UpdateAsync(UserEntity user);
    
    // DELETE
    Task<bool> DeleteAsync(string id);
    
    // SOFT DELETE
    Task<bool> DeactivateAsync(string id);
    
    // REACTIVATE
    Task<bool> ReactivateAsync(string id);
}

