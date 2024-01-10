using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarRentalHub.Data;
using CarRentalHub.Models;
using Microsoft.AspNetCore.Authorization;
using CarRentalHub.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;

namespace CarRentalHub.Controllers
{
    [Authorize]
    public class CarsController : Controller
    {
        private readonly CarRentalHubContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly PhotoContext _photoContext;
        private readonly CarAvailabilityContext _carAvailabilityContext;


        public CarsController(CarRentalHubContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment, PhotoContext photoContext, CarAvailabilityContext carAvailabilityContext)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _photoContext = photoContext;
            _carAvailabilityContext = carAvailabilityContext;
        }

        // GET: Cars
        public async Task<IActionResult> Index(string selectedMarka, string selectedModel, string selectedGeneration, int? selectedYearFrom, int? selectedYearTo, string selectedBodyType, string selectedFuelType, int? MileageFrom, int? MileageTo, int? PriceFrom, int? PriceTo)
        {
            // Pobierz samochody na podstawie wybranych marek i modeli z bazy danych
            var filteredCars = GetFilteredCars(selectedMarka, selectedModel, selectedGeneration, selectedYearFrom, selectedYearTo, selectedBodyType, selectedFuelType, MileageFrom, MileageTo, PriceFrom, PriceTo);

            var mainPhotos = _photoContext.Photo
                .Where(p => p.IsMainPhoto)
                .Select(p => new Photo
                {
                    ImagePath = p.ImagePath,
                    AdvertisementId = p.AdvertisementId
                })
                .ToList();

            if (!filteredCars.Any())
            {
                var model = new Tuple<IEnumerable<Car>, IEnumerable<Photo>>(
                    await _context.CarInfoModel.ToListAsync(),
                    await _photoContext.Photo.ToListAsync()
                    );
                return View(model);
            }
            else
            {
                var model = new Tuple<IEnumerable<Car>, IEnumerable<Photo>>(
                    filteredCars.ToList(),
                    await _photoContext.Photo.ToListAsync()
                    );
                return View(model);
            }

            // return View(await _context.CarInfoModel.ToListAsync());
        }

        private IQueryable<Car> GetFilteredCars(string selectedMarka, string selectedModel, string selectedGeneration, int? selectedYearFrom, int? selectedYearTo, string selectedBodyType, string selectedFuelType, int? MileageFrom, int? MileageTo, int? PriceFrom, int? PriceTo)
        {
            // Pobierz samochody z bazy danych na podstawie wybranych marek i modeli
            var cars = _context.CarInfoModel.AsQueryable();

            if (!string.IsNullOrEmpty(selectedMarka))
            {
                cars = cars.Where(c => c.VehicleBrand == selectedMarka);
            }

            if (!string.IsNullOrEmpty(selectedModel))
            {
                cars = cars.Where(c => c.CarModel == selectedModel);
            }

            if (!string.IsNullOrEmpty(selectedGeneration))
            {
                cars = cars.Where(c => c.Generation == selectedGeneration);
            }

            if (selectedYearFrom != null)
            {
                cars = cars.Where(c => c.YearOfProduction >= selectedYearFrom);
            }

            if (selectedYearTo != null)
            {
                cars = cars.Where(c => c.YearOfProduction <= selectedYearTo);
            }

            if (!string.IsNullOrEmpty(selectedBodyType))
            {
                cars = cars.Where(c => c.BodyType == selectedBodyType);
            }
            
            if (!string.IsNullOrEmpty(selectedFuelType))
            {
                cars = cars.Where(c => c.FuelType == selectedFuelType);
            }
            
            if (MileageFrom != null)
            {
                cars = cars.Where(c => c.Mileage >= MileageFrom);
            }

            if (MileageTo != null)
            {
                cars = cars.Where(c => c.Mileage <= MileageTo);
            }

            if (PriceFrom != null)
            {
                cars = cars.Where(c => c.Price >= PriceFrom);
            }

            if (PriceTo != null)
            {
                cars = cars.Where(c => c.Price <= PriceTo);
            }


            return cars;
        }

        // Rezerwacja auta na dany termin
        [HttpPost]
        public IActionResult AddAvailability(int carId, DateTime startDate, DateTime endDate)
        {
            var newAvailability = new CarAvailability
            {
                CarID = carId,
                StartDate = startDate,
                EndDate = endDate
            };

            _carAvailabilityContext.Add(newAvailability);
            _carAvailabilityContext.SaveChanges();

            return RedirectToAction("Details", new { id = carId });
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.CarInfoModel
                .FirstOrDefaultAsync(m => m.ID == id);

            if (car == null)
            {
                return NotFound();
            }

            // Pobierz rzeczywiste niedostępne daty z bazy danych
            var niedostepneDatyQuery = _carAvailabilityContext.CarAvailability
                .Where(ca => ca.CarID == id)
                .AsEnumerable()  // Ta linia powinna umożliwić ewaluację części zapytania na poziomie klienta
                .SelectMany(ca => Enumerable.Range(0, 1 + (ca.EndDate - ca.StartDate).Days)
                                  .Select(offset => ca.StartDate.AddDays(offset)));
            var niedostepneDaty = niedostepneDatyQuery.ToList();

            // Przekazanie danych do widoku
            ViewBag.NiedostepneDaty = niedostepneDaty;

            var model = new Tuple<IEnumerable<Car>, IEnumerable<Photo>>(
                await _context.CarInfoModel.ToListAsync(),
                await _photoContext.Photo.ToListAsync()
            );

            ViewData["CurrentAdvertisementId"] = id;

            return View(model);
        }

        // GET: Cars/Create
        public IActionResult Create()
        {
			// Typ nadwozia
			List<string> bodyType = new List<string> { "Auta małe", "Auta miejskie", "Coupe", "Kabriolet", "Kombi", "Kompakt", "Minivan", "Sedan", "Suv" };
			ViewBag.BodyType = bodyType.Select(item => new SelectListItem { Text = item, Value = item }).ToList();

			var model = new Car
            {
                YearsList = Enumerable.Range(1900, DateTime.Now.Year - 1899).Reverse().ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,VehicleBrand,CarModel,Generation,BodyType,YearOfProduction,FuelType,Mileage,Price,UserId,Photos,SelectedPhotoId,SelectedFileName")] Car car)
        {
            if (ModelState.IsValid)
            {
                // Get logged in user
                var user = await _userManager.GetUserAsync(User);

                // Assign UserId before adding to the database context
                car.UserId = user.Id;

                _context.Add(car);
                await _context.SaveChangesAsync();

                Console.WriteLine("ELO");

                // Przetwarzanie przesyłanych plików
                if (car.Photos != null && car.Photos.Count > 0)
                {
                    Console.WriteLine("MELO");

                    var photoCount = 0;

                    foreach (var photo in car.Photos)
                    {
                        Console.WriteLine("ELO");
                        Console.WriteLine(photo.FileName);
                        Console.WriteLine(car.SelectedFileName);
                        // Sprawdź, czy plik został przesłany
                        if (photo.Length > 0)
                        {
                            // Wygeneruj unikalną nazwę pliku (możesz użyć GUID)
                            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(photo.FileName);

                            // Określ lokalizację, gdzie zostanie zapisane zdjęcie (np. w folderze wwwroot/images)
                            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            // Zapisz plik na dysku
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await photo.CopyToAsync(fileStream);
                            }

                            // Tutaj możesz zapisać ścieżkę do zdjęcia w bazie danych
                            // Przykładowo, dodaj kolumnę do modelu Car i zapisz ścieżkę
                            // car.PhotoPath = "/images/" + uniqueFileName;
  
                            _photoContext.Photo.Add(new Photo
                            {
                                ImagePath = filePath,
                                IsMainPhoto = (photo.FileName == car.SelectedFileName) || (photoCount == 0 && car.SelectedFileName == null), // IsMainPhoto Setting
                                AdvertisementId = car.ID
                            });
                            photoCount++;
                        }
                    }
                    // Zapisz zmiany w bazie danych
                    await _photoContext.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            return View(car);
        }



        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.CarInfoModel.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            // Ustawienie dostępu do edycji ogłoszenia
            var userId = _userManager.GetUserId(User);
            if (car.UserId != userId)
            {
                return Forbid(); // Możesz również użyć return Unauthorized();
            }

            var model = new Tuple<Car, IEnumerable<Photo>>(
                car,
                await _photoContext.Photo.ToListAsync()
            );

            ViewData["CurrentAdvertisementId"] = id;

            return View(model);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,VehicleBrand,CarModel,Generation,BodyType,YearOfProduction,FuelType,Mileage,Price,UserId,Photos,SelectedPhotoId,SelectedFileName")] Car car)
        {
            if (id != car.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get logged in user
                    var user = await _userManager.GetUserAsync(User);

                    // Assign UserId before adding to the database context
                    car.UserId = user.Id;

                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                Console.WriteLine(car.SelectedFileName);
                // Przetwarzanie jeżeli zostało zmienione zdjęcie główne bez przesłania plików

                // Pobierz wszystkie rekordy dla AdvertisementId = car.ID
                var previousMainPhoto = _photoContext.Photo.FirstOrDefault(p => p.AdvertisementId == car.ID && p.IsMainPhoto);
                var newMainPhoto = _photoContext.Photo.FirstOrDefault(p => p.AdvertisementId == car.ID && p.ImagePath == car.SelectedFileName);

                Console.WriteLine(previousMainPhoto);
                Console.WriteLine(car.SelectedFileName);
                Console.WriteLine(previousMainPhoto.ImagePath);
                Console.WriteLine(newMainPhoto);

                if (previousMainPhoto != null && car.SelectedFileName != previousMainPhoto.ImagePath && car.SelectedFileName != null && newMainPhoto != null)
                {
                    Console.WriteLine("MELO");
                    previousMainPhoto.IsMainPhoto = false;
                    newMainPhoto.IsMainPhoto = true;

                    await _photoContext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }


                // Przetwarzanie przesyłanych plików
                if (car.Photos != null && car.Photos.Count > 0)
                {
                    Console.WriteLine("MELO");

                    var photoCount = 0;
                    bool _isMainPhoto = false;

                    // Pobierz wszystkie rekordy dla AdvertisementId = car.ID
                    var photosToDelete = _photoContext.Photo.Where(p => p.AdvertisementId == car.ID);

                    // Usuń wszystkie znalezione rekordy
                    _photoContext.Photo.RemoveRange(photosToDelete);

                    foreach (var photo in car.Photos)
                    {
                        Console.WriteLine("ELO");
                        Console.WriteLine(photo.FileName);
                        Console.WriteLine(car.SelectedFileName);
                        // Sprawdź, czy plik został przesłany
                        if (photo.Length > 0)
                        {
                            // Wygeneruj unikalną nazwę pliku (możesz użyć GUID)
                            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(photo.FileName);

                            // Określ lokalizację, gdzie zostanie zapisane zdjęcie (np. w folderze wwwroot/images)
                            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            // Zapisz plik na dysku
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await photo.CopyToAsync(fileStream);
                            }

                            // Tutaj możesz zapisać ścieżkę do zdjęcia w bazie danych
                            // Przykładowo, dodaj kolumnę do modelu Car i zapisz ścieżkę
                            // car.PhotoPath = "/images/" + uniqueFileName;
                            if (photoCount != 0)
                            {
                                _isMainPhoto = false;
                            }

                            photoCount++;

                            _photoContext.Photo.Add(new Photo
                            {
                                ImagePath = filePath,
                                IsMainPhoto = photo.FileName == car.SelectedFileName, // IsMainPhoto Setting
                                AdvertisementId = car.ID
                            });
                        }
                    }
                    // Zapisz zmiany w bazie danych
                    await _photoContext.SaveChangesAsync();
                }


                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.CarInfoModel
                .FirstOrDefaultAsync(m => m.ID == id);
            if (car == null)
            {
                return NotFound();
            }

            // Ustawienie dostępu do edycji ogłoszenia
            var userId = _userManager.GetUserId(User);
            if (car.UserId != userId)
            {
                return Forbid(); // Możesz również użyć return Unauthorized();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.CarInfoModel.FindAsync(id);
            if (car != null)
            {
                _context.CarInfoModel.Remove(car);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return _context.CarInfoModel.Any(e => e.ID == id);
        }
    }
}
