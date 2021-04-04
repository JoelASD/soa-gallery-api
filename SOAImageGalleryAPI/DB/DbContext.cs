using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SOAImageGalleryAPI.Models;

namespace ConsoleApp.PostgreSQL
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base (options)
        {

        }
        public DbSet<Image> Images { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}