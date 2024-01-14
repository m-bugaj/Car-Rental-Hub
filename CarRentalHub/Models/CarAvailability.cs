using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalHub.Models
{
    public class CarAvailability
    {
        public int ID { get; set; }

        public int CarID { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Adres E-mail")]
        public string Email { get; set; }

        [Display(Name = "Numer telefonu")]
        public string Phone { get; set; }

        [Display(Name = "User ID")]
        public string UserId { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReservationDate { get; set; }
    }
}
