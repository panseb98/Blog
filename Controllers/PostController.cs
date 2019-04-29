using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blog.Data;
using Blog.Models;
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


        public PostController(ApplicationDbContext db, IHostingEnvironment host)
        {
            _host = host;
            _db = db;

        }
        public IActionResult Index()
        {
            return View(_db.Post.ToList());
        }

        public async Task<IActionResult> Create(Post model, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                
                var date = DateTime.UtcNow;
                model.Date = date;
                
                    var fileName = file.FileName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    model.Photo = fileName;

                
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
                oldmodel.Context = model.Context;
                oldmodel.Title = model.Title;
                oldmodel.Date = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
            return View(model);
        }

        public IActionResult Details(int? id)
        {
            
            if (id != null)
            {
                var model = _db.Post.Find(id);
                
                return View(model);
            }
            return NotFound();
        }
      
    }
}