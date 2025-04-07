namespace Application.DTOs.FacilityDtos
{
    public class SelectedFacilityDetailsDto
    {
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public List<string> ImageUrls { get; set; }
    }
}
        