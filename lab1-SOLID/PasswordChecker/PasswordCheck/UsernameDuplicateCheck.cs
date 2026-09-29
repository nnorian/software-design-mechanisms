// fails if the password contains the username 
public class UsernameDuplicateCheck : IPasswordCheck{

    public CheckResult Check(Credentials credentials){
        string username = credentials.Username.ToLower();
        string password = credentials.Password.ToLower();

        if (username.Length > 0 && password.Contains(username)){
            return new CheckResult(false, "your password should not include your username");

        }

        return ew CheckResult(true, "");
    }

}