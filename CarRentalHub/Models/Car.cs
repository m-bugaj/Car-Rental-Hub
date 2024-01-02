using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CarRentalHub.Areas.Identity.Data;

namespace CarRentalHub.Models
{
    public class Car
    {
        public int ID { get; set; }

        [Display(Name = "Marka pojazdu")]
        public string VehicleBrand { get; set; }

        [Display(Name = "Model pojazdu")]
        public string CarModel { get; set; }

        [Display(Name = "Generacja")]
        public string Generation { get; set; }

        [Display(Name = "Typ nadwozia")]
        public string BodyType { get; set; }

        [Display(Name = "Rok produkcji")]
        public int? YearOfProduction { get; set; }

        // Lista do przechowywania rozwijanej listy lat
        
        public List<int>? YearsList { get; set; }

        [Display(Name = "Typ paliwa")]
        public string FuelType { get; set; }

        [Display(Name = "Przebieg")]
        public int Mileage { get; set; }

        [Display(Name = "Cena za dobę")]
        public decimal Price { get; set; }

        //Foreign key (from AspNetUsers - Id)
        public string? UserId { get; set; }

        [NotMapped]
        [Display(Name = "Prześlij zdjęcia: ")]
        public List<IFormFile>? Photos { get; set; }

        public string? SelectedPhotoId { get; set; }

    }
}
