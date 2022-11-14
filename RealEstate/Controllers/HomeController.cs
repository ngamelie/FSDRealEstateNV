using BingMapsRESTToolkit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using RealEstate.Models;
using System.Diagnostics;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.Serialization.Json;
using System.Text;
using Location = BingMapsRESTToolkit.Location;

namespace RealEstate.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(ILogger<HomeController> logger, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _roleManager = roleManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IdentityResult createUserRole = await _roleManager.CreateAsync(new IdentityRole("Seller"));
            IdentityResult createAdminRole = await _roleManager.CreateAsync(new IdentityRole("Agent"));
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}