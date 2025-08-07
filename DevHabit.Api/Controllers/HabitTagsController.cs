using DevHabit.Api.Database;
using DevHabit.Api.DTOs.HabitTags;
using DevHabit.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace DevHabit.Api.Controllers;

[ApiController]
[Route("api/habits/{habitId}/tags")]
public sealed class HabitTagsController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpPut("{tagId}")]
    public async Task<ActionResult> AddTagToHabit([FromRoute] string habitId,
        [FromBody] UpsertHabitTagsDto upsertHabitTagsDto)
    {
        var habit = await dbContext.Habits.Include(h => h.HabitTags).FirstOrDefaultAsync(h => h.Id == habitId);
        if (habit is null) return NotFound();

        //All tags of the current habit
        var currentTagIds = habit.HabitTags.Select(ht => ht.TagId).ToHashSet();
        if (currentTagIds.SetEquals(upsertHabitTagsDto.TagIds)) return NoContent();

        //All tags existed for the Ids of the request
        var existingTagIds = await dbContext.Tags.Where(t => upsertHabitTagsDto.TagIds.Contains(t.Id)).Select(t => t.Id)
            .ToListAsync();

        if (existingTagIds.Count != upsertHabitTagsDto.TagIds.Count)
            return BadRequest("One or more tag IDs is invalid.");
        //Add new tags to the habit

        var newTags = upsertHabitTagsDto.TagIds.Except(currentTagIds);

        //Remove tags that are not in the request
        habit.HabitTags.ToList().RemoveAll(ht => !upsertHabitTagsDto.TagIds.Contains(ht.TagId));

        habit.HabitTags.AddRange(newTags.Select(tagId => new HabitTag
        {
            HabitId = habitId,
            TagId = tagId,
            CreatedAtUtc = DateTime.UtcNow
        }));


        await dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{tagId}")]
    public async Task<ActionResult> DeleteHabitTag([FromRoute] string habitId, [FromRoute] string tagId)
    {
        var habit = await dbContext.Habits.Include(h => h.HabitTags).FirstOrDefaultAsync(h => h.Id == habitId);
        if (habit is null) return NotFound();
        var habitTag = habit.HabitTags.FirstOrDefault(ht => ht.TagId == tagId);
        if (habitTag is null) return NotFound();
        habit.HabitTags.Remove(habitTag);

        await dbContext.SaveChangesAsync();
        return Ok();
    }
}