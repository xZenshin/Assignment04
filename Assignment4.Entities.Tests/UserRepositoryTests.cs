using System;
using System.Collections.ObjectModel;
using Assignment4.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Assignment4.Entities.Tests
{
    public class UserRepositoryTests
    {
        private readonly KanbanContext _context;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
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
            _repository = new UserRepository(_context);
        }

        [Fact]
        public void Create_returnsTupleOfResponseAndUserId()
        {
            var user = new UserCreateDTO{ Name = "User1", Email = "User1@mail.com"};
            var created = _repository.Create(user);

            Assert.Equal((Response.Created,5),created);
        }

        [Fact]
        public void Create_givenSameEmail_returnsConflict()
        {
            var user = new UserCreateDTO{ Name = "User1", Email = "bob@gmail.com"};
            var created = _repository.Create(user);

            Assert.Equal((Response.Conflict, 0), created);
        }
        
        [Fact]
        public void Delete_givenUserWithoutTasks_returnsDeleted() 
        {
            var deleted = _repository.Delete(1);
            Assert.Equal(Response.Deleted, deleted);
        }

        [Fact]
        public void Delete_givenUserWithTaskAndForce()
        {
        //Given
        
        //When
        
        //Then
        }
        [Fact]
        public void Delete_givenUserWithTaskAndWithoutForce()
        {
        //Given
        
        //When
        
        //Then
        }


        [Fact]
        public void Read_returns_userDTO_with_id_corresponding_to_input()
        {
            
        }

        [Fact]
        public void Update_returnsResponseAndUpdatesUserFromUserUpdateDTO()
        {
            UserUpdateDTO updatedTask = new UserUpdateDTO {
                Name = "user1",
                Email = "user1@gmail.com",
                Id = 2
            };
            
            var update = _repository.Update(updatedTask);
            Assert.Equal(Response.Updated, update);
        }
        [Fact]
        public void Update_givenExistingEmail_ReturnsConflict()
        {
            UserUpdateDTO updatedTask = new UserUpdateDTO {
                Name = "user1",
                Email = "bob@gmail.com",
                Id = 1
            };
            var update = _repository.Update(updatedTask);
            Assert.Equal(Response.Conflict, update);
        }

        [Fact]
        public void Delete_returnsResponseAndDeletesUser()
        {
            
        }
    }
}
