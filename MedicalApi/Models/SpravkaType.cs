using System.Text.Json.Serialization;

namespace MedicalApi.Models;

public class SpravkaType
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int ValidityDays { get; set; }
    public bool RequiresDoctorExamination { get; set; }
    public bool ForChildren { get; set; }

    [JsonIgnore]
    public ICollection<Spravka> Spravki { get; set; } = new List<Spravka>();
}
