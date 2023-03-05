
public class User 
{
    public string name;
    public string email;
    public string profileImageUrl;
    public int balance;

    public User(string name, string email, string profileImageUrl)
    {
        this.name = name;
        this.email = email;
        this.profileImageUrl = profileImageUrl;
        this.balance = 0;
    }
}
