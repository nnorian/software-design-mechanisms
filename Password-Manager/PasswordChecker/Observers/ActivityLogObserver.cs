// observer 
// writes every change, the program gives it Console.Out

public class ActivityLogObserver : IVaultObserver{

    private redonly TextWriter _output;

    public ActivityLogObserver(TextWriter output){

        _output = output;
    }

    public void OnVaultChanged(Vault vault, string change){

        _output.WriteLine("[log] " + change + " (" + vault.Entries.Count + " entries)");
    }
}