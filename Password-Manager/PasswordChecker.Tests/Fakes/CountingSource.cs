// fake real source for the proxy tests
// counts how many times the real search was done

public class CountingSource : IBreachedPasswordSource{
    private readonly HashSet<string> _breached;
    public int Calls { get; private set; }

    public CountingSource(params string[] breached){
        _breached = new HashSet<string>(breached);
    }

    public bool IsBreached(string password){
        Calls++;
        return _breached.Contains(password);
    }
}
