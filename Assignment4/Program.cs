using System;
using System.IO;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Assignment4.Entities;

namespace Assignment4
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = LoadConfiguration();
            var connectionString = "Server=localhost;Database=KanbanBoard;User Id=sa;Password=ad620d7a-a8d2-40d5-8540-1c8a8e4f62e6";
        }
        static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
                
            return builder.Build();
        }
    }
}
