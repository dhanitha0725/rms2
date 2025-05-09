namespace Domain.Entities
{
    public class Pricing
    {
        public int PriceID { get; set; }
        public decimal Price { get; set; }
        public string Sector { get; set; }
        public int PackageID { get; set; }

        public Package Package { get; set; }
    }
}