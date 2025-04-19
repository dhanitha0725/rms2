namespace Application.DTOs.FacilityDtos
{
    public class FacilityCardDto
    {
        public int FacilityID { get; set; }
        public string FacilityName { get; set; }
        public string Location { get; set; }
        public string? ImageUrl{ get; set; }
        public decimal Price { get; set; }
    }
}
