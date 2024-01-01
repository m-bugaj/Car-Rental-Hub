using System.ComponentModel.DataAnnotations;

namespace CarRentalHub.Models
{
    public class Photo
    {
        public int ID { get; set; }

        public string ImagePath { get; set; }

        public bool IsMainPhoto { get; set; }

        //Foreign key (from AspNetUsers - Id)
        public int AdvertisementId { get; set; }
    }
}
