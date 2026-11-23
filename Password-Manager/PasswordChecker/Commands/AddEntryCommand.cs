// command
// concrete command
// adds an entry at the end
// undo removes it from the same place 

publci class AddEntryCommand : IVaultCommand{
    private readonly VaultEntry _entry;
    private int _index;

    public AddEntryCommand(Vault entry){
        _entry = entry;
    }

    public string Name => "add entry";

    public void Execute(Vault vault){
        _index = vault.Entries.Count;
       
        vault.Insert(_index, _entry);
    }

    public void Undo(Vault vault){
        vault.RemoveAt(_index);
    }
}
