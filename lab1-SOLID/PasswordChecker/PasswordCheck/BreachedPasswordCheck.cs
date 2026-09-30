// falis if the password is found in rockyou 

public class BreachedPasswordCheck : IPasswordCheck{
    private readonly IBreachedPasswordSource _source;

    public FileBreachedPasswordCheck(IBreachedPasswordSource source){
        _source = source;
    }

    public CheckResult Check(Credentials credentials){
        if (_source.IsBreached(credentials.Password)){
            return new CheckResult(false, "password was found in a list of leacked passwords");
        }
        return new CheckResult(true, "");
    }
}
