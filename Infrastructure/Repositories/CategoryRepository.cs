using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryEntity> AddAsync(CategoryEntity category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<CategoryEntity>> GetAllAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task<CategoryEntity?> GetByIdAsync(string id)
    {
        var category = await _context.Categories.FindAsync(id);
        return category ?? null;
    }

    public async Task<CategoryEntity> UpdateAsync(CategoryEntity category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        return category;
    }
}
