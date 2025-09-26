using System.Text.Json.Serialization;

namespace IGrok.Models;

public class Config
{
    [JsonConstructor]
    private Config(int userId, string name, string jsonContent)
    {
        UserId = userId;
        Name = name;
        JsonContent = jsonContent;
    }

    public int Id { get; private set; }

    public string Name { get; private set; }
    public string JsonContent { get; private set; }

    public int UserId { get; private set; }
    public User? User { get; private set; }

    public static Config Create(int userId, string name, string jsonContent) 
        => new(userId, name, jsonContent);

    public void Update(string name, string jsonContent)
    {
        Name = name;
        JsonContent = jsonContent;
    }
}
