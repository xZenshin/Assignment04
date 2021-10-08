using System;
using Xunit;
using Assignment4.Core;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Assignment4.Entities.Tests
{
    public class TaskRepositoryTests  : IDisposable
    {

        private readonly KanbanContext _context;
        private readonly TaskRepository _repository;

        public TaskRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<KanbanContext>();
            builder.UseSqlite(connection);
            var context = new KanbanContext(builder.Options);
            context.Database.EnsureCreated();


            /* INSERT TEST DATA HERE */

            context.SaveChanges();

            _context = context;
            _repository = new TaskRepository(_context);

        }


        [Fact]
        public void All_returnsListOfAllTaskDTO()
        {
            
            
        }

        [Fact]
        public void Create_createsANewTaskDTO()
        {
            
            var testTaskDTO = new TaskDTO{
                                    Title ="Task1", 
                                    Description="Description1", 
                                    Tags = new ReadOnlyCollection<string>(new string[]{"tag1","tag2"}),
                                    State = State.New
                                    };

        }

        [Fact]
        public void Delete_givenTaskId_deletesTask()
        {
            
            var testTaskDTO = new TaskDTO{
                                    Title ="Task1", 
                                    Description="Description1", 
                                    Tags = new ReadOnlyCollection<string>(new string[]{"tag1","tag2"}),
                                    State = State.New
                                    };

            
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
