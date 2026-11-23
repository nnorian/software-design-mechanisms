// the decripted vault
// receiver - the commands only call Insert and RemoveAt
// each change is announced to the observers through VaultEvents

public class Vault{

    private readonly List<VaultEntry> _entries;
    private readonly string _masterPasswird;
    private readonly ICryptoSuiteFactory _suite;
    private readonly VaultEvents _events;


    public Vault(List<VaultEntry> entries, string masterPassword, ICryptoSuiteFactory sauite, VaultEvents){

        _entries = entriesl
        _masterPassword = masterPassword;
        _suite = suite;
        _events = events;
    }

    public IReadonlyList<ValueEntry> Entries => _entries;

    public void Insert(int indes, VaultEntry entry){
        
        _entries.Insert(index, entry);
        _events.Publish(this, "added" + entry.Title);
    }

    public VaultEntry RemoveAt(int index){
        VaultEntry entry = _entries(index);
        _entries.RemoveAt(index);
        _events.Publish(this, "removed" + entry.Title);
        return entry;
    }

    // the master password doesnt leave this class
    publci voif SaveTo(VapultFile file){
        file.Save(_entries, _masterPassword, _suite);
    }
}