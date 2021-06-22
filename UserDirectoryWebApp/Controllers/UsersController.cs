using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UserDirectoryWebApp.Data.Contexts;
using UserDirectoryWebApp.Data.Entities;
using UserDirectoryWebApp.ViewModels;

namespace UserDirectoryWebApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsersController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index(string sortBy="name_asc")
        {
            return sortBy switch
            {
                "name_asc" => View(await _context.Users.OrderBy(x => x.Name).ToListAsync()),
                "name_desc" => View(await _context.Users.OrderByDescending(x => x.Name).ToListAsync()),
                "surname_asc" => View(await _context.Users.OrderBy(x => x.Surname).ToListAsync()),
                "surname_desc" => View(await _context.Users.OrderByDescending(x => x.Surname).ToListAsync()),
                _ => View(await _context.Users.OrderBy(x => x.Name).ToListAsync()),
            };
        }

        

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,Email,BirthDate,Phone,ImageFile")] User user)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = CheckUploadedImage(user);
                user.UserImageName = uniqueFileName;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var vm = new UserVM
            {
                User = user,
                DeleteImage = "0"
            };
            return View(vm);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserVM userVM)
        {
            if (id != userVM.User.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userInDb = await _context.Users.FindAsync(id);
                    if (userVM.User.ImageFile == null)
                    {
                        if (userVM.DeleteImage == "1")
                        {
                            DeleteImage(userInDb);
                            userVM.User.UserImageName = null;
                        }
                        else
                        {
                            userVM.User.UserImageName = userInDb.UserImageName;
                        }

                    }
                    else
                    {             
                        string uniqueFileName = CheckUploadedImage(userVM.User);
                        DeleteImage(userInDb);
                        userVM.User.UserImageName = uniqueFileName;
                    };
                    _context.Entry(userInDb).State = EntityState.Detached;
                    _context.Update(userVM.User);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(userVM.User.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(userVM);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            DeleteImage(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private string CheckUploadedImage(User user)
        {
            string uniqueFileName = null;

            if (user.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "image");
                string ufileName = Path.GetFileNameWithoutExtension(user.ImageFile.FileName);
                string ufileExtension = Path.GetExtension(user.ImageFile.FileName);
                uniqueFileName = user.Id + "_" + user.Name + "_" + ufileName + "_" + DateTime.Now.ToString("ddMMyyyy") + ufileExtension;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                user.ImageFile.CopyTo(fileStream);
            }

            return uniqueFileName;
        }


        private void DeleteImage(User user)
        {
            if (user.UserImageName != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "image");
                string filePath = Path.Combine(uploadsFolder, user.UserImageName);
                System.IO.File.Delete(filePath);
            }
        }
    }
}
