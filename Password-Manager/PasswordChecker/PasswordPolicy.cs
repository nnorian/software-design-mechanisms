// facade
// one method instead of multiple checks 

public class PasswordPolicy{
    private readonly PasswordValidator _validator;

    public PasswordPolicy(string folder){
        var checks = new List<IPasswordCheck>{

            new MinLengthCheck(12),
            new CharactersCheck(),
            new UsernameDuplicateCheck()
        };

        string txtPath = Path.Combine(folder, "rockyou.txt");
        string zipPath = Path.Combine(folder, "rockyou.zip");

        IBreachedPasswordSource? source = null;
        if (File.Exists(txtPath)) source = new FileBreachedPasswordSource(txtPath);
        else if (File.Exists(zipPath)) source = new ZipBreachedPasswordSource(zipPath);

        if (source != null){
            checks.Add(new BreachedPasswordCheck(new CachedBreachedPasswordSource(source)));
        }

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