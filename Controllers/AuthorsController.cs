using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Singularity.Data;
using Singularity.Models;
using Microsoft.AspNetCore.Authorization;
using Singularity.DTOs;

namespace Singularity.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase{
    private readonly AppDbContext _context;
    public AuthorsController(AppDbContext context) => _context = context;

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Author>> PostAuthor(CreateAuthorDto authorDto){
        var author = new Author { Name = authorDto.Name };

        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAuthors), new {id = author.Id}, author);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorReadDto>>> GetAuthors(){
        var authors = await _context.Authors.Include(a => a.Books).ToListAsync();

        var response = authors.Select(a => new AuthorReadDto{
            Id = a.Id,
            Name = a.Name,
            BooksCount = a.Books.Count 
        });

        return Ok(response);
    }
}