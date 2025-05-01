using System;

namespace MoolahApi.Models
{
    public class Todo
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string Type { get; set; } = TodoTypes.Personal;
        public int Order { get; set; } = 0;

        public int UserId { get; set; }

        public virtual User? User { get; set; }
    }

    public static class TodoTypes
    {
        public const string Personal = "PERSONAL";
        public const string Work = "WORK";

        public static readonly string[] AllTypes = { Personal, Work };
    }
} 