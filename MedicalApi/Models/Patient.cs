using System.Text.Json.Serialization;

namespace MedicalApi.Models;

public class Patient
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string LastName { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string Patronymic { get; set; } = "";
    public DateTime BirthDate { get; set; }
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string Address { get; set; } = "";
    public string OmsPolicyNumber { get; set; } = "";
    public string PassportSeries { get; set; } = "";
    public string PassportNumber { get; set; } = "";

    [JsonIgnore]
    public ICollection<Spravka> Spravki { get; set; } = new List<Spravka>();
}
