using Microsoft.EntityFrameworkCore;

using WebApplication4.Models;


namespace WebApplication4.Data
{
    public class VeriTabani
    {public class FilmContext : DbContext
        {


            public FilmContext(DbContextOptions<FilmContext> options) : base(options) { }
            public DbSet<Tablo1> tablo1s { get; set; }
           
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                
            }
            
        }
    }
}
