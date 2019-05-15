using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blog.Data;
using Blog.Models;
using Blog.Models.ViewModel;
using Blog.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _host;
        IBlogService blog = new IBlogService();


        public PostController(ApplicationDbContext db, IHostingEnvironment host)
        {
            _host = host;
            _db = db;

        }
        public IActionResult Index()
        {
            
            return View(_db.Post.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Post model, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                
                var date = DateTime.UtcNow;
                model.Date = date;


                var fileName = file.FileName;
                int ids;
                if((ids = fileName.LastIndexOf("\\")) != -1) fileName = fileName.Substring(ids + 2, fileName.Length - 2 - ids);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }



                await _db.AddAsync(model);
                await _db.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            } 
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {

            if (id == null) return NotFound();
            
            var model = _db.Post.Find(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Post model, IFormFile file)
        {

            if (ModelState.IsValid)
            {
                var oldmodel = await _db.Post.SingleOrDefaultAsync(x => x.Id == id);

                try
                {
                    var fileName = file.FileName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", model.Id.ToString());
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                        
                    }
                    oldmodel.Photo = fileName;
                }
                catch(Exception ex) { }
                oldmodel.Context = model.Context;
                oldmodel.Title = model.Title;
                oldmodel.Date = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Details(int? id)
        {
            PostViewModel model = new PostViewModel();
            
            if (id != null)
            {
                var model1 = _db.Post.Find(id);
                model.Comments = _db.Commnet.Where(x => x.PostId == id).ToList();
                model.Post = model1;
                return View(model);
            }
            return NotFound();
        }
        
        public async Task<IActionResult> CreateCom(PostViewModel model, int Id)
        {
            var post = _db.Post.Find(Id);
            int i = model.Comment.Email.IndexOf("@");
            model.Comment.Author = model.Comment.Email.Substring(0, i);
            model.Comment.PubDate = DateTime.UtcNow;
            post.Comments.Add(model.Comment);
           // model.Comments.Reverse();
            await _db.SaveChangesAsync();
            return RedirectToAction("Details", "Post", new { id = Id });
        }



        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _db.Post
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        [HttpPost, ActionName("Delete")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _db.Post.FindAsync(id);
            int ids = category.Id;

            List<Comment> comments = _db.Commnet.Where(x => x.PostId == ids).ToList();

            foreach(var comment in comments)
            {
                 _db.Commnet.Remove(comment);
            }
           
            _db.Post.Remove(category);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));





        }

     
    }
}