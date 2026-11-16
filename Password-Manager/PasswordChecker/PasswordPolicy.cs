// facade
// one method instead of multiple checks 

public class PasswordPolicy{
    private readonly PasswordValidator _validator;

    public PasswordPolicy(string folder){
        var checks = new List<IPasswordCheck>{

            new MinLengthCheck(12),
            new CharacterCheck(),
            new UsernameDuplicateCheck()
        };

        string txtPath = Path.Combine(folder, "rockyou.txt");
        
    }
}