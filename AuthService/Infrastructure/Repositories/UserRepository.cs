namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    
    // CREATE
    public async Task<UserEntity> CreateAsync(UserEntity user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }
    
    // READ
    public async Task<UserEntity?> GetByIdAsync(string id)
    {
        return await _context.Users.FindAsync(id);
    }
    
    public async Task<UserEntity?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }
    
    public async Task<IEnumerable<UserEntity>> GetAllAsync(bool includeInactive = false)
    {
        IQueryable<UserEntity> query = _context.Users;
        
        if (!includeInactive)
            query = query.Where(u => u.IsActive);
            
        return await query.ToListAsync();
    }
    
    public async Task<UserEntity?> UpdateAsync(UserEntity user)
    {
        var existingUser = await _context.Users.FindAsync(user.Id);
        if (existingUser == null)
            return null;
            
        // Atualiza as propriedades
        user.UpdatedAt = DateTime.UtcNow;
        _context.Entry(existingUser).CurrentValues.SetValues(user);
        await _context.SaveChangesAsync();
        
        return existingUser;
    }
    
    public async Task<bool> DeleteAsync(string id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;
            
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<bool> DeactivateAsync(string id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;
            
        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<bool> ReactivateAsync(string id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;
            
        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        
        return true;
    }
}

