using MedicalApi.Data;
using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/medicines")]
public class MedicinesController : ControllerBase
{
    private readonly MedicalDbContext _dbContext;

    public MedicinesController(MedicalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<Medicine>>> GetAll()
    {
        return Ok(await _dbContext.Medicines.AsNoTracking().ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Medicine>> GetById(int id)
    {
        Medicine? medicine = await _dbContext.Medicines.FindAsync(id);
        if (medicine == null)
        {
            return NotFound();
        }

        return medicine;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Medicine), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Medicine>> Create(Medicine medicine)
    {
        medicine.Id = 0;

        ActionResult? validationError = Validate(medicine);
        if (validationError != null)
        {
            return validationError;
        }

        _dbContext.Medicines.Add(medicine);
        await _dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = medicine.Id }, medicine);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Medicine>> Update(int id, Medicine medicine)
    {
        Medicine? existingMedicine = await _dbContext.Medicines.FindAsync(id);
        if (existingMedicine == null)
        {
            return NotFound();
        }

        ActionResult? validationError = Validate(medicine);
        if (validationError != null)
        {
            return validationError;
        }

        existingMedicine.Name = medicine.Name;
        existingMedicine.Manufacturer = medicine.Manufacturer;
        existingMedicine.Price = medicine.Price;
        existingMedicine.Quantity = medicine.Quantity;
        existingMedicine.ExpirationDate = medicine.ExpirationDate;
        existingMedicine.RequiresPrescription = medicine.RequiresPrescription;

        await _dbContext.SaveChangesAsync();
        return Ok(existingMedicine);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        if (id <= 0)
        {
            return BadRequest("Id лекарства должен быть положительным числом.");
        }

        Medicine? medicine = await _dbContext.Medicines.FindAsync(id);
        if (medicine == null)
        {
            return NotFound();
        }

        _dbContext.Medicines.Remove(medicine);
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("Лекарство не удалось удалить из базы данных.");
        }

        return NoContent();
    }

    private static ActionResult? Validate(Medicine medicine)
    {
        if (string.IsNullOrWhiteSpace(medicine.Name))
        {
            return new BadRequestObjectResult("Название лекарства не может быть пустым.");
        }

        if (string.IsNullOrWhiteSpace(medicine.Manufacturer))
        {
            return new BadRequestObjectResult("Производитель лекарства не может быть пустым.");
        }

        if (medicine.Price < 0)
        {
            return new BadRequestObjectResult("Цена лекарства не может быть отрицательной.");
        }

        if (medicine.Quantity < 0)
        {
            return new BadRequestObjectResult("Количество лекарства не может быть отрицательным.");
        }

        if (medicine.ExpirationDate.Date < DateTime.Today)
        {
            return new BadRequestObjectResult("Срок годности лекарства уже истёк.");
        }

        return null;
    }
}
