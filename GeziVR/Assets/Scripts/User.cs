
public class User 
{
    public string name;
    public string email;
    public string profileImageUrl;
    public string userId;

    public User(string name, string email, string profileImageUrl, string userId)
    {
        this.name = name;
        this.email = email;
        this.profileImageUrl = profileImageUrl;
        this.userId = userId;
    }
}
