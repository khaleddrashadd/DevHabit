using DevHabit.Api.Database;
using DevHabit.Api.DTOs;
using DevHabit.Api.DTOs.Habits;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class HabitsController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<HabitsCollectionDto>> GetHabits()
    {
        var habits = await dbContext.Habits.Select(HabitQueries.ProjectToDto()).ToListAsync();

        var habitsCollection = new HabitsCollectionDto
        {
            Items = habits
        };

        return Ok(habitsCollection);
    }

    [HttpGet("{Id}")]
    public async Task<ActionResult<HabitDto?>> GetHabit([FromRoute] string id)
    {
        var habit = await dbContext.Habits.Where(h => h.Id == id).Select(HabitQueries.ProjectToDto())
            .FirstOrDefaultAsync();
        if (habit is null) return NotFound();

        return Ok(habit);
    }

    [HttpPost]
    public async Task<ActionResult<HabitDto?>> CreateHabit([FromBody] CreateHabitDto request)
    {
        var habit = request.ToEntity();
        await dbContext.Habits.AddAsync(habit);
        await dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetHabit), new { id = habit.Id },
            habit.ToDto());
    }

    [HttpPut]
    public async Task<ActionResult<HabitDto?>> UpdateHabit([FromBody] UpdateHabitDto request)
    {
        var habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == request.Id);

        if (habit is null) return NotFound();
        habit.UpdateFromDto(request);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<HabitDto?>> PatchHabit(
        [FromRoute] string id, [FromBody] JsonPatchDocument<HabitDto> patchDocument)
    {
        var habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id);

        if (habit is null) return NotFound();


        var habitDto = habit.ToDto();
        // if patching fails, ModelState will be invalid
        patchDocument.ApplyTo(habitDto, ModelState);
        // validation problem better than bad request
        if (!TryValidateModel(habitDto)) return ValidationProblem(ModelState);

        // update the habit entity from the patched DTO
        habit.Name = habitDto.Name;
        habit.Description = habitDto.Description;
        habit.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<HabitDto?>> DeleteHabit(
        [FromRoute] string id)
    {
        var habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id);

        if (habit is null) return NotFound();

        dbContext.Habits.Remove(habit);

        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}