namespace Application.DTOs.ReservationDtos
{
    public class CreateReservationDto
    {
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string UserType { get; set; }
        public List<PackageRequest> Packages { get; set; }
        public List<RoomRequest> Rooms { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? OrganizationName { get; set; }
        public string CreatedBy { get; set; }
    }

    public class PackageRequest
    {
        public int PackageId { get; set; }
    }

    public class RoomRequest
    {
        public string RoomType { get; set; }
        public int Quantity { get; set; }
    }

    public class ReservationResultDto
    {
        public int ReservationId { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
    }
}
