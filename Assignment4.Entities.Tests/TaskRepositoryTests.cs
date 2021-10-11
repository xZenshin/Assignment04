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
            
            context.Users.AddRange(
                new User { Id=1, Name = "Bob",  Email = "bob@gmail.com"},
                new User { Id=2, Name = "Carl", Email = "carl1@gmail.com"},
                new User { Id=3, Name = "Ib",   Email = "ib@gmail.com"}, 
                new User { Id=4, Name = "Bo",   Email = "bo@gmail.com"}   
            );

            Tag todo =  new Tag {Name = "TODO"};
            Tag done = new Tag {Name = "Done"};

            context.Tags.AddRange(
                todo, done
            );
            
            context.Tasks.AddRange(
                new Task {Id = 1, Title = "Add Method", Description = "Please create a new method",  
                            Tags = new ReadOnlyCollection<Tag>(new Tag[]{todo}),
                            State = Core.State.Active},
                new Task {Id = 2, Title = "Remove unused code", Description = "Remove the code that is unused", 
                            Tags = new ReadOnlyCollection<Tag>(new Tag[]{done}), 
                            State = Core.State.Removed}
            ); 

            context.SaveChanges();

            _context = context;
            _repository = new TaskRepository(_context);
        }
        
        [Fact]
        public void Create_createsANewTaskDTO()
        {
            var task = new TaskCreateDTO{
                                    Title ="Task1", 
                                    AssignedToId = 1,
                                    Description="Description1", 
                                    Tags = new ReadOnlyCollection<string>(new string[]{"Done","TODO"}),                                    
            };
            
            var created = _repository.Create(task);
            Assert.Equal((Response.Created,3), created);
        }
        
        [Fact]
        public void ReadAll_returnsListOfAllTaskDTO()
        {
            var tasks = _repository.ReadAll();
            
            TaskDTO actual_t1 = tasks.First();
            TaskDTO actual_t2 = tasks.Skip(1).Take(1).First();
            
            Assert.Equal(1, actual_t1.Id);
            Assert.Equal("Add Method", actual_t1.Title);
            Assert.Null(actual_t1.AssignedToName);
            Assert.True(actual_t1.Tags.ToList().SequenceEqual(new [] {"TODO"}));
            Assert.Equal(State.Active, actual_t1.State);

            Assert.Equal(2, actual_t2.Id);
            Assert.Equal("Remove unused code", actual_t2.Title);
            Assert.Null(actual_t2.AssignedToName);
            Assert.True(actual_t2.Tags.ToHashSet().SetEquals(new [] {"Done"}));
            Assert.Equal(State.Removed, actual_t2.State);
        }

        [Fact]
        public void Read_returnsTaskWithID()
        {
            var task = _repository.Read(1);
            
            Assert.Equal(1, task.Id);
            Assert.Equal("Add Method", task.Title);
            Assert.Equal("Please create a new method", task.Description);
            Assert.Null(task.AssignedToName);
            Assert.True(task.Tags.ToList().SequenceEqual(new [] {"TODO"}));
            Assert.Equal(State.Active, task.State);
        }

        [Fact]
        public void ReadAllRemoved_returnsAllStatesRemoved()
        {
            var task = _repository.ReadAllRemoved().First();

            Assert.Equal(2, task.Id);
            Assert.Equal("Remove unused code", task.Title);
            Assert.Null(task.AssignedToName);
            Assert.True(task.Tags.ToList().SequenceEqual(new [] {"Done"}));
            Assert.Equal(State.Removed, task.State);
        }

        [Fact]
        public void ReadAllByTag_returnsTaskWithId()
        {
            var task = _repository.ReadAllByTag("Done").First();

            Assert.Equal(2, task.Id);
            Assert.Equal("Remove unused code", task.Title);
            Assert.Null(task.AssignedToName);
            Assert.True(task.Tags.ToList().SequenceEqual(new [] {"Done"}));
            Assert.Equal(State.Removed, task.State);
        }

        [Fact]
        public void ReadAllByUser_givenId2_returnsTaskWithId1()
        {
            TaskUpdateDTO t = new TaskUpdateDTO{
                Id = 1,
                Title = "task1",
                AssignedToId = 1,
                Description = "description1",
                Tags = new string[]{"TODO"},
                State = State.Closed,
            };

            _repository.Update(t);

            var task = _repository.ReadAllByUser(1).First();

            Assert.Equal(1, task.Id);
            Assert.Equal("task1", task.Title);
            Assert.Equal("Bob",task.AssignedToName);
            Assert.True(task.Tags.ToList().SequenceEqual(new [] {"TODO"}));
            Assert.Equal(State.Closed, task.State);            
        }

        [Fact]
        public void ReadAllByState_returns()
        {
            var task = _repository.ReadAllByState(State.Active).First();

            Assert.Equal(1, task.Id);
            Assert.Equal("Add Method", task.Title);
            Assert.Null(task.AssignedToName);
            Assert.True(task.Tags.ToList().SequenceEqual(new [] {"TODO"}));
            Assert.Equal(State.Active, task.State);
        }
        
        [Fact]
        public void Delete_givenTaskId_deletesTask()
        {
            var deleted = _repository.Delete(1);
            Assert.Equal(Response.Deleted, deleted);
        }

        [Fact]
        public void FindById_returnsNull()
        {
            var task = _repository.Read(4);
            Assert.Null(task);
        }

        [Fact]
        public void Update_returnsUpdated()
        {
            TaskUpdateDTO updatedTask = new TaskUpdateDTO {
                            Id = 1, Title = "Add Method", Description = "Please create a new method",  
                            Tags = new ReadOnlyCollection<string>(new string[]{"TODO"}),
                            State = Core.State.Closed
                            };
            
            var update = _repository.Update(updatedTask);
            Assert.Equal(Response.Updated, update);
        }

        [Fact]
        public void Update_returnsNotFound() 
        {
            TaskUpdateDTO updatedTask = new TaskUpdateDTO {Id = 27, Title = "Add Method", Description = "Please create a new method",  
                            Tags = new ReadOnlyCollection<string>(new string[]{"todo"}),
                            State = Core.State.Active};
            var update = _repository.Update(updatedTask);
            Assert.Equal(Response.NotFound, update);
        }

        // Tear down, deconstruction
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
