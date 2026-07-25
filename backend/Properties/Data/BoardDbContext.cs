using Microsoft.EntityFrameworkCore;
using BoardApi.Models;

namespace BoardApi.Data;

public class BoardDbContext : DbContext
{
    public BoardDbContext(DbContextOptions<BoardDbContext> options) : base(options)
    {
        
    }    
    public DbSet<Post> Posts => Set<Post>();
}