namespace Domain.Entities
{
    public class Image
    {
        public int ImageID { get; set; }
        public string ImageUrl { get; set; }
        public int FacilityID { get; set; }

        public Facility Facility { get; set; }
    }
}
