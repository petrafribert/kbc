using System.Text.Json.Serialization;

namespace KBC.Areas.AutoComplete.Models
{
    public class PovijestPregleda
    {
        [JsonPropertyName("label")] public string Label { get; set; }
        [JsonPropertyName("id")] public int Id { get; set; }

        public PovijestPregleda()
        {
        }

        public PovijestPregleda(int id, string label)
        {
            Id = id;
            Label = label;
        }
    }
}