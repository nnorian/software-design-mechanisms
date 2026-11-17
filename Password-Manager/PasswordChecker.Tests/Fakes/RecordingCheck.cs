// fake check for the facade tests
// always gives the same result and writes down when it was called

public class RecordingCheck : IPasswordCheck{
    private readonly string _name;
    private readonly bool _passed;
    private readonly List<string> _callLog;

    public Credentials? Received { get; private set; }

    public RecordingCheck(string name, bool passed, List<string> callLog){
        _name = name;
        _passed = passed;
        _callLog = callLog;
    }

    public CheckResult Check(Credentials credentials){
        Received = credentials;
        _callLog.Add(_name);
        return new CheckResult(_passed, _passed ? "" : _name + " failed");
    }
}
