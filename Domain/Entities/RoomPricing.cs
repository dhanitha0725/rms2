namespace Domain.Entities
{
    public class RoomPricing
    {
        public int RoomPricingID { get; set; }
        public decimal Price { get; set; }
        public string Sector { get; set; } 
        public int FacilityID { get; set; }
        public int RoomTypeID { get; set; }

        public Facility Facility { get; set; }
        public RoomType RoomType { get; set; }
    }
}
