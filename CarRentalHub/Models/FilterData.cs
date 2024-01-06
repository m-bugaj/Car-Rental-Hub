using System.ComponentModel.DataAnnotations;

namespace CarRentalHub.Models
{
    public class FilterData
    {
        public int ID { get; set; }

        [Display(Name = "Marka pojazdu")]
        public string VehicleBrand { get; set; }

        [Display(Name = "Model pojazdu")]
        public string CarModel { get; set; }
    }
}
