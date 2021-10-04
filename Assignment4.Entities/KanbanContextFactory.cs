using System;
using System.IO;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Assignment4.Entities;
using Microsoft.EntityFrameworkCore.Design;

namespace Assignment4.Entities
{
    public class KanbanContextFactory : IDesignTimeDbContextFactory<KanbanContext>
    {
        public KanbanContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
                
                

            var connectionString = "Server=.;Database=KanbanBoard;User Id=sa;Password=ad620d7a-a8d2-40d5-8540-1c8a8e4f62e6";

            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>()
                .UseSqlServer(connectionString);

            return new KanbanContext(optionsBuilder.Options);
        }

        
         public static void Seed(KanbanContext context)
        {
            context.Database.ExecuteSqlRaw("DELETE dbo.Users");
            context.Database.ExecuteSqlRaw("DELETE dbo.Tasks");
            context.Database.ExecuteSqlRaw("DELETE dbo.Tags");
            context.Database.ExecuteSqlRaw("DELETE dbo.TagTask");
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('dbo.Users', RESEED, 0)");
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('dbo.Tasks', RESEED, 0)");
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('dbo.Tags', RESEED, 0)"); 
 
           

            var Bob = new User {Name = "Bob",Email = "bob@gmail.com"};
            var Carl = new User {Name = "Carl",Email = "carl1@gmail.com"};
            var sa = new User {Name = "Carl",Email = "carl2@gmail.com"};   
            var d = new User {Name = "Carl",Email = "carl3@gmail.com"};       

            var addMethod = new Task { Title = "Add Method", Description = "Please create a new method", State = Core.State.Active};    
            var removeUnusedCode = new Task { Title = "Remove unused code", Description = "Remove the code that is unused", State = Core.State.New};
            
            context.Users.AddRange(Bob,Carl,d,sa);

           
            context.Tags.AddRange(
             new Tag {Name = "TODO"},
             new Tag {Name = "Done"}
            );
            
            context.Tasks.AddRange(addMethod,removeUnusedCode);

            context.SaveChanges();
        }
    }
}