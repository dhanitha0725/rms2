namespace Application.DTOs.ReservationDtos
{
    // reservation table data
    public class ReservationDataDto
    {
        public int ReservationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public string UserType { get; set; }
    }

    // reservation details
    public class ReservationDetailsDto
    {
        public int ReservationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public string UserType { get; set; }

        public ReservationUserDetailDto ReservationUser { get; set; }
        public List<PaymentDto>? Payments { get; set; }
        public List<ReservationPackageDto>? ReservedPackages { get; set; }
        public List<ReservationRoomDto>? ReservedRooms { get; set; }
    }

    // reservation user details
    public class ReservationUserDetailDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? OrganizationName { get; set; }
    }

    // reservation payment details
    public class PaymentDto
    {
        public string OrderID { get; set; }
        public string Method { get; set; }
        public decimal? AmountPaid { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
    }

    // reserved rooms details
    public class ReservationRoomDto
    {
        public string RoomType { get; set; }
        public string FacilityName { get; set; }
    }

    // reserved packages details
    public class ReservationPackageDto
    {
        public string PackageName { get; set; }
        public string FacilityName { get; set; }
    }
}
