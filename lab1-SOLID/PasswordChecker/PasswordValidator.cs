// runs checks and collects the results

public class PasswordValidator{
    private readonly List<IPasswordCheck> _checks;

    public PasswordValidator(List<IPasswordCheck> checks){
        _checks = checks;
    }

    public List<CheckResult> Validate(Credentials credentials){
        var results = new List<CheckResult>();
        foreach (var check in _checks){
            results.Add(check.Check(credentials));
        }
        return results;
    }
}