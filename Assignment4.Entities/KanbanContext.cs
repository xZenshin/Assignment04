using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Assignment4.Core;

namespace Assignment4.Entities
{
    public class KanbanContext: DbContext
    {

         public DbSet<Task> Cities { get; set; }
        public DbSet<Tag> Powers { get; set; }
        public DbSet<User> Characters { get; set; }

        public KanbanContext(DbContextOptions<KanbanContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Task>()
                .Property(e => e.State)
                .HasConversion<string>();
        }
    }
}
