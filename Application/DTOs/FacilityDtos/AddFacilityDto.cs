namespace Application.DTOs.FacilityDtos
{
    public class AddFacilityDto
    {
        public string FacilityName { get; set; }
        public int FacilityTypeId { get; set; }
        public List<string> Attributes { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int? ParentFacilityId { get; set; }
    }
}
