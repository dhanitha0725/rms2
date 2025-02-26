using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.DTOs.FacilityDtos
{
    public class AddFacilityDto
    {
        public string FacilityName { get; set; }
        public int FacilityTypeId { get; set; }
        //[FromForm(Name = "Attributes")]
        //[JsonPropertyName("Attributes")]
        //public string AttributesJson { get; set; }
        //[JsonIgnore]
        //public Dictionary<string, object> Attributes =>
        //    JsonSerializer.Deserialize<Dictionary<string, object>>(AttributesJson);
        public Dictionary<string, object> Attributes { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        //public List<IFormFile> Images { get; set; }
    }
}
