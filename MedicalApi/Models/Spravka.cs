using System.Text.Json.Serialization;

namespace MedicalApi.Models;

public class Spravka
{
    public int Id { get; set; }
    public int SpravkaTypeId { get; set; }
    public string Name { get; set; } = "";
    public int PatientId { get; set; }
    public decimal Price { get; set; }

    [JsonIgnore]
    public Patient? Patient { get; set; }

    [JsonIgnore]
    public SpravkaType? SpravkaType { get; set; }
}
