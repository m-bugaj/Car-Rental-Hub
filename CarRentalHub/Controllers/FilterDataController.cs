using CarRentalHub.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarRentalHub.Controllers
{
    public class FilterDataController : Controller
    {
        private readonly FilterDataContext _context;

        public FilterDataController(FilterDataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.DostepneMarki = PobierzListeMarek();

            // Uzyskaj pierwszą markę z listy
            var pierwszaMarka = (ViewBag.DostepneMarki as List<SelectListItem>)?.FirstOrDefault()?.Value;

            // Pobierz dostępne modele dla pierwszej marki
            ViewBag.DostepneModele = PobierzModeleZBazy(pierwszaMarka);

            return View();
        }



        private List<SelectListItem> PobierzListeMarek()
        {
            // Pobierz unikalne marki samochodów z bazy danych
            var marki = _context.FilterData
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
            return _context.FilterData
                .Where(m => m.VehicleBrand == marka)
                .Select(m => m.CarModel)
                .Distinct()
                .ToList();
        }


    }
}
