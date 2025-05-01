using System.Collections.Generic;
using System.Threading.Tasks;
using MoolahApi.Models;
using MoolahApi.Models.DTOs;

namespace MoolahApi.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<Todo>> GetAllTodosAsync();
        Task<Todo?> GetTodoByIdAsync(int id);
        Task<Todo> CreateTodoAsync(Todo todo);
        Task<Todo?> UpdateTodoAsync(int id, Todo todo);
        Task<bool> DeleteTodoAsync(int id);
        Task<IEnumerable<Todo>> SearchTodosAsync(string searchTerm);
        Task<bool> ReorderTodosAsync(IEnumerable<TodoOrderItem> todoOrders);
    }
} 