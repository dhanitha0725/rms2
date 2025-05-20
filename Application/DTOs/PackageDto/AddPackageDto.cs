namespace Application.DTOs.PackageDto
{
    public class AddPackageDto
    {
        public string PackageName { get; set; }
        public TimeSpan? Duration { get; set; }
        public Dictionary<string, decimal> Pricings { get; set; }
    }

    public class UpdatePackageDto
    {
        public string PackageName { get; set; }
        public TimeSpan? Duration { get; set; }
        public Dictionary<string, decimal> Pricing { get; set; }
    }
}
