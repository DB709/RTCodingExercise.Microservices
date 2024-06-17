using System;
using System.Text.Json.Serialization;

namespace WebMVC.Models
{
    public class PlateListModel
    {
        [JsonPropertyName("value")]
        public IEnumerable<Plate> Plates { get; set; }
        [JsonPropertyName("@odata.count")]
        public int TotalPlates { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalPlates / (double)PageSize);
    }
}
