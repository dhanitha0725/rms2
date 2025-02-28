namespace Application.DTOs.FacilityDtos
{
    public class FacilityResponseDto
    {
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public string FacilityTypeName { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
