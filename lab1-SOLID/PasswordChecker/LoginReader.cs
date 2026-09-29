public class LoginReader{
    public Credentials Read(){
        Console.Write("Username: ");
        var username = Console.ReadLine();
        Console.Write("Password: ");
        var password = Console.ReadLine();
        return new Credentials(username, password);
    }
}