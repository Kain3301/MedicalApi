using MedicalApi.Data;
using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpravkaTypesController : ControllerBase
{
    private readonly MedicalDbContext _dbContext;

    public SpravkaTypesController(MedicalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<SpravkaType>>> GetAll()
    {
        return Ok(await _dbContext.SpravkaTypes.AsNoTracking().ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SpravkaType>> GetById(int id)
    {
        SpravkaType? type = await _dbContext.SpravkaTypes.FindAsync(id);
        if (type == null)
        {
            return NotFound();
        }

        return type;
    }

    [HttpPost]
    public async Task<ActionResult<SpravkaType>> Create(SpravkaType type)
    {
        type.Id = 0;

        ActionResult? validationError = Validate(type);
        if (validationError != null)
        {
            return validationError;
        }

        _dbContext.SpravkaTypes.Add(type);
        await _dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = type.Id }, type);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SpravkaType>> Update(int id, SpravkaType type)
    {
        SpravkaType? existingType = await _dbContext.SpravkaTypes.FindAsync(id);
        if (existingType == null)
        {
            return NotFound();
        }

        ActionResult? validationError = Validate(type);
        if (validationError != null)
        {
            return validationError;
        }

        existingType.Name = type.Name;
        existingType.Price = type.Price;
        existingType.ValidityDays = type.ValidityDays;
        existingType.RequiresDoctorExamination = type.RequiresDoctorExamination;
        existingType.ForChildren = type.ForChildren;

        await _dbContext.SaveChangesAsync();
        return Ok(existingType);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        if (id <= 0)
        {
            return BadRequest("Id вида справки должен быть положительным числом.");
        }

        SpravkaType? type = await _dbContext.SpravkaTypes.FindAsync(id);
        if (type == null)
        {
            return NotFound();
        }

        bool hasSpravki = await _dbContext.Spravki.AnyAsync(s => s.SpravkaTypeId == id);
        if (hasSpravki)
        {
            return Conflict("Нельзя удалить вид справки, который используется в справках.");
        }

        _dbContext.SpravkaTypes.Remove(type);
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("Вид справки нельзя удалить, пока он используется в справках.");
        }

        return NoContent();
    }

    private static ActionResult? Validate(SpravkaType type)
    {
        if (string.IsNullOrWhiteSpace(type.Name))
        {
            return new BadRequestObjectResult("Название вида справки не может быть пустым.");
        }

        if (type.Price < 0)
        {
            return new BadRequestObjectResult("Цена вида справки не может быть отрицательной.");
        }

        if (type.ValidityDays <= 0)
        {
            return new BadRequestObjectResult("Срок действия должен быть больше нуля дней.");
        }

        return null;
    }
}
