namespace User.Neo4j;

public class User
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Mobile { get; set; }
    public int? CountryCode { get; set; }
}