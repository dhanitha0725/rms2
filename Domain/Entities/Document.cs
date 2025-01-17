namespace Domain.Entities
{
    public class Document
    {
        public int DocumentID { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public Guid ReservationID { get; set; }
        public int UserID { get; set; }

        public Reservation Reservation { get; set; }
        public User User { get; set; }
    }
}
