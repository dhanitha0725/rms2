using Microsoft.AspNetCore.Http;

namespace Application.DTOs.ReservationDtos
{
    public class CreateReservationDto
    {

    }

    public class ReservationUserInfoDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string OrganizationName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }

    public class ReservationResultDto
    {
        public int ReservationId { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public string OrderId { get; set; }
        public decimal Total { get; set; }
    }

    public class DocumentDto
    {
        public string DocumentType { get; set; }
        public IFormFile File { get; set; }
    }
}
