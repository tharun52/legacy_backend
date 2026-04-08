using System;
using System.ComponentModel.DataAnnotations;

namespace BlogApi.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Author { get; set; } = "Anonymous";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int PostId { get; set; }
        public Post? Post { get; set; }
    }
}
