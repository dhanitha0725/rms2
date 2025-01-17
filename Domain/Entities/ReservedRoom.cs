namespace Domain.Entities
{
    public class ReservedRoom
    {
        public int ReservedRoomID { get; set; }
        public Guid ReservationID { get; set; }
        public int RoomID { get; set; }

        public Reservation Reservation { get; set; }
        public Room Room { get; set; }
    }
}