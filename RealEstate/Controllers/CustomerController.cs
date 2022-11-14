using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Models;

namespace RealEstate.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        List<UserAppModel> listusers = new List<UserAppModel>();

        public CustomerController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Customer
        public async Task<IActionResult> Index()
        {
            var userdb = await _context.User.ToListAsync();

            foreach (var user in userdb)
            {
                UserAppModel userapp = new UserAppModel();
                userapp.id = user.id;
                userapp.Email = user.email;
                if (user.role == 1)
                    userapp.Role = "Agent";
                else
                    userapp.Role = "Seller";
                userapp.Name = user.name;
                userapp.Phone = user.phone;

                listusers.Add(userapp);
            }
                
            return View(listusers);
        }

        // GET: Customer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserAppModel model)
        {
            if (ModelState.IsValid)
            {
                //IdentityUser 
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                };

                // DB User
                User dbuser = new User();

                // add IdentityUser to DB
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    dbuser.email = model.Email;
                    dbuser.password = model.Password;
                    dbuser.name = model.Email;  // name = email
                    dbuser.phone = model.Phone;

                    if (model.Role == "1")
                    {
                        dbuser.role = 1;  // add role to DB User
                        // add role to IdentityUser
                        IdentityResult roleresult = await _userManager.AddToRoleAsync(user, "Agent");                        
                    }
                    else // for role = Seller
                    {
                        dbuser.role = 2;
                        IdentityResult roleresult = await _userManager.AddToRoleAsync(user, "Seller");
                    }

                    // Add DB user to User table
                    _context.Add<User>(dbuser);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }

            return View(model);
        }

        // GET: Customer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.id == id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.role == 1)
                ViewBag.role = "Agent";
            else
                ViewBag.role = "Seller";

            return View(user);
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            UserAppModel userapp = new UserAppModel();
            userapp.id = user.id;
            userapp.Email = user.email;
            userapp.Password = user.password;
            if (user.role == 1)
                userapp.Role = "Agent";
            else
                userapp.Role = "Seller";
            userapp.Name = user.name;
            userapp.Phone = user.phone;

            return View(userapp);
        }       

        // POST: Customer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserAppModel userapp)
        {
            if (id != userapp.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userapp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(userapp.id))
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
            return View(userapp);
        }

        // GET: Customer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'ApplicationDbContext.User'  is null.");
            }
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                _context.User.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
          return _context.User.Any(e => e.id == id);
        }
    }
}
