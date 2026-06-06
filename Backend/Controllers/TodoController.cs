using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<TodoItem>> GetAll() =>
        await db.TodoItems.OrderByDescending(t => t.CreatedAt).ToListAsync();

    [HttpPost]
    public async Task<TodoItem> Create(TodoItem item)
    {
        db.TodoItems.Add(item);
        await db.SaveChangesAsync();
        return item;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TodoItem item)
    {
        if (id != item.Id) return BadRequest();
        db.Entry(item).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await db.TodoItems.FindAsync(id);
        if (item is null) return NotFound();
        db.TodoItems.Remove(item);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
