#pragma warning disable CS1591

using System.Text.Json.Serialization;

namespace KBC.ViewModels
{
    public class IdLabel
    {
        [JsonPropertyName("label")] public string Label { get; set; }
        [JsonPropertyName("id")] public long Id { get; set; }

        public IdLabel()
        {
        }

        public IdLabel(int id, string label)
        {
            Id = id;
            Label = label;
        }
    }
}