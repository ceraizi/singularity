using System.ComponentModel.DataAnnotations;

namespace Singularity.DTOs;

public class CreateAuthorDto{
    [Required(ErrorMessage = "The author's name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "The name should have a minimum of 2 characters, and a maximum of 100")]
    public string Name {get; set;} = string.Empty;
}