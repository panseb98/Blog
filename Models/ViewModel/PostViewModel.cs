using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models.ViewModel
{
    public class PostViewModel
    {
        public Post Post { get; set; }
        public Comment Comment { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
