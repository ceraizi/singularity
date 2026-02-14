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

    public BooksController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/books
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks(){
        return await _context.Books.ToListAsync();
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