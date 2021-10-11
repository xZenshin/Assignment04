using System.Collections.Generic;
using Assignment4.Core;
using System.Linq;

namespace Assignment4.Entities
{
    public class UserRepository : IUserRepository
    {
        private readonly KanbanContext _context;

        public UserRepository(KanbanContext context)
        {
            _context = context;
        }
        public (Response Response, int UserId) Create(UserCreateDTO user)
        {
            var emails = _context.Users.Select(u => u.Email);
            if (emails.Contains(user.Email)) return (Response.Conflict, 0);
            
            var entity = new User
            {
                Name = user.Name,
                Email = user.Email
            };
            _context.Users.Add(entity);
            _context.SaveChanges();
            
            return (Response.Created, entity.Id);
        }

        public Response Delete(int userId, bool force = false)
        {
            var user = _context.Users.Find(userId);

            if (user.Tasks != null)
            {
                if (force)
                {
                    if (user == null) return Response.NotFound;

                    _context.Users.Remove(user);
                    _context.SaveChanges();
                    
                    return Response.Deleted;
                }
                else
                {
                    return Response.Conflict;
                }
            }
            _context.Users.Remove(user);
            _context.SaveChanges();
                    
            return Response.Deleted;
        }

        public UserDTO Read(int userId)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<UserDTO> ReadAll()
        {
            throw new System.NotImplementedException();
        }

        //Complete and tested
        public Response Update(UserUpdateDTO user)
        {
            var _user = _context.Users.Find(user.Id);
            
            if(_user == null) return Response.NotFound;
            
            var emails = _context.Users.Select(u => u.Email);
            if (emails.Contains(user.Email)) return Response.Conflict;

            _user.Id = user.Id;
            _user.Name = user.Name;
            _user.Email = user.Email;

            _context.SaveChanges();

            return Response.Updated;
        }
    }
}