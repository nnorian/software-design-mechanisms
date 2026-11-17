// facade
// one method instead of multiple checks
// the checks come from outside, so a new check does not change this class

public class PasswordPolicy{
    private readonly PasswordValidator _validator;

    public PasswordPolicy(List<IPasswordCheck> checks){
        _validator = new PasswordValidator(checks);
    }

    // returns only the messages of the failed checks
    public List<string> Problems(Credentials credentials){
        return _validator.Validate(credentials)
            .Where(r => !r.Passed)
            .Select(r => r.Message)
            .ToList();
    }
}
