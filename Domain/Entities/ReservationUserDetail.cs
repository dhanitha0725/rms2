namespace Domain.Entities
{
    public class ReservationUserDetail
    {
        public int ReservationUserDetailID { get; set; }
        public int ReservationID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? OrganizationName { get; set; }

        public Reservation Reservation { get; set; }
    }
}
