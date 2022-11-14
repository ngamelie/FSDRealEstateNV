using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Data;
using RealEstate.Models;
using System;

namespace RealEstate.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationDbContext _context;

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AccountController(
           ApplicationDbContext context,
           UserManager<IdentityUser> userManager,
           SignInManager<IdentityUser> signInManager,
           ILogger<AccountController> logger,
           RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
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
                    _logger.LogInformation("User created a new account with password.");
                    
                    dbuser.email = model.Email;
                    dbuser.password = model.Password;
                    dbuser.name = model.Email;  // name = email
                    dbuser.phone = user.PhoneNumber;

                    if (model.Email.Contains("agent")) {
                        // check role added when home page load
                        var role = await this._roleManager.FindByNameAsync("Agent");
                        
                        dbuser.role = 1;  // add role to DB User
                        if (role != null)
                        {
                            // add role to IdentityUser
                            IdentityResult roleresult = await _userManager.AddToRoleAsync(user, role.Name);
                        }                        
                    }
                    else // for role = Seller
                    {
                        dbuser.role = 2;
                        IdentityResult roleresult = await _userManager.AddToRoleAsync(user, "Seller");
                    }

                    // Add DB user to User table
                    _context.Add<User>(dbuser);
                    _context.SaveChanges();

                    return RedirectToAction("RegisterSuccess", new { email = model.Email });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, false, false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    
                    IdentityUser u = await _userManager.FindByEmailAsync(user.Email);
                    
                    if (await _userManager.IsInRoleAsync(u, "Agent"))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else if (await _userManager.IsInRoleAsync(u, "Seller"))
                    {
                        return RedirectToAction("Index", "Home");
                    }                  
                }                

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }
            return View(user);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult RegisterSuccess(string email)
        {
            return View((object)email);
        }
    }
}
