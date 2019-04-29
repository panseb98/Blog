using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class Comment
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Author { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime PubDate { get; set; } = DateTime.UtcNow;

        public bool IsAdmin { get; set; }
    }
}
