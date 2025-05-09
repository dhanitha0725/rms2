namespace Domain.Entities
{
    public class FacilityType
    {
        public int FacilityTypeId { get; set; }
        public string TypeName { get; set; }

        public ICollection<Facility> Facilities { get; set; }
    }
}
