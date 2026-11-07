using MedicalApi.Data;
using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpravkiController : ControllerBase
{
    private readonly MedicalDbContext _dbContext;

    public SpravkiController(MedicalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<Spravka>>> GetAll()
    {
        return Ok(await _dbContext.Spravki.AsNoTracking().ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Spravka>> GetById(int id)
    {
        Spravka? spravka = await _dbContext.Spravki.FindAsync(id);
        if (spravka == null)
        {
            return NotFound();
        }
        return spravka;
    }

    [HttpPost]
    public async Task<ActionResult<Spravka>> Create(Spravka spravka)
    {
        spravka.Id = 0;

        bool patientExists = await _dbContext.Patients.AnyAsync(p => p.Id == spravka.PatientId);
        if (!patientExists)
        {
            return BadRequest("Пациент с таким PatientId не найден.");
        }

        SpravkaType? type = await _dbContext.SpravkaTypes.FindAsync(spravka.SpravkaTypeId);
        if (type == null)
        {
            return BadRequest("Вид справки с таким SpravkaTypeId не найден.");
        }

        spravka.Name = type.Name;
        spravka.Price = type.Price;
        _dbContext.Spravki.Add(spravka);
        await _dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = spravka.Id }, spravka);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Spravka>> Update(int id, Spravka spravka)
    {
        Spravka? existingSpravka = await _dbContext.Spravki.FindAsync(id);
        if (existingSpravka == null)
        {
            return NotFound();
        }

        bool patientExists = await _dbContext.Patients.AnyAsync(p => p.Id == spravka.PatientId);
        if (!patientExists)
        {
            return BadRequest("Пациент с таким PatientId не найден.");
        }

        SpravkaType? type = await _dbContext.SpravkaTypes.FindAsync(spravka.SpravkaTypeId);
        if (type == null)
        {
            return BadRequest("Вид справки с таким SpravkaTypeId не найден.");
        }

        existingSpravka.SpravkaTypeId = spravka.SpravkaTypeId;
        existingSpravka.Name = type.Name;
        existingSpravka.PatientId = spravka.PatientId;
        existingSpravka.Price = type.Price;

        await _dbContext.SaveChangesAsync();
        return Ok(existingSpravka);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        if (id <= 0)
        {
            return BadRequest("Id справки должен быть положительным числом.");
        }

        Spravka? spravka = await _dbContext.Spravki.FindAsync(id);
        if (spravka == null)
        {
            return NotFound();
        }

        _dbContext.Spravki.Remove(spravka);
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("Справку не удалось удалить из базы данных.");
        }

        return NoContent();
    }
}
