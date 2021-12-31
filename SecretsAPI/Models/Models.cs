namespace SecretsAPI.Models;

public class ChangePassword
{
    public ChangePassword ()
    {
        this.Password = string.Empty;
        this.PasswordConfirm = string.Empty;
    }
    public string Password { get; set; }
    public string PasswordConfirm { get; set; }
}

public class Login
{
    public Login ()
    {
        this.Username = string.Empty;
        this.Password = string.Empty;
    }
    public string Username { get; set; }
    public string Password { get; set; }

    public bool IsValid()
    {
        int maxLength = 30;
        if(string.IsNullOrWhiteSpace(this.Username) || 
        string.IsNullOrWhiteSpace(this.Password))
        {
            return false;
        }
        if(this.Username.Length > maxLength || this.Password.Length > maxLength)
        {
            return false;
        }
        return true;
    }
}

public class User
{
    public User ()
    {
        this.Id = Guid.NewGuid();
        this.DateCreated = DateTime.Now;
        this.Username = string.Empty;
    }
    public Guid Id { get; set; }
    public string Username { get; set; }
    public DateTime DateCreated { get; set; }
    public bool IsActive { get; set; }
}

public class Secret
{
    public Secret()
    {
        this.Id = Guid.NewGuid();
        this.DateCreated = DateTime.Now;
        this.DateModified = DateTime.Now;
        this.Content = string.Empty;
    }
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateModified { get; set; }
    public string Content { get; set; }
}