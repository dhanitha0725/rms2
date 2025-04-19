namespace Domain.Entities
{
    public class Room
    {
        public int RoomID { get; set; }
        public string Type { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; }
        public int? NumberOfBeds { get; set; }
        public int FacilityID { get; set; }

        public Facility Facility { get; set; }
        public ICollection<ReservedRoom> ReservedRooms { get; set; }
    }
}