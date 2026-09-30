public class MinLengthCheck : IPasswordCheck{
    private readonly int _minLength;

    public MinLengthCheck(int minLength){
        _minLength = minLength;
    }

    public CheckResult Check(Credentials credentials){
        if (credentials.Password.Length < _minLength){

            return new CheckResult(false, "Password must be at least " + _minLength + " characters long");
        }

        return new CheckResult(true, "");
    }
}