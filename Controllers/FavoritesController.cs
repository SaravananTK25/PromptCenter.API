using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromptCenter.API.Data;
using PromptCenter.API.models;
using PromptCenter.API.Models;
using System.Security.Claims;

namespace PromptCenter.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FavoritesController : ControllerBase
{
    private readonly AppDbContext _context;

    public FavoritesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetFavorites()
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var favorites = await _context.Favorites
            .Where(f => f.UserId == userId)
            .ToListAsync();

        return Ok(favorites);
    }

    [HttpPost("{promptId}")]
    public async Task<IActionResult> AddFavorite(int promptId)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var exists = await _context.Favorites
            .AnyAsync(f =>
                f.UserId == userId &&
                f.PromptId == promptId);

        if (exists)
        {
            return BadRequest(new
            {
                message = "Prompt is already a favorite."
            });
        }

        var favorite = new Favorite
        {
            UserId = userId,
            PromptId = promptId
        };

        _context.Favorites.Add(favorite);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Favorite added."
        });
    }

    [HttpDelete("{promptId}")]
    public async Task<IActionResult> RemoveFavorite(int promptId)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var favorite = await _context.Favorites
            .FirstOrDefaultAsync(f =>
                f.UserId == userId &&
                f.PromptId == promptId);

        if (favorite == null)
        {
            return NotFound(new
            {
                message = "Favorite not found."
            });
        }

        _context.Favorites.Remove(favorite);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Favorite removed."
        });
    }
}