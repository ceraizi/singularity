using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Singularity.Data;
using Singularity.DTOs;
using Singularity.Models;
using Microsoft.AspNetCore.Authorization;
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
        var query = _context.Books.Include(b => b.Author).AsQueryable();

        if (!string.IsNullOrWhiteSpace(title)){
            query = query.Where(b => b.Title.ToLower().Contains(title.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(author)){
            query = query.Where(b => b.Author != null && b.Author.Name.ToLower().Contains(author.ToLower()));
        }

        var books = await query.ToListAsync();

        var response = books.Select(b => new BookReadDto{
            Id = b.Id,
            Title = b.Title,
            AuthorId = b.AuthorId,
            AuthorName = b.Author?.Name ?? "Unknown Author"
        });

        return Ok(response);
    }


    // GET: api/books/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBook(int id){
        var book = await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id);

        if (book == null){
            throw new AppException("Book not found", 404);
        }

        return book;
    }

    // POST: api/books
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Book>> PostBook(CreateBookDto bookDto){
        var author = await _context.Authors.FirstOrDefaultAsync(a => a.Name.ToLower() == bookDto.AuthorName.ToLower());

        if (author == null){
            author = new Author { Name = bookDto.AuthorName };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
        }

        var book = new Book{
            Title = bookDto.Title,
            AuthorId = author.Id
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBook), new {id = book.Id}, book);
    }

    // PUT: api/books/{id}
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, CreateBookDto bookDto){
        var book = await _context.Books.FindAsync(id);

        if (book == null){
            throw new AppException("Book not found", 404);
        }

        var author = await _context.Authors.FirstOrDefaultAsync(a => a.Name.ToLower() == bookDto.AuthorName.ToLower());

        if (author == null){
            author = new Author { Name = bookDto.AuthorName };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
        }

        book.Title = bookDto.Title;
        book.AuthorId = author.Id;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/books/{id}
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id){
        var book = await _context.Books.FindAsync(id);

        if (book == null){
            throw new AppException("Book not found", 404);
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}