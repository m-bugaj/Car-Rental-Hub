using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CarRentalHub.Models;

namespace CarRentalHub.Data
{
    public class FilterDataContext : DbContext
    {
        public FilterDataContext (DbContextOptions<FilterDataContext> options)
            : base(options)
        {
        }

        public DbSet<CarRentalHub.Models.FilterData> FilterData { get; set; } = default!;
    }
}
