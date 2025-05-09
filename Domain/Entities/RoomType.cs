namespace Domain.Entities
{
    public class RoomType
    {
        public int RoomTypeID { get; set; }
        public string TypeName { get; set; }

        public ICollection<Room> Rooms { get; set; }
        public ICollection<RoomPricing> RoomPricings { get; set; }
    }
}
