using System;
using System.Collections.Generic;

namespace MoolahApi.Models.DTOs
{
    public class CreateTodoRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public string Type { get; set; } = TodoTypes.Personal;
    }

    public class UpdateTodoRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public string Type { get; set; } = TodoTypes.Personal;
    }
    
    public class TodoOrderItem
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Type { get; set; } = TodoTypes.Personal;
    }
} 