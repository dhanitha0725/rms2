namespace Domain.Entities
{
    public class ReservedRoom
    {
        public int ReservedRoomID { get; set; }
        public int ReservationID { get; set; }
        public int RoomID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Reservation Reservation { get; set; }
        public Room Room { get; set; }
    }
}