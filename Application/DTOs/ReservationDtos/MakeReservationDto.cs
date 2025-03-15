using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.DTOs.ReservationDtos
{
    public class MakeReservationDto
    {
        public int FacilityId { get; set; }
        public string CustomerType { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [FromForm(Name = "packages")]
        public string PackagesJson { get; set; } // selected packages with quantities
        public string? GuestEmail { get; set; } // for employee bookings
        public string? GuestPhone { get; set; } // for employee bookings
        public List<IFormFile>? Documents { get; set; } // for public/corporate
        public string PaymentMethod { get; set; }
        public decimal AmountPaid { get; set; }
        public List<IFormFile>? BankTransferReceipts { get; set; } //only for bank payment method

        [JsonIgnore]
        public List<SelectedPackageDto>? Packages =>
            string.IsNullOrEmpty(PackagesJson)
                ? new List<SelectedPackageDto>()
                : JsonSerializer.Deserialize<List<SelectedPackageDto>>(PackagesJson);
    }
}
