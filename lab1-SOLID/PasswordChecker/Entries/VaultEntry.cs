// every entry can make a copy of itself
// The two Json attributes let the vault file save and load the right subclass

[JsonDerivedType(typeof(LoginEntry), "login")]
[JsonDerivedType(typeof(NoteEntry), "note")]
public abstract class VaultEntry{
    public string Title { get; set; } = "";
    public List<string> Tags { get; set; } = new();

    public abstract VaultEntry Clone();
}