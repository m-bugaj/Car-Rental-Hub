using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarRentalHub.Data;
using CarRentalHub.Models;

namespace CarRentalHub
{
    public class CarAvailabilitiesController : Controller
    {
        private readonly CarAvailabilityContext _context;

        public CarAvailabilitiesController(CarAvailabilityContext context)
        {
            _context = context;
        }

        // GET: CarAvailabilities
        public async Task<IActionResult> Index()
        {
            return View(await _context.CarAvailability.ToListAsync());
        }

        // GET: CarAvailabilities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carAvailability = await _context.CarAvailability
                .FirstOrDefaultAsync(m => m.ID == id);
            if (carAvailability == null)
            {
                return NotFound();
            }

            return View(carAvailability);
        }

        // GET: CarAvailabilities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CarAvailabilities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,CarID,StartDate,EndDate")] CarAvailability carAvailability)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carAvailability);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(carAvailability);
        }

        // GET: CarAvailabilities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carAvailability = await _context.CarAvailability.FindAsync(id);
            if (carAvailability == null)
            {
                return NotFound();
            }
            return View(carAvailability);
        }

        // POST: CarAvailabilities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,CarID,StartDate,EndDate")] CarAvailability carAvailability)
        {
            if (id != carAvailability.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carAvailability);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarAvailabilityExists(carAvailability.ID))
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
            return View(carAvailability);
        }

        // GET: CarAvailabilities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carAvailability = await _context.CarAvailability
                .FirstOrDefaultAsync(m => m.ID == id);
            if (carAvailability == null)
            {
                return NotFound();
            }

            return View(carAvailability);
        }

        // POST: CarAvailabilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carAvailability = await _context.CarAvailability.FindAsync(id);
            if (carAvailability != null)
            {
                _context.CarAvailability.Remove(carAvailability);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarAvailabilityExists(int id)
        {
            return _context.CarAvailability.Any(e => e.ID == id);
        }
    }
}
