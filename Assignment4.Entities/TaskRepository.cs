using System.Collections.Generic;
using Assignment4.Core;
using System.Linq;
using System.Collections.ObjectModel;
using System;
namespace Assignment4.Entities
{
    public class TaskRepository : ITaskRepository
    {
        private readonly KanbanContext _context;
        
        public  TaskRepository(KanbanContext context)
        {
            _context = context;
        }  
        public (Response Response, int TaskId) Create(TaskCreateDTO task)
        {
            List<Tag> tags = new List<Tag>();
            foreach (string tagName in task.Tags)
            {
                tags.Add((from t in _context.Tags where tagName == t.Name select t).First());    
            }

             var entity = new Task
            {
                Title = task.Title,
                AssignedTo = (from u in _context.Users where u.Id == task.AssignedToId select u).First(),
                Description = task.Description,
                Created = DateTime.UtcNow,
                State = Core.State.New,
                Tags = tags,
                StateUpdated = DateTime.UtcNow
            };

            _context.Tasks.Add(entity);

            _context.SaveChanges();

            return (Response.Created, entity.Id);
        }
        
         public IReadOnlyCollection<TaskDTO> ReadAll() => _context.Tasks.
            Select(t => new TaskDTO(t.Id, t.Title, t.AssignedTo.Name, (from tag in t.Tags select tag.Name).ToList(), t.State)).ToList().AsReadOnly();

        public TaskDetailsDTO Read(int taskId)
        {
            var tasks = from t in _context.Tasks
                             where t.Id == taskId
                             select new TaskDetailsDTO(
                                 t.Id,
                                 t.Title,
                                 t.Description,
                                 t.Created,
                                 t.AssignedTo.Name,
                                 t.Tags.Select(x => x.Name).ToList().AsReadOnly(),
                                 t.State,
                                 t.StateUpdated
                             );
                             
            // If null returns null
            return tasks.FirstOrDefault();
        }
        
        public IReadOnlyCollection<TaskDTO> ReadAllRemoved()=> _context.Tasks.Where(x => x.State == State.Removed).
            Select(t => new TaskDTO(t.Id, t.Title, t.AssignedTo.Name, (from tag in t.Tags select tag.Name).ToList(), t.State)).ToList().AsReadOnly();
        
        public IReadOnlyCollection<TaskDTO> ReadAllByTag(string tag)
        {
            return _context.Tasks.Where(tasks => tasks.Tags.Any(t=> t.Name == tag)).
            Select(t => new TaskDTO(t.Id, t.Title, t.AssignedTo.Name, (from tag in t.Tags select tag.Name).ToList(), t.State)).ToList().AsReadOnly();

        }
        
        public IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId) => _context.Tasks.Where(x => x.AssignedTo.Id == userId).
            Select(t => new TaskDTO(t.Id, t.Title, t.AssignedTo.Name, (from tag in t.Tags select tag.Name).ToList(), t.State)).ToList().AsReadOnly();
        
        public IReadOnlyCollection<TaskDTO> ReadAllByState(State state) => _context.Tasks.Where(x => x.State == state).
            Select(t => new TaskDTO(t.Id, t.Title, t.AssignedTo.Name, (from tag in t.Tags select tag.Name).ToList(), t.State)).ToList().AsReadOnly();
        
        public Response Update(TaskUpdateDTO task)
        {
            var _task = _context.Tasks.Find(task.Id);
            
            if(_task == null) return Response.NotFound;
           
            var user = (from u in _context.Users where u.Id == task.AssignedToId select u).FirstOrDefault();
            
            if (task.AssignedToId != null)
            {
                if (user == null) return Response.BadRequest;   
            } 
            
            List<Tag> tags = new List<Tag>();
            foreach (string tagName in task.Tags)
            {
                tags.Add((from t in _context.Tags where tagName == t.Name select t).First());    
            }

            _task.Id = task.Id;
            _task.Title = task.Title;
            _task.AssignedTo = user;
            _task.Description = task.Description;
            _task.Tags = tags;
            _task.State = task.State;
            _task.StateUpdated = DateTime.UtcNow;

            _context.SaveChanges();

            return Response.Updated;
        }

         public Response Delete(int taskId)
        {
            var entity = _context.Tasks.Find(taskId);

            if (entity == null) return Response.NotFound;

            _context.Tasks.Remove(entity);
            _context.SaveChanges();
            
            return Response.Deleted;
        }
    }
}
