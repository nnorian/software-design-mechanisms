// keeps templates 
// creates new entries by cloning them

public class EntryTemplateRegistry{
    private readonly Dictionary<string, VaultEntry> _templates = new();

    public void Register(string name, VaultEntry template){
        _templates[name] = template;
    }

    public List<string> Names(){
        return _templates.Keys.ToList();
    }

    public VaultEntry Create(string name){
        return _templates[name].Clone();
    }

    public static EntryTemplateRegistry CreateDefault(){
        var registry = new EntryTemplateRegistry();

        registry.Register("work-login", new LoginEntry{
            Title = "Work account",
            Url = "https://sso.company.com",
            Tags = new List<string>{ "work", "sso" }
        });
        registry.Register("social-login", new LoginEntry{
            Title = "Social media",
            Tags = new List<string>{ "personal", "social" }
        });
        registry.Register("wifi", new NoteEntry{
            Title = "Wi-Fi",
            Content = "Network name: \nPassword: ",
            Tags = new List<string>{ "home" }
        });

        return registry;
    }
}