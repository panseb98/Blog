using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class CommentController : Controller
    {
        ApplicationDbContext _db;

        public CommentController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Create(Comment model, int id)
        {
            var post = _db.Post.Find(id);
            model.PubDate = DateTime.UtcNow;
            post.Comments.Add(model);
            await _db.SaveChangesAsync();
            return View();
        }
    }
}