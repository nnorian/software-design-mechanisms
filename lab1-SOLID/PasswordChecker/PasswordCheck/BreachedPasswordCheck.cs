// fails if the password is found in rockyou 

public class BreachedPasswordCheck : IPasswordCheck{
    private readonly IBreachedPasswordSource _source;

    public BreachedPasswordCheck(IBreachedPasswordSource source){
        _source = source;
    }

    public CheckResult Check(Credentials credentials){
        if (_source.IsBreached(credentials.Password)){
            return new CheckResult(false, "password was found in a list of leaked passwords");
        }
        return new CheckResult(true, "");
    }
}
