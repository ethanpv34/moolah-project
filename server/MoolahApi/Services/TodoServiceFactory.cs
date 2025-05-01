using MoolahApi.Data;
using MoolahApi.Models;

namespace MoolahApi.Services
{
    public class TodoServiceFactory
    {
        private readonly TodoDbContext _context;

        public TodoServiceFactory(TodoDbContext context)
        {
            _context = context;
        }

        public ITodoService CreateService(string type, int userId)
        {
            if (!TodoTypes.AllTypes.Contains(type))
            {
                type = TodoTypes.Personal;
            }

            return new TodoService(_context, type, userId);
        }
    }
}