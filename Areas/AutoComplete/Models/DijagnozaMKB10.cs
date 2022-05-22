using System.Text.Json.Serialization;

namespace KBC.Areas.AutoComplete.Models
{
    public class DijagnozaMKB10
    {

        [JsonPropertyName("label")]
        public string Label { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        public DijagnozaMKB10() { }
        public DijagnozaMKB10(string MKB10, string label)
        {
            Id = MKB10;
            Label = label;
        }

    }
}
