namespace MedicalApi.Models;

public class Medicine
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Manufacturer { get; set; } = "";
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool RequiresPrescription { get; set; }
}
