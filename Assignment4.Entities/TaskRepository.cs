using System.Collections.Generic;
using Assignment4.Core;
using System.Linq;
using System.Collections.ObjectModel;

namespace Assignment4.Entities
{
    public class TaskRepository : ITaskRepository
    {
        private readonly KanbanContext _context;
        
        public  TaskRepository(KanbanContext context)
        {
            _context = context;
        }  

        public IReadOnlyCollection<TaskDTO> All()
        {
           List<TaskDTO> readonlyTasks = new List<TaskDTO>();
           
          var listoftasks = (from t in _context.Tasks select t).ToList();
            
            foreach (var task in listoftasks)
            {
                
                var tasktags = (from n in task.Tags select n.Name).ToList();
                TaskDTO tempTaskDTO;
                if(task.AssignedTo != null)
                {
                        tempTaskDTO = new TaskDTO {             
                                        Id = task.Id, 
                                        Title = task.Title, 
                                        Description = task.Description, 
                                        AssignedToId=task.AssignedTo.Id, 
                                        Tags = tasktags,
                                        State = task.State 
                                        };

                }
                else
                {
                    tempTaskDTO = new TaskDTO {             
                                        Id = task.Id, 
                                        Title = task.Title, 
                                        Description = task.Description, 
                                        Tags = tasktags,
                                        State = task.State 
                                        };
                }

                readonlyTasks.Add(tempTaskDTO);
                
            }
           return new ReadOnlyCollection<TaskDTO>(readonlyTasks); 
        }

        public int Create(TaskDTO task)
        {
            User user = (from u in _context.Users
                        where u.Id == task.AssignedToId
                        select u).FirstOrDefault();

            var tags = _context.Tags.Where(s=>task.Tags.All(s2 => s2 != s.Name)).ToList();
                  
            Task taskInput = new Task{
                                Title=task.Title,
                                Description=task.Description,
                                AssignedTo = user,
                                Tags = tags,
                                State = task.State};    
                                
             _context.Tasks.Add(taskInput);   

            return taskInput.Id;
        }

        public void Delete(int taskId)
        {
            var task = (from t in _context.Tasks where t.Id == taskId select t).FirstOrDefault();
            _context.Tasks.Remove(task);
        }

        public void Dispose()
        {
            var tasks = (from t in _context.Tasks select t).ToList();
            _context.Tasks.RemoveRange(tasks);
        }

        public TaskDetailsDTO FindById(int id)
        {
                var task = (from t in _context.Tasks where t.Id == id select t).FirstOrDefault();
                TaskDetailsDTO taskdetails;

                string username = task.AssignedTo.Name;
                string useremail = task.AssignedTo.Email;
                var tasktags = from n in task.Tags select n.Name;
                
                if(task.AssignedTo != null)
                {
                    int userid = task.AssignedTo.Id;
                    taskdetails = new TaskDetailsDTO{Id = task.Id, 
                                                    Title = task.Title, 
                                                    Description = task.Description, 
                                                    AssignedToId=userid, 
                                                    AssignedToName = username, 
                                                    AssignedToEmail = useremail, 
                                                    Tags = tasktags,
                                                    State = task.State };
                    return taskdetails;
                }
            taskdetails = new TaskDetailsDTO{Id = task.Id, 
                                                Title = task.Title, 
                                                Description = task.Description,  
                                                AssignedToName = username,                                                     AssignedToEmail = useremail, 
                                                Tags = tasktags,
                                                State = task.State };
            return taskdetails;

        }

        public void Update(TaskDTO task)
        {
            var _task = (from t in _context.Tasks where t.Id==task.Id select t).FirstOrDefault();
            if(_task == null)
            {
                Create(task);
                return;
            }
            _context.Tasks.Update(_task);
        }
    }
}
