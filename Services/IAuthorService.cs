using Singularity.Models;

namespace Singularity.Services;

public interface IAuthorService{
    Task<Author> GetOrCreateAuthorAsync(string name);
}