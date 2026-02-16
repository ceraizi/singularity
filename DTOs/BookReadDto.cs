namespace Singularity.DTOs;

public class BookReadDto{
    public int Id {get; set;}
    public string Title {get; set;} = string.Empty;
    public int AuthorId {get; set;}
    public string AuthorName {get; set;} = string.Empty;
}