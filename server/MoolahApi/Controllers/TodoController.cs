using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoolahApi.Models;
using MoolahApi.Models.DTOs;
using MoolahApi.Services;

namespace MoolahApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TodoController : ControllerBase
    {
        private readonly TodoServiceFactory _serviceFactory;

        public TodoController(TodoServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null)
            {
                throw new InvalidOperationException("User ID claim not found");
            }
            return int.Parse(userIdClaim.Value);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodos([FromQuery] string? type = null)
        {
            var todoType = type ?? TodoTypes.Personal;
            var userId = GetUserId();
            var service = _serviceFactory.CreateService(todoType, userId);
            var todos = await service.GetAllTodosAsync();
            return Ok(todos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Todo>> GetTodo(int id, [FromQuery] string? type = null)
        {
            var todoType = type ?? TodoTypes.Personal;
            var userId = GetUserId();
            var service = _serviceFactory.CreateService(todoType, userId);
            var todo = await service.GetTodoByIdAsync(id);
            if (todo == null)
                return NotFound();
            return Ok(todo);
        }

        [HttpPost]
        public async Task<ActionResult<Todo>> CreateTodo(CreateTodoRequest request)
        {
            if (string.IsNullOrEmpty(request.Type) || !TodoTypes.AllTypes.Contains(request.Type))
            {
                request.Type = TodoTypes.Personal;
            }
            
            var userId = GetUserId();
            var todo = new Todo
            {
                Title = request.Title,
                Description = request.Description,
                IsCompleted = request.IsCompleted,
                Type = request.Type,
                UserId = userId
            };
            
            ModelState.Clear();
            
            var service = _serviceFactory.CreateService(request.Type, userId);
            var createdTodo = await service.CreateTodoAsync(todo);
            return CreatedAtAction(nameof(GetTodo), new { id = createdTodo.Id }, createdTodo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, UpdateTodoRequest request)
        {
            if (string.IsNullOrEmpty(request.Type) || !TodoTypes.AllTypes.Contains(request.Type))
            {
                request.Type = TodoTypes.Personal;
            }
            
            var userId = GetUserId();
            var todo = new Todo
            {
                Id = id,
                Title = request.Title,
                Description = request.Description,
                IsCompleted = request.IsCompleted,
                Type = request.Type,
                UserId = userId
            };
            
            ModelState.Clear();
            
            var service = _serviceFactory.CreateService(request.Type, userId);
            var updatedTodo = await service.UpdateTodoAsync(id, todo);
            if (updatedTodo == null)
                return NotFound();
            return Ok(updatedTodo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id, [FromQuery] string? type = null)
        {
            var todoType = type ?? TodoTypes.Personal;
            var userId = GetUserId();
            var service = _serviceFactory.CreateService(todoType, userId);
            var result = await service.DeleteTodoAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Todo>>> SearchTodos([FromQuery] string term, [FromQuery] string? type = null)
        {
            var todoType = type ?? TodoTypes.Personal;
            var userId = GetUserId();
            var service = _serviceFactory.CreateService(todoType, userId);
            var todos = await service.SearchTodosAsync(term);
            return Ok(todos);
        }

        [HttpPost("reorder")]
        public async Task<IActionResult> ReorderTodos(List<TodoOrderItem> orderItems)
        {
            if (orderItems == null || !orderItems.Any())
                return BadRequest("No order items provided");
            
            var type = orderItems.First().Type;
            if (string.IsNullOrEmpty(type) || !TodoTypes.AllTypes.Contains(type))
            {
                type = TodoTypes.Personal;
            }
            
            var userId = GetUserId();
            var service = _serviceFactory.CreateService(type, userId);
            var result = await service.ReorderTodosAsync(orderItems);
            
            if (!result)
                return NotFound("No todos found to reorder");
                
            return Ok("Todos reordered successfully");
        }
    }
} 