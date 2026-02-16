using System.ComponentModel.DataAnnotations;

namespace Singularity.DTOs;

public class CreateBookDto{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, MinimumLength = 1)]
    public string Title {get; set;} = string.Empty;

    [Required]
    public int AuthorId {get; set;}
}