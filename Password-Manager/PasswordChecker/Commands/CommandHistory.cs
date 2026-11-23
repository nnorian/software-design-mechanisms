// command
//invoker
// runs the commands on the vault and keeps, them so the last one can be undone

public class CommandHistory{

    private readonly Vault _vault;
    private readonly Stack<IVaultCommand> _done = new();

    public CommandHistory(Vault vault){

        _vault = vault;
    }

    public void Execute(IVaultCommand command){

        command.Execute(_vault);
        _done.Push(command);
    }

    public string Undo(){
        if(!_done.TryPop(out IVaultCommand? command)){
            return "nothing to undo";

        }

        command.Undo(_vault);
        return "undone: " + command.Name;
    }


}