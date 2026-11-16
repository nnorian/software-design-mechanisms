// proxy
// same face as the real source 
// rembers previous chcks
// the search runs only ones for each password

public class CachedPasswordSource : IBreachedPasswordSource{

    private readonly IBreachedPasswordSource _realSource;
    private readonly Dictionary<string, bool> _cache = new();

    public CachedBreachedPasswordSource(IBreachedPasswordSource realSource){

        _realSource = realSource;
    }

    public bool IsBreached(string password){

        if (!_cache.ContainsKey(password)){
            _cache[password] = _realSource.IsBreached(password);
        }

        return _cache[password];
    }

}