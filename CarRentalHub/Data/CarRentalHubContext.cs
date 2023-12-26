using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CarRentalHub.Models;

namespace CarRentalHub.Data
{
    public class CarRentalHubContext : DbContext
    {
        public CarRentalHubContext (DbContextOptions<CarRentalHubContext> options)
            : base(options)
        {
        }

        public DbSet<CarRentalHub.Models.Car> CarInfoModel { get; set; } = default!;
    }
}
