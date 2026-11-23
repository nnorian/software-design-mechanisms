// command
// conclrere command
// removes an entry, remembers it , undo puts it back at the same place 

public class RemoveEntryCommand : IVaultCommand{

    private readonly int _index;
    private VaultEntry? _removed;

    public RemoveEntryCommand(int index){
        _index = index;
    }

    public string Name => "remove entry";

    public void Execute(Vault vault){

        _removed = vault.RemoveAt(_index);
    }

    public void Undo(Vault vault){
        vault.Insert(_index, _removed!);
    }
}
