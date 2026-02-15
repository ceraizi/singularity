using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Singularity.Data;
using Singularity.DTOs;
using Singularity.Models;

namespace Singularity.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase{
    private readonly AppDbContext _context;

    public BooksController(AppDbContext context){
        _context = context;
    }


    // GET: api/books?title=...&author=...
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks([FromQuery] string? title, [FromQuery] string? author){
        var query = _context.Books.AsQueryable();

        if (!string.IsNullOrWhiteSpace(title)){
            query = query.Where(b => b.Title.ToLower().Contains(title.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(author)){
            query = query.Where(b => b.Author.ToLower().Contains(author.ToLower()));
        }

        return await query.ToListAsync();
    }


    // GET: api/books/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBook(int id){
        var book = await _context.Books.FindAsync(id);

        if (book == null){
            throw new AppException("Book not found", 404);
        }

        return book;
    }

    // POST: api/books
    [HttpPost]
    public async Task<ActionResult<Book>> PostBook(CreateBookDto bookDto){
        var book = new Book{
            Title = bookDto.Title,
            Author = bookDto.Author
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBooks), new {id = book.Id}, book);
    }
}