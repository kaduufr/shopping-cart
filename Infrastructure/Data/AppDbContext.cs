namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<ProductEntity> Products { get; set; } = null!;
    
    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     base.OnModelCreating(modelBuilder);
    //     // Configure suas entidades aqui
    //     // modelBuilder.Entity<UserEntity>()
    //     //     .ToTable("Users", "public")
    //     //     .HasKey(u => u.Id);
    //     //
    //     // modelBuilder.Entity<UserEntity>()
    //     //     .HasIndex(u => u.Email)
    //     //     .IsUnique();
    // }
}
