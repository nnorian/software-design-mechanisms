// factory
// builds the default policy: the Lab 0 checks + the leaked passwords check
// connects the adapter and the proxy

public static class PasswordPolicyFactory{

    public static PasswordPolicy CreateDefault(string folder){
        var checks = new List<IPasswordCheck>{
            new MinLengthCheck(12),
            new CharactersCheck(),
            new UsernameDuplicateCheck()
        };

        IBreachedPasswordSource? source = FindBreachedSource(folder);
        if (source != null){
            checks.Add(new BreachedPasswordCheck(new CachedBreachedPasswordSource(source, Console.Out)));
        }
        else{
            Console.WriteLine("Warning: rockyou.txt and rockyou.zip were not found, the leaked passwords check is skipped");
        }

        return new PasswordPolicy(checks);
    }

    // the unzipped file is a bit faster, so it is used first
    private static IBreachedPasswordSource? FindBreachedSource(string folder){
        string txtPath = Path.Combine(folder, "rockyou.txt");
        string zipPath = Path.Combine(folder, "rockyou.zip");

        if (File.Exists(txtPath)) return new FileBreachedPasswordSource(txtPath);
        if (File.Exists(zipPath)) return new ZipBreachedPasswordSource(zipPath);
        return null;
    }
}
