// the three patterns together:
// PasswordPolicy (Facade) -> BreachedPasswordCheck -> CachedBreachedPasswordSource (Proxy) -> ZipBreachedPasswordSource (Adapter)

public class ChainTests{

    [Fact]
    public void FacadeProxyAdapter_FindLeakedPasswordInTheZip(){
        using var zip = new TempZip("Summer2024!@#");
        var source = new CachedBreachedPasswordSource(new ZipBreachedPasswordSource(zip.Path));
        var policy = new PasswordPolicy(new List<IPasswordCheck>{ new BreachedPasswordCheck(source) });

        Assert.Equal(
            new[]{ "password was found in a list of leaked passwords" },
            policy.Problems(new Credentials("bob", "Summer2024!@#")));
        Assert.Empty(policy.Problems(new Credentials("bob", "Xk9#mP2$vL7!")));
    }

    [Fact]
    public void FacadeProxyAdapter_SecondCheckOfTheSamePassword_DoesNotOpenTheZipAgain(){
        using var zip = new TempZip("qwerty");
        var log = new StringWriter();
        var source = new CachedBreachedPasswordSource(new ZipBreachedPasswordSource(zip.Path), log);
        var policy = new PasswordPolicy(new List<IPasswordCheck>{ new BreachedPasswordCheck(source) });

        policy.Problems(new Credentials("bob", "qwerty"));
        File.Delete(zip.Path); // if the proxy opened the zip again, it would crash now
        List<string> problems = policy.Problems(new Credentials("bob", "qwerty"));

        Assert.Single(problems);
        Assert.Contains("taken from the cache", log.ToString());
    }
}
