namespace Application.DTOs.FacilityDtos
{
    public class FullFacilityDetailsDto
    {
        public int FacilityID { get; set; }
        public string FacilityName { get; set; }
        public string FacilityType { get; set; }
        public string Location { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<string> Attributes { get; set; } = new();
        public List<ChildFacilityDto> ChildFacilities { get; set; } = new();
        public List<string> Images { get; set; } = new();
    }

    public class ChildFacilityDto
    {
        public int ChildrenFacilityId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
