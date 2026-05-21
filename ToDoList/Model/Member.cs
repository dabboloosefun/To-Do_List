using System.Text.Json.Serialization;

public class Member : IHasId
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public Member(int id, string name, string password)
    {
        Id = id;
        Name = name;
        Password = password;
    }
}