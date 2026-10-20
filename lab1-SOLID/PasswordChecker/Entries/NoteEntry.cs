// concrete prototype
// secret note 

public class NoteEntry : VaultEntry{
    public string Content { get; set; } = "";

    public override VaultEntry Clone(){
        var copy = (NoteEntry)MemberwiseClone();
        copy.Tags = new List<string>(Tags);
        return copy;
    }

    public override string ToString(){
        return "[note] " + Title + " [" + string.Join(", ", Tags) + "]";
    }
}