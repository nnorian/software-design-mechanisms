// proxy
// same face as the real source
// remembers previous checks
// the search runs only once for each password

public class CachedBreachedPasswordSource : IBreachedPasswordSource{

    private readonly IBreachedPasswordSource _realSource;
    private readonly Dictionary<string, bool> _cache = new();
    private readonly TextWriter? _log;

    // log is optional, the program gives Console.Out to show when the cache is used
    public CachedBreachedPasswordSource(IBreachedPasswordSource realSource, TextWriter? log = null){

        _realSource = realSource;
        _log = log;
    }

    public bool IsBreached(string password){

        if (_cache.ContainsKey(password)){
            _log?.WriteLine("[proxy] password was checked before, answer taken from the cache");
        }
        else{
            _log?.WriteLine("[proxy] new password, searching the leaked passwords list...");
            _cache[password] = _realSource.IsBreached(password);
        }

        return _cache[password];
    }

}
