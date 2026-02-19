using Microsoft.EntityFrameworkCore;
using Singularity.Data;
using Singularity.Models;

namespace Singularity.Services;

public class AuthorService : IAuthorService{
    private readonly AppDbContext _context;

    public AuthorService(AppDbContext context){
        _context = context;
    }

    public async Task<Author> GetOrCreateAuthorAsync(string name){
        var author = await _context.Authors
            .FirstOrDefaultAsync(a => a.Name.ToLower() == name.ToLower());

        if (author == null){
            author = new Author {Name = name};
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
        }

        return author;
    }
}