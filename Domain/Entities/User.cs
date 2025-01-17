namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Role { get; set; }

        public ICollection<Payment> Payments { get; set; }
        public ICollection<Reservation> Reservations { get; set; }

    }
}