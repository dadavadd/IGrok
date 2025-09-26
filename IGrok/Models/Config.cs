namespace IGrok.Models;

public class Config
{
    public int Id { get; set; }

    public required string Name { get; set; }
    public required string JsonContent { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }
}
