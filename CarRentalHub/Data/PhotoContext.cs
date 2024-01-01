using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CarRentalHub.Models;

namespace CarRentalHub.Data
{
    public class PhotoContext : DbContext
    {
        public PhotoContext (DbContextOptions<PhotoContext> options)
            : base(options)
        {
        }

        public DbSet<CarRentalHub.Models.Photo> Photo { get; set; } = default!;
    }
}
