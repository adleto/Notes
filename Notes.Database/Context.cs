using Microsoft.EntityFrameworkCore;
using Notes.Database.Entities;

namespace Notes.Database
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) {}
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Note> Note { get; set; }
    }
}
