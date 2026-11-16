// using the created data type CheckResult we will check the saved there credentials 
public interface IPasswordCheck{
    CheckResult Check(Credentials credentials);
}