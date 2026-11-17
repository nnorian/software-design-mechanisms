// Facade: PasswordPolicy hides the validator and the checks behind one method

public class FacadeTests{

    [Fact]
    public void Problems_CallsEveryCheckInOrder_WithTheSameCredentials(){
        var callLog = new List<string>();
        var first = new RecordingCheck("first", true, callLog);
        var second = new RecordingCheck("second", true, callLog);
        var third = new RecordingCheck("third", true, callLog);
        var policy = new PasswordPolicy(new List<IPasswordCheck>{ first, second, third });
        var credentials = new Credentials("bob", "secret");

        policy.Problems(credentials);

        Assert.Equal(new[]{ "first", "second", "third" }, callLog);
        Assert.Same(credentials, first.Received);
        Assert.Same(credentials, third.Received);
    }

    [Fact]
    public void Problems_ReturnsOnlyTheMessagesOfFailedChecks(){
        var callLog = new List<string>();
        var policy = new PasswordPolicy(new List<IPasswordCheck>{
            new RecordingCheck("length", false, callLog),
            new RecordingCheck("characters", true, callLog),
            new RecordingCheck("username", false, callLog)
        });

        List<string> problems = policy.Problems(new Credentials("bob", "x"));

        Assert.Equal(new[]{ "length failed", "username failed" }, problems);
    }

    [Fact]
    public void Problems_IsEmpty_WhenAllChecksPass(){
        var policy = new PasswordPolicy(new List<IPasswordCheck>{
            new MinLengthCheck(12),
            new CharactersCheck(),
            new UsernameDuplicateCheck()
        });

        Assert.Empty(policy.Problems(new Credentials("bob", "Xk9#mP2$vL7!")));
    }

    [Fact]
    public void Problems_ListsEveryProblem_OfAWeakPassword(){
        var policy = new PasswordPolicy(new List<IPasswordCheck>{
            new MinLengthCheck(12),
            new CharactersCheck(),
            new UsernameDuplicateCheck()
        });

        Assert.Equal(3, policy.Problems(new Credentials("bob", "bob1")).Count);
    }
}
