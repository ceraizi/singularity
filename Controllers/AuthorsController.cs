using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Singularity.Data;
using Singularity.Models;

namespace Singularity.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase{
    private readonly AppDbContext _context;
    public AuthorsController(AppDbContext context) => _context = context;

    [HttpPost]
    public async Task<ActionResult<Author>> PostAuthor(Author author){
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        return Ok(author);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Author>>> GetAuthors() => await _context.Authors.ToListAsync();
}