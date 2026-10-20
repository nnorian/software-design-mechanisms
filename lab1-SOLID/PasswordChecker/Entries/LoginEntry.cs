// concrete prototype
// website login

public class LoginEntry : VaultEntry{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string Url { get; set; } = "";

    public override VaultEntry Clone(){
       
        var copy = (LoginEntry)MemberwiseClone();
        copy.Tags = new List<string>(Tags);
        return copy;
    }

    public override string ToString(){
        return "[login] " + Title + " - " + Username + " " + Url + " [" + string.Join(", ", Tags) + "]";
    }
}