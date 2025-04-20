namespace Application.DTOs.FacilityDtos
{
    public class UpdateFacilityDto
    {
        public string FacilityName { get; set; }
        public string FacilityType { get; set; }
        public List<string> Attributes { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}
