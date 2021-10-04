using System;
using System.IO;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Assignment4.Entities;
using Microsoft.EntityFrameworkCore.Design;

namespace Assignment4
{
    class Program 
    {
        
        static void Main(string[] args)
        {
            var connectionString = "Server=.;Database=KanbanBoard;User Id=sa;Password=ad620d7a-a8d2-40d5-8540-1c8a8e4f62e6";
            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>().UseSqlServer(connectionString);
            using var context = new KanbanContext(optionsBuilder.Options);
            
            KanbanContextFactory.Seed(context);

            var chars = from c in context.Tags
                        where c.Id == 0
                        select c;
                       

            foreach (var c in chars)
            {
                Console.WriteLine(c.Id);

            }
        }   
        
    }
}
