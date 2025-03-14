using Microsoft.AspNetCore.Http;

namespace Application.DTOs.ReservationDtos
{
    public class MakeReservationDto
    {
        public int FacilityId { get; set; }
        public string CustomerType { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<SelectedPackageDto> Packages { get; set; } // selected packages with quantities
        public string? GuestEmail { get; set; } // for employee bookings
        public string? GuestPhone { get; set; } // for employee bookings
        public List<IFormFile>? Documents { get; set; } // for public/corporate
        public string PaymentMethod { get; set; }
        public decimal AmountPaid { get; set; }
        public List<IFormFile>? BankTransferReceipts { get; set; }
    }
}
