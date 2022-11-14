using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingMapsRESTToolkit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Models;

namespace RealEstate.Controllers
{
    public class AllPropertiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _hostingEnvironment;

        List<PropertyAppModel> listprops = new List<PropertyAppModel>();

        public AllPropertiesController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: AllProperties of Admin sign in
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            string name = await _userManager.GetUserNameAsync(user); // name = email

            // get DB User
            var userdb = _context.User.FirstOrDefault(x => x.email == name);

            if (userdb != null)
            {
                // get all properties of admin user
                var models = await _context.Property.ToListAsync();

                foreach (var mod in models)
                {
                    PropertyAppModel pro = new PropertyAppModel();

                    // get category
                    var p = _context.Category.FirstOrDefault(x => x.Id == mod.category_id);

                    var u = _context.User.FirstOrDefault(x => x.id == mod.owner_id);

                    pro.id = mod.id;
                    pro.Category = p.categoryName;
                    pro.Description = mod.description;
                    pro.Address = mod.Address;
                    pro.Price = mod.price;
                    pro.Status = getStatus(mod.status.ToString());
                    pro.Location = mod.location;

                    var im = _context.Image.FirstOrDefault(x => x.property_id == mod.id);
                    if (im != null)
                        pro.ImageURL = im.imageUrl;

                    pro.Owner = u.email;

                    listprops.Add(pro);
                }

                if (listprops.Count > 0)
                {
                    ViewBag.empty = false;
                }
                else
                {
                    ViewBag.empty = true;
                }
                return View(listprops);
            }
            return View();
        }

        // GET: AllProperties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Property == null)
            {
                return NotFound();
            }

            var property = await _context.Property.FirstOrDefaultAsync(m => m.id == id);
            if (property == null)
            {
                return NotFound();
            }

            ViewBag.category = getCategory(property.category_id.ToString());
            ViewBag.status = getStatus(property.status.ToString());

            //get location to generate to map
            var request = new GeocodeRequest();
            request.BingMapsKey = "AjbSxlgzSHLbi3vL9WhgUp0AaOyV3zCiARaEx8uNL2ma-aCzmCTjrlVwu0qTttUp";

            request.Query = property.location;

            var result = request.Execute();
            Location Geocode = (BingMapsRESTToolkit.Location)result.Result.ResourceSets[0].Resources[0];

            ViewBag.latitude = Geocode.Point.Coordinates[0];
            ViewBag.longitude = Geocode.Point.Coordinates[1];

            var img = _context.Image.FirstOrDefault(x => x.property_id == property.id);

            if (img != null)
                ViewBag.imgurl = img.imageUrl;
            else
                ViewBag.imgurl = "";

            return View(property);
        }


        // GET: AllProperties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Property == null)
            {
                return NotFound();
            }

            var property = await _context.Property.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            var img = _context.Image.FirstOrDefault(x => x.property_id == property.id);

            PropertyAppModel proapp = new PropertyAppModel
            {
                Category = getCategory(property.category_id.ToString()),
                Description = property.description,
                Address = property.Address,
                Price = property.price,
                Status = getStatus(property.status.ToString()),
                Location = property.location,
                ImageURL = img.imageUrl
            };

            return View(proapp);
        }

        // POST: AllProperties/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PropertyAppModel propertyAppModel)
        {
            if (id != propertyAppModel.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "img");
                    string pathForPic = propertyAppModel.Image.FileName;
                    string filePath = Path.Combine(uploadFolder, pathForPic);

                    Property p = _context.Property.FirstOrDefault(x => x.id == id);
                    
                    p.Address = propertyAppModel.Address;
                    p.description = propertyAppModel.Description;
                    p.price = propertyAppModel.Price;
                    p.status = Convert.ToInt32(propertyAppModel.Status);
                    p.location = propertyAppModel.Location;
                    p.category_id = Convert.ToInt32(propertyAppModel.Category);
                   
                    _context.Update(p);
                    await _context.SaveChangesAsync();

                    var img = _context.Image.FirstOrDefault(i => i.property_id == p.id);

                    if (img != null && img.imageUrl != pathForPic)
                    {
                        propertyAppModel.Image.CopyTo(new FileStream(filePath, FileMode.Create));
                        img.imageUrl = pathForPic;
                        _context.Update(img); // add image to DB
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyExists(propertyAppModel.id))
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
            return View(propertyAppModel);
        }

        // GET: AllProperties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Property == null)
            {
                return NotFound();
            }

            var property = await _context.Property.FirstOrDefaultAsync(m => m.id == id);
            if (property == null)
            {
                return NotFound();
            }

            ViewBag.category = getCategory(property.category_id.ToString());
            ViewBag.status = getStatus(property.status.ToString());

            var img = _context.Image.FirstOrDefault(x => x.property_id == property.id);

            if (img != null)
                ViewBag.imgurl = img.imageUrl;
            else
                ViewBag.imgurl = "";

            return View(property);
        }

        // POST: AllProperties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Property == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Properties'  is null.");
            }

            var property = await _context.Property.FindAsync(id);

            if (property != null)
            {
                var img = _context.Image.FirstOrDefault(x => x.property_id == property.id);

                // delete image from folder
                string file = img.imageUrl;
                string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "img");
                string fileToDelete = Path.Combine(uploadFolder, file);
                if (System.IO.File.Exists(fileToDelete))
                {
                    System.IO.File.Delete(fileToDelete);
                }

                _context.Property.Remove(property);  // delete property from DB
                _context.Image.Remove(img); // delete image from DB
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyExists(int id)
        {
          return _context.Property.Any(e => e.id == id);
        }

        public string getCategory(string catid)
        {
            string cat = String.Empty;
            if (catid == "1")
                cat = "House";
            else if (catid == "2")
                cat = "Condo";
            else if (catid == "3")
                cat = "Apartment";
            else
                cat = "Cottagge";
            return cat;
        }

        public string getStatus(string statid)
        {
            string stat = String.Empty;
            if (statid == "0")
                stat = "OnSale";
            else
                stat = "Sold";
            return stat;
        }

    }  // end class
}// end namespace
