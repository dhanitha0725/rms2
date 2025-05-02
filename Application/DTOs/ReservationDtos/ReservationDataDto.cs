namespace Application.DTOs.ReservationDtos
{
    public class ReservationDataDto
    {
        public int ReservationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public string UserType { get; set; }
    }
}
