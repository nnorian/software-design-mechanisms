// check fails if the password does not contain uppercase, lowercase, digits and special characters

public class CharactersCheck: IPasswordCheck{
    public CheckResult Check(Credentials credentials){
        bool hasUpper = false;
        bool hasLower = false;
        bool hasDigit = false;
        bool hasSpecial = false;

        foreach (char c in credentials.Password){
            if(char.IsUpper(c)){
                hasUpper = true;
            }
            else if (char.IsLower(c)){
                hasLower = true;

            }
            else if (char.IsDigit(c)){
                hasDigit = true;
            }

            else {
                hasSpecial = true;
            }
        }

        if (!hasUpper || !hasLower || !hasDigit || !hasSpecial){
            return new CheckResult(false, "password must contain uppercase, lowercase, digits and special characters");
        }

        return new CheckResult(true, "");

    }
}