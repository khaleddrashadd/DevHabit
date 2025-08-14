using DevHabit.Api.Database;
using DevHabit.Api.DTOs;
using DevHabit.Api.DTOs.Common;
using DevHabit.Api.DTOs.Habits;
using DevHabit.Api.Entities;
using DevHabit.Api.Services;
using DevHabit.Api.Services.Sort;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = Roles.Member)]
public sealed class HabitsController(ApplicationDbContext dbContext, UserContext userContext) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = Roles.Member)]
    public async Task<ActionResult<PaginationResult<HabitDto>>> GetHabits(
        [FromQuery] HabitQueryParamsDto habitQueryParamsDto, ISortableService<Habit> sortHabitService)
    {
        var userId = await userContext.GetUserAsync();
        if (userId is null) return Unauthorized();
        var (search, type, status, sort, page, pageSize, fields) = habitQueryParamsDto;
        search = search.ToLower();
        var query = dbContext.Habits.Where(h => h.UserId == userId).Where(h =>
                h.Name.ToLower().Contains(search) || (h.Description !=
                    null && h.Description.ToLower().Contains(search))).Where(h => type == null || h.Type == type)
            .Where(h => status == null || h.Status == status);

        var projectedQuery = sortHabitService.ApplySorting(query, sort).Select(HabitQueries.ProjectToDto());


        var habitsCollection = await PaginationResult<HabitDto>.CreateAsync(projectedQuery, page, pageSize);


        return Ok(habitsCollection);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<HabitWithTagsDto?>> GetHabit([FromRoute] string id)
    {
        var userId = await userContext.GetUserAsync();
        if (userId is null) return Unauthorized();
        var habitDto = await dbContext.Habits.Where(h => h.Id == id && h.UserId == userId)
            .Select(HabitQueries.ProjectWithTagsToDto())
            .FirstOrDefaultAsync();
        if (habitDto is null) return NotFound();

        return Ok(habitDto);
    }

    [HttpPost]
    public async Task<ActionResult<HabitDto?>> CreateHabit([FromBody] CreateHabitDto request,
        IValidator<CreateHabitDto> validator, ProblemDetailsFactory problemDetailsFactory)
    {
        // var validationResult = await validator.ValidateAsync(request);
        // this will throw an exception if validation fails

        // and will be handled (caught) by the ValidationExceptionHandler middleware
        var userId = await userContext.GetUserAsync();
        if (userId is null) return Unauthorized();
        await validator.ValidateAndThrowAsync(request);

        var habit = request.ToEntity(userId);
        await dbContext.Habits.AddAsync(habit);
        await dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetHabit), new { id = habit.Id },
            habit.ToDto());
    }

    [HttpPut]
    public async Task<ActionResult<HabitDto?>> UpdateHabit([FromBody] UpdateHabitDto request)
    {
        var userId = await userContext.GetUserAsync();
        if (userId is null) return Unauthorized();
        var habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == request.Id && h.UserId == userId);

        if (habit is null) return NotFound();
        habit.UpdateFromDto(request);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<HabitDto?>> PatchHabit(
        [FromRoute] string id, [FromBody] JsonPatchDocument<HabitDto> patchDocument)
    {
        var userId = await userContext.GetUserAsync();
        if (userId is null) return Unauthorized();
        var habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);

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
    [Authorize(Roles = Roles.Member)]
    public async Task<ActionResult<HabitDto?>> DeleteHabit(
        [FromRoute] string id)
    {
        var userId = await userContext.GetUserAsync();
        if (userId is null) return Unauthorized();
        var habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);

        if (habit is null) return NotFound();

        dbContext.Habits.Remove(habit);

        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}