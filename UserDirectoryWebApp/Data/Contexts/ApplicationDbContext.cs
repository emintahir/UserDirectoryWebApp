using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserDirectoryWebApp.Data.Entities;

namespace UserDirectoryWebApp.Data.Contexts
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }

    }
}
