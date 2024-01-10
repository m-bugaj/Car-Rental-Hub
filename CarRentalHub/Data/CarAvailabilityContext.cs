using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CarRentalHub.Models;

namespace CarRentalHub.Data
{
    public class CarAvailabilityContext : DbContext
    {
        public CarAvailabilityContext (DbContextOptions<CarAvailabilityContext> options)
            : base(options)
        {
        }

        public DbSet<CarRentalHub.Models.CarAvailability> CarAvailability { get; set; } = default!;
    }
}
