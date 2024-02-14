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
using System.Globalization;

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
        private readonly FilterDataContext _filterDataContext;


        public CarsController(CarRentalHubContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment, PhotoContext photoContext, CarAvailabilityContext carAvailabilityContext, FilterDataContext filterDataContext)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _photoContext = photoContext;
            _carAvailabilityContext = carAvailabilityContext;
            _filterDataContext = filterDataContext;
        }

        // GET: Cars
        [AllowAnonymous]
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

            // Przekazanie parametrów formularza do widoku
            ViewBag.FilterParameters = new FilterParameters
            {
                SelectedMarka = selectedMarka,
                SelectedModel = selectedModel,
                SelectedGeneration = selectedGeneration,
                SelectedYearFrom = selectedYearFrom,
                SelectedYearTo = selectedYearTo,
                SelectedBodyType = selectedBodyType,
                SelectedFuelType = selectedFuelType,
                MileageFrom = MileageFrom,
                MileageTo = MileageTo,
                PriceFrom = PriceFrom,
                PriceTo = PriceTo

            };

            ViewBag.DostepneMarki = PobierzListeMarek();

            // Uzyskaj pierwszą markę z listy
            var pierwszaMarka = (ViewBag.DostepneMarki as List<SelectListItem>)?.FirstOrDefault()?.Value;

            // Pobierz dostępne modele dla pierwszej marki
            ViewBag.DostepneModele = PobierzModeleZBazy(pierwszaMarka);

            // Generacje
            var firstCarModel = (ViewBag.DostepneModele as List<SelectListItem>)?.FirstOrDefault()?.Value;
            ViewBag.AvailableGenerations = GetGenerationFromDatabase(firstCarModel);

            // Lata
            var years = Enumerable.Range(1900, DateTime.Now.Year - 1899).OrderByDescending(y => y).ToList();
            ViewBag.AvailableYears = years;

            // Typ nadwozia
            List<string> bodyType = ["Auta małe", "Auta miejskie", "Coupe", "Kabriolet", "Kombi", "Kompakt", "Minivan", "Sedan", "Suv"];
            ViewBag.BodyType = bodyType;

            // Typ paliwa
            List<string> fuelType = ["Benzyna", "Benzyna+CNG", "Benzyna+LPG", "Diesel", "Elektryczny", "Etanol", "Hybryda", "Wodór"];
            ViewBag.FuelType = fuelType;


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
        public IActionResult AddAvailability(int carId, DateTime startDate, DateTime endDate, string email, string phone, string userId)
        {
            var reservationDate = DateTime.Now.Date;

            var newAvailability = new CarAvailability
            {
                CarID = carId,
                StartDate = startDate,
                EndDate = endDate,
                Email = email,
                Phone = phone,
                UserId = userId,
                ReservationDate = reservationDate
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


            // Get logged in userId
            var userId = _userManager.GetUserId(User);

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
            ViewData["UserId"] = userId;

            return View(model);
        }

        // GET: Cars/Create
        public IActionResult Create()
        {
            ViewBag.DostepneMarki = PobierzListeMarek();

            // Uzyskaj pierwszą markę z listy
            var pierwszaMarka = (ViewBag.DostepneMarki as List<SelectListItem>)?.FirstOrDefault()?.Value;

            // Pobierz dostępne modele dla pierwszej marki
            var dostepneModele = PobierzModeleZBazy(pierwszaMarka);

            // Konwertuj listę modeli na listę SelectListItem
            ViewBag.DostepneModele = dostepneModele.Select(model => new SelectListItem { Value = model, Text = model }).ToList();

            // Generacje
            var firstCarModel = (ViewBag.DostepneModele as List<SelectListItem>)?.FirstOrDefault()?.Value;
            ViewBag.AvailableGenerations = GetGenerationFromDatabase(firstCarModel);

            // Lata
            var years = Enumerable.Range(1900, DateTime.Now.Year - 1899).OrderByDescending(y => y).ToList();
            ViewBag.AvailableYears = years;

            // Typ paliwa
            List<string> fuelType = new List<string> { "Benzyna", "Benzyna+CNG", "Benzyna+LPG", "Diesel", "Elektryczny", "Etanol", "Hybryda", "Wodór" };
            ViewBag.FuelType = fuelType.Select(item => new SelectListItem { Text = item, Value = item }).ToList();

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

                return RedirectToAction(nameof(MyOffers));
            }

            return View(car);
        }



        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.DostepneMarki = PobierzListeMarek();

            // Uzyskaj pierwszą markę z listy
            var pierwszaMarka = (ViewBag.DostepneMarki as List<SelectListItem>)?.FirstOrDefault()?.Value;

            // Pobierz dostępne modele dla pierwszej marki
            var dostepneModele = PobierzModeleZBazy(pierwszaMarka);

            // Konwertuj listę modeli na listę SelectListItem
            ViewBag.DostepneModele = dostepneModele.Select(model => new SelectListItem { Value = model, Text = model }).ToList();

            // Generacje
            var firstCarModel = (ViewBag.DostepneModele as List<SelectListItem>)?.FirstOrDefault()?.Value;
            ViewBag.AvailableGenerations = GetGenerationFromDatabase(firstCarModel);

            // Lata
            var years = Enumerable.Range(1900, DateTime.Now.Year - 1899).OrderByDescending(y => y).ToList();
            ViewBag.AvailableYears = years;

            // Typ paliwa
            List<string> fuelType = new List<string> { "Benzyna", "Benzyna+CNG", "Benzyna+LPG", "Diesel", "Elektryczny", "Etanol", "Hybryda", "Wodór" };
            ViewBag.FuelType = fuelType.Select(item => new SelectListItem { Text = item, Value = item }).ToList();

            // Typ nadwozia
            List<string> bodyType = new List<string> { "Auta małe", "Auta miejskie", "Coupe", "Kabriolet", "Kombi", "Kompakt", "Minivan", "Sedan", "Suv" };
            ViewBag.BodyType = bodyType.Select(item => new SelectListItem { Text = item, Value = item }).ToList();

            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.CarInfoModel.FindAsync(id);
            car.YearsList = Enumerable.Range(1900, DateTime.Now.Year - 1899).Reverse().ToList();
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
            // Przekształć tekstową wartość ceny na decimal
            if (int.TryParse(car.Price.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out int parsedPrice))
            {
                car.Price = parsedPrice;
            }
            else
            {
                // Jeśli nie uda się przekształcić, obsłuż błąd
                ModelState.AddModelError("Item1.Price", "Nieprawidłowy format ceny.");
            }
            Console.WriteLine("QQQ");
            Console.WriteLine(car.Price);
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

            car.YearsList = Enumerable.Range(1900, DateTime.Now.Year - 1899).Reverse().ToList();

            var model = new Tuple<Car, IEnumerable<Photo>>(
                car,
                await _photoContext.Photo.ToListAsync()
            );

            return View(model);
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

        private List<SelectListItem> PobierzListeMarek()
        {
            // Pobierz unikalne marki samochodów z bazy danych
            var marki = _filterDataContext.FilterData
                .Select(m => m.VehicleBrand)
                .Distinct()
                .ToList();

            // Przygotuj listę SelectListItem dla rozwijanej listy
            var listaMarek = marki.Select(m => new SelectListItem
            {
                Value = m,
                Text = m
            }).ToList();

            return listaMarek;
        }

        [HttpGet]
        public JsonResult PobierzModele(string marka)
        {
            // Tutaj pobierz modele samochodów dla danej marki z bazy danych
            var modele = PobierzModeleZBazy(marka);

            // Zwróć dane w formacie JSON
            return Json(modele);
        }

        private List<string> PobierzModeleZBazy(string marka)
        {
            return _filterDataContext.FilterData
                .Where(m => m.VehicleBrand == marka)
                .Select(m => m.CarModel)
                .Distinct()
                .ToList();
        }

        public JsonResult GetGeneration(string carModel)
        {
            var generation = GetGenerationFromDatabase(carModel);
            return Json(generation);
        }

        private List<string> GetGenerationFromDatabase(string carModel)
        {
            return _filterDataContext.FilterData
                .Where(m => m.CarModel == carModel)
                .Select(m => m.Generation)
                .Distinct()
                .ToList();
        }

        // GET: Cars/MyReservations/
        public async Task<IActionResult> MyReservations()
        {
            // Ustawienie dostępu do edycji ogłoszenia
            var userId = _userManager.GetUserId(User);

            // Wybierz rezerwacje z bazy danych, gdzie UserId jest równe userId
            var reservations = await _carAvailabilityContext.CarAvailability
                .Where(r => r.UserId == userId)
                .ToListAsync();

            // Pobierz ID zarezerwowanych samochodów
            var reservedCarIds = await _carAvailabilityContext.CarAvailability
                .Where(r => r.UserId == userId)
                .Select(r => r.CarID)
                .ToListAsync();

            if (reservations == null)
            {
                return NotFound();
            }

            // Wybierz auta z CarInfoModel, których ID występuje w zarezerwowanych CarID
            var bookedCarsInfo = _context.CarInfoModel
                .Where(b => reservedCarIds.Contains(b.ID))
                .ToList();

            var model = new Tuple<IEnumerable<Car>, IEnumerable<Photo>, IEnumerable<CarAvailability>>(
                    bookedCarsInfo,
                    await _photoContext.Photo.ToListAsync(),
                    reservations
                    );
            return View(model);
        }

        // GET: Cars/MyReservation/5
        public async Task<IActionResult> ReservationDetails(int? id)
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


            // Get logged in userId
            var userId = _userManager.GetUserId(User);

            // Wybierz rezerwacje z bazy danych, gdzie ID jest równe id z zapytania GET
            var reservation = await _carAvailabilityContext.CarAvailability
                .Where(r => r.ID == id)
                .ToListAsync();

            // Pobierz ID zarezerwowanego samochodów
            var reservedCarId = await _carAvailabilityContext.CarAvailability
                .Where(r => r.ID == id)
                .Select(r => r.CarID)
                .ToListAsync();

            // Wybierz auta z CarInfoModel, których ID występuje w zarezerwowanych CarID
            var bookedCarInfo = _context.CarInfoModel
                .Where(b => reservedCarId.Contains(b.ID))
                .ToList();

            var model = new Tuple<IEnumerable<Car>, IEnumerable<Photo>, IEnumerable<CarAvailability>>(
                    bookedCarInfo,
                    await _photoContext.Photo.ToListAsync(),
                    reservation
                    );

            var sellerId = _context.CarInfoModel
                .Where(c => c.ID == bookedCarInfo.First().ID)
                .Select(c=> c.UserId)
                .FirstOrDefault();

            var sellersEmail = _userManager.Users
                .Where(u => u.Id == sellerId)
                .Select(u => u.Email)
                .FirstOrDefault();

            ViewBag.SellersEmail = sellersEmail;
            ViewData["CurrentReservationId"] = id;
            ViewData["UserId"] = userId;

            return View(model);


        }

        // GET: Cars/Reservations/5
        public async Task<IActionResult> Reservations(int? id)
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


            // Get logged in userId
            var userId = _userManager.GetUserId(User);

            // Wybierz rezerwacje z bazy danych, gdzie ID jest równe id z zapytania GET
            var reservation = await _carAvailabilityContext.CarAvailability
                .Where(r => r.CarID == id)
                .ToListAsync();


            // Pobierz ID zarezerwowanego samochodów
            var reservedCarId = await _carAvailabilityContext.CarAvailability
                .Where(r => r.CarID == id)
                .Select(r => r.CarID)
                .ToListAsync();

            // Wybierz auta z CarInfoModel, których ID występuje w zarezerwowanych CarID
            var bookedCarInfo = _context.CarInfoModel
                .Where(b => reservedCarId.Contains(b.ID))
                .ToList();

            var model = new Tuple<IEnumerable<Car>, IEnumerable<Photo>, IEnumerable<CarAvailability>>(
                    bookedCarInfo,
                    await _photoContext.Photo.ToListAsync(),
                    reservation
                    );

            //var sellerId = _context.CarInfoModel
            //    .Where(c => c.ID == bookedCarInfo.First().ID)
            //    .Select(c => c.UserId)
            //    .FirstOrDefault();

            //var sellersEmail = _userManager.Users
            //    .Where(u => u.Id == sellerId)
            //    .Select(u => u.Email)
            //    .FirstOrDefault();

            //ViewBag.SellersEmail = sellersEmail;
            ViewData["CurrentReservationId"] = id;
            ViewData["UserId"] = userId;

            return View(model);


        }

        public async Task<IActionResult> MyOffers()
        {
            // Ustawienie dostępu do edycji ogłoszenia
            var userId = _userManager.GetUserId(User);

            var myCars = await _context.CarInfoModel
                .Where(c => c.UserId == userId)
                .ToListAsync();

            var myCarsIds = await _context.CarInfoModel
                .Where(c => c.UserId == userId)
                .Select(c => c.ID)
                .ToListAsync();

            if (myCars == null)
            {
                return NotFound();
            }

            // Wybierz auta z CarInfo, których ID występuje w zarezerwowanych CarID
            var myCarsInfo = _carAvailabilityContext.CarAvailability
                .Where(m => myCarsIds.Contains(m.CarID))
                .ToList();

            var model = new Tuple<IEnumerable<Car>, IEnumerable<Photo>, IEnumerable<CarAvailability>>(
                    myCars,
                    await _photoContext.Photo.ToListAsync(),
                    myCarsInfo
                    );
            return View(model);
        }

    }

    public class FilterParameters
    {
        public string SelectedMarka { get; set; }
        public string SelectedModel { get; set; }
        public string SelectedGeneration { get; set; }
        public int? SelectedYearFrom { get; set; }
        public int? SelectedYearTo { get; set; }
        public string SelectedBodyType { get; set; }
        public string SelectedFuelType { get; set; }
        public int? MileageFrom { get; set; }
        public int? MileageTo { get; set; }
        public int? PriceFrom { get; set; }
        public int? PriceTo { get; set; }
    }
}
