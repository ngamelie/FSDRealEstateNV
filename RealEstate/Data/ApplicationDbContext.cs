using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using RealEstate.Models;

namespace RealEstate.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Category> Category { get; set; }
        public DbSet<Image> Image { get; set; } 
        public DbSet<Property> Property { get; set; }
        public DbSet<User> User { get; set; }

    }
        
}
