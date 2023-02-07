
public class User 
{
    public string name;
    public string email;
    public string profileImageUrl;
    public string googleToken;

    public User(string name, string email, string profileImageUrl, string googleToken)
    {
        this.name = name;
        this.email = email;
        this.profileImageUrl = profileImageUrl;
        this.googleToken = googleToken;
    }
}
