using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoolahApi.Data;
using MoolahApi.Models;
using MoolahApi.Models.DTOs;

namespace MoolahApi.Services
{
    public class TodoService : ITodoService
    {
        private readonly TodoDbContext _context;
        private readonly string _todoType;
        private readonly int _userId;

        public TodoService(TodoDbContext context, string todoType, int userId)
        {
            _context = context;
            _todoType = todoType;
            _userId = userId;
        }

        public async Task<IEnumerable<Todo>> GetAllTodosAsync()
        {
            return await _context.Todos
                .Where(t => t.Type == _todoType && t.UserId == _userId)
                .OrderBy(t => t.Order)
                .ToListAsync();
        }

        public async Task<Todo?> GetTodoByIdAsync(int id)
        {
            return await _context.Todos
                .Where(t => t.Type == _todoType && t.Id == id && t.UserId == _userId)
                .FirstOrDefaultAsync();
        }

        public async Task<Todo> CreateTodoAsync(Todo todo)
        {
            todo.CreatedAt = DateTime.UtcNow;
            todo.Type = _todoType;
            todo.UserId = _userId;
            
            var maxOrder = await _context.Todos
                .Where(t => t.Type == _todoType && t.UserId == _userId)
                .Select(t => (int?)t.Order)
                .MaxAsync() ?? -1;
            
            todo.Order = maxOrder + 1;
            
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();
            return todo;
        }

        public async Task<Todo?> UpdateTodoAsync(int id, Todo todo)
        {
            var existingTodo = await _context.Todos
                .Where(t => t.Type == _todoType && t.Id == id && t.UserId == _userId)
                .FirstOrDefaultAsync();
            
            if (existingTodo == null)
                return null;

            existingTodo.Title = todo.Title;
            existingTodo.Description = todo.Description;
            existingTodo.IsCompleted = todo.IsCompleted;

            if (todo.IsCompleted && !existingTodo.CompletedAt.HasValue)
                existingTodo.CompletedAt = DateTime.UtcNow;
            else if (!todo.IsCompleted)
                existingTodo.CompletedAt = null;

            await _context.SaveChangesAsync();
            return existingTodo;
        }

        public async Task<bool> DeleteTodoAsync(int id)
        {
            var todo = await _context.Todos
                .Where(t => t.Type == _todoType && t.Id == id && t.UserId == _userId)
                .FirstOrDefaultAsync();
                
            if (todo == null)
                return false;

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Todo>> SearchTodosAsync(string searchTerm)
        {
            return await _context.Todos
                .Where(t => t.Type == _todoType && t.UserId == _userId &&
                      (t.Title.Contains(searchTerm) || t.Description.Contains(searchTerm)))
                .OrderBy(t => t.Order)
                .ToListAsync();
        }

        public async Task<bool> ReorderTodosAsync(IEnumerable<TodoOrderItem> todoOrders)
        {
            var todoIds = todoOrders.Select(t => t.Id).ToList();
            var todos = await _context.Todos
                .Where(t => t.Type == _todoType && t.UserId == _userId && todoIds.Contains(t.Id))
                .ToListAsync();

            if (!todos.Any())
                return false;

            foreach (var todo in todos)
            {
                var orderItem = todoOrders.FirstOrDefault(t => t.Id == todo.Id);
                if (orderItem != null)
                {
                    todo.Order = orderItem.Order;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
} 