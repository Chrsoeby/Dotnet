namespace Api.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Status { get; set; } = "Want to Read"; // Options: "Want to Read", "Reading", "Read"
}