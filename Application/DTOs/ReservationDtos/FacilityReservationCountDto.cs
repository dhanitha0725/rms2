namespace Application.DTOs.ReservationDtos
{
    public class FacilityReservationCountDto
    {
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public int ReservationCount { get; set; }
    }

    public class FacilityReservationCountsResponse
    {
        public List<FacilityReservationCountDto> FacilityCounts { get; set; } = new List<FacilityReservationCountDto>();
    }
}