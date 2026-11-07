using MedicalApi.Data;
using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly MedicalDbContext _dbContext;

    public PatientsController(MedicalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<Patient>>> GetAll()
    {
        return Ok(await _dbContext.Patients.AsNoTracking().ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Patient>> GetById(int id)
    {
        Patient? patient = await _dbContext.Patients.FindAsync(id);
        if (patient == null)
        {
            return NotFound();
        }
        return patient;
    }

    [HttpGet("{id}/spravki")]
    public async Task<ActionResult<List<Spravka>>> GetSpravki(int id)
    {
        bool patientExists = await _dbContext.Patients.AnyAsync(p => p.Id == id);
        if (!patientExists)
        {
            return NotFound();
        }

        List<Spravka> spravki = await _dbContext.Spravki
            .AsNoTracking()
            .Where(s => s.PatientId == id)
            .ToListAsync();

        return spravki;
    }

    [HttpPost]
    public async Task<ActionResult<Patient>> Create([FromBody] Patient patient)
    {
        patient.Id = 0;
        patient.Spravki = new List<Spravka>();

        _dbContext.Patients.Add(patient);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Patient>> Update(int id, Patient patient)
    {
        Patient? existingPatient = await _dbContext.Patients.FindAsync(id);
        if (existingPatient == null)
        {
            return NotFound();
        }

        existingPatient.Name = patient.Name;
        existingPatient.LastName = patient.LastName;
        existingPatient.FirstName = patient.FirstName;
        existingPatient.Patronymic = patient.Patronymic;
        existingPatient.BirthDate = patient.BirthDate;
        existingPatient.Phone = patient.Phone;
        existingPatient.Email = patient.Email;
        existingPatient.Address = patient.Address;
        existingPatient.OmsPolicyNumber = patient.OmsPolicyNumber;
        existingPatient.PassportSeries = patient.PassportSeries;
        existingPatient.PassportNumber = patient.PassportNumber;

        await _dbContext.SaveChangesAsync();
        return Ok(existingPatient);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        if (id <= 0)
        {
            return BadRequest("Id пациента должен быть положительным числом.");
        }

        Patient? patient = await _dbContext.Patients.FindAsync(id);
        if (patient == null)
        {
            return NotFound();
        }

        bool hasSpravki = await _dbContext.Spravki.AnyAsync(s => s.PatientId == id);
        if (hasSpravki)
        {
            return Conflict("Нельзя удалить пациента, у которого есть справки.");
        }

        _dbContext.Patients.Remove(patient);
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("Пациента нельзя удалить, пока у него есть связанные справки.");
        }

        return NoContent();
    }
}
