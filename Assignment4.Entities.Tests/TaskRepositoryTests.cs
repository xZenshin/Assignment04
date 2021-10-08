using System;
using Xunit;
using Assignment4.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Assignment4.Entities.Tests
{
    public class TaskRepositoryTests 
    {

        private readonly KanbanContext _context;
        private readonly TaskRepository _repository;

        public TaskRepositoryTests()
        {

        }


        [Fact]
        public void All_returnsListOfAllTaskDTO()
        {
            
            var connectionString = "Server=.;Database=KanbanBoard;User Id=sa;Password=ad620d7a-a8d2-40d5-8540-1c8a8e4f62e6";

            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>().UseSqlServer(connectionString);
            using var context = new KanbanContext(optionsBuilder.Options);
           
            //var output = taskRepo.All();
        }

        [Fact]
        public void Create_createsANewTaskDTO()
        {
            var connectionString = "Server=.;Database=KanbanBoard;User Id=sa;Password=b9ce16cc-1165-451b-88a8-76b2917551af";

            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>().UseSqlServer(connectionString);
            using var context = new KanbanContext(optionsBuilder.Options);
            TaskRepository taskRepo = new TaskRepository(context);

            var testTaskDTO = new TaskDTO{Title ="Task1", 
                                    Description="Description1", 
                                    Tags = new ReadOnlyCollection<string>(new string[]{"tag1","tag2"}),
                                    State = State.New};

            var actualId = taskRepo.Create(testTaskDTO);

            var expectedId = (from c in context.Tasks
                           where c.Id == testTaskDTO.Id
                           select c.Id).First();
           
           //Assuming that Id's are uniqe
            Assert.Equal(expectedId,actualId);

             var remove = (from c in context.Tasks
                           where c.Id == testTaskDTO.Id
                           select c).First();

             context.Tasks.Remove(remove);   
        }

        [Fact]
        public void Delete_givenTaskId_deletesTask()
        {
            var connectionString = "Server=.;Database=KanbanBoard;User Id=sa;Password=b9ce16cc-1165-451b-88a8-76b2917551af";
            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>().UseSqlServer(connectionString);
            using var context = new KanbanContext(optionsBuilder.Options);
            TaskRepository taskRepo = new TaskRepository(context);

            var testTaskDTO = new TaskDTO{Title ="Task1", 
                                    Description="Description1", 
                                    Tags = new ReadOnlyCollection<string>(new string[]{"tag1","tag2"}),
                                    State = State.New};

            var id = taskRepo.Create(testTaskDTO);

            taskRepo.Delete(id);

            var listoftasks = (from t in context.Tasks select t).ToList();

            var task = (from c in context.Tasks
                           where c.Id == testTaskDTO.Id
                           select c).First();

            Assert.DoesNotContain(task,listoftasks);

        }
    }
}
