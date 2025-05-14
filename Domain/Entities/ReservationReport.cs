namespace Domain.Entities
{
    public class ReservationReport
    {
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public int TotalReservations { get; set; }
        public int TotalCompletedReservations { get; set; }
    }
}
