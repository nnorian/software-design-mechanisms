# Laboratory Work Nr. 1 — SOLID Principles

**Student:** Kushnirenko Ecaterina
**Academic group:** FAF-243

## Introduction

SOLID is a set of 5 principles for object oriented design. They were introduced by Robert C. Martin. The idea is to write code that is easy to read, change and extend.

- Single Responsibility Principle: a class should have only one reason to change, so it does only one job.
- Open/Closed Principle: a class should be open for extension, but closed for modification. New behavior is added with new code, not by changing the old code.
- Liskov Substitution Principle: an object of a child class should work in every place where the parent class is expected, without breaking the program.
- Interface Segregation Principle: it is better to have many small interfaces than one big interface. A class should not implement methods that it does not use.
- Dependency Inversion Principle: high level classes should not depend on low level classes. Both should depend on abstractions (interfaces).

In this laboratory I had to implement 3 of these principles. I chose S O and D.

## The project

The project is a simple console password checker written in C# (.NET 8). The user introduces a username and a password. The program checks the password with 4 rules, shows which rules failed and says if the password is good or bad.

The rules are:

1. the password has at least 12 characters;
2. the password has uppercase, lowercase, digits and special characters;
3. the password does not include the username;
4. the password is not in `rockyou.txt`, a list of leaked passwords.

Project structure:

```
PasswordChecker/
├── Program.cs                        creates the objects and shows the result
├── LoginReader.cs                    reads username and password from the console
├── Credentials.cs                    stores username and password
├── CheckResult.cs                    stores the result of one check
├── PasswordValidator.cs              runs all checks and collects the results
├── rockyou.zip                       leaked passwords list
└── PasswordCheck/
    ├── IPasswordCheck.cs             interface for all checks
    ├── MinLengthCheck.cs             checks the length
    ├── CharactersCheck.cs            checks the types of characters
    ├── UsernameDuplicateCheck.cs     checks if the password includes the username
    ├── BreachedPasswordCheck.cs      checks if the password was leaked
    ├── IBreachedPasswordSource.cs    interface for the leaked passwords source
    └── FileBreachedPasswordSource.cs searches the password in rockyou.txt
```

### How to run

`rockyou.txt` is too big for GitHub (140 MB), so in the repository there is only `rockyou.zip`. It has to be unzipped before running:

```bash
cd PasswordChecker
unzip rockyou.zip
dotnet run
```

Example of a bad password:

```
Username: alice
Password: password
Password must be at least 12 characters long
password must contain uppercase, lowercase, digits and special characters
password was found in a list of leaked passwords
Bad password
```

Example of a good password:

```
Username: bob
Password: K7#mQz!vR2pL
Good password
```

## Where the principles are used

| Principle | Where                                                                                                                   |
|-----------|-------------------------------------------------------------------------------------------------------------------------|
| SRP       | every class: `LoginReader`, each check in `PasswordCheck/`, `FileBreachedPasswordSource`, `PasswordValidator`, `Program.cs` |
| OCP       | `IPasswordCheck`, `PasswordValidator` and the list of checks in `Program.cs`                                            |
| DIP       | `BreachedPasswordCheck` with `IBreachedPasswordSource`, `PasswordValidator` with `IPasswordCheck`, objects created in `Program.cs` |

## Single Responsibility Principle

Every class in the project does only one thing.

`LoginReader` only reads the credentials. It does not know anything about the checks:

```csharp
public class LoginReader{
    public Credentials Read(){
        Console.Write("Username: ");
        var username = Console.ReadLine() ?? "";
        Console.Write("Password: ");
        var password = Console.ReadLine() ?? "";
        return new Credentials(username, password);
    }
}
```

Every check is a separate class with one rule. For example, `MinLengthCheck` only checks the length:

```csharp
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
```

If I want to read the credentials from a file, I change only `LoginReader`. If the length rule changes, I change only `MinLengthCheck`. Without SRP all this code would be in one big class with many `if`s, and every change would touch the same class.

## Open/Closed Principle

All checks implement the same interface:

```csharp
public interface IPasswordCheck{
    CheckResult Check(Credentials credentials);
}
```

`PasswordValidator` does not know which checks exist. It only goes through a list of `IPasswordCheck`:

```csharp
public List<CheckResult> Validate(Credentials credentials){
    var results = new List<CheckResult>();
    foreach (var check in _checks){
        results.Add(check.Check(credentials));
    }
    return results;
}
```

The checks are added in `Program.cs`:

```csharp
var checks = new List<IPasswordCheck>();
checks.Add(new MinLengthCheck(12));
checks.Add(new CharactersCheck());
checks.Add(new UsernameDuplicateCheck());
checks.Add(new BreachedPasswordCheck(new FileBreachedPasswordSource(rockYouPath)));
```

So the project is open for extension. To add a new rule, I only create a new class. For example, a check for spaces.
and add one line in `Program.cs` like:

```csharp
checks.Add(new NoSpacesCheck());
```

The project is also closed for modification as `PasswordValidator` and the other checks stay the same.

## Dependency Inversion Principle

`BreachedPasswordCheck` needs to know if a password was leaked, but it does not read the file itself. It depends on the interface `IBreachedPasswordSource`:

```csharp
public interface IBreachedPasswordSource{
    bool IsBreached(string password);
}
```

```csharp
public class BreachedPasswordCheck : IPasswordCheck{
    private readonly IBreachedPasswordSource _source;

    public BreachedPasswordCheck(IBreachedPasswordSource source){
        _source = source;
    }

    public CheckResult Check(Credentials credentials){
        if (_source.IsBreached(credentials.Password)){
            return new CheckResult(false, "password was found in a list of leaked passwords");
        }
        return new CheckResult(true, "");
    }
}
```

The work with the file is in a separate class, `FileBreachedPasswordSource`. It reads `rockyou.txt` line by line:

```csharp
public class FileBreachedPasswordSource: IBreachedPasswordSource{
    private readonly string _path;

    public FileBreachedPasswordSource(string path){
        _path = path;
    }

    public bool IsBreached(string password){
        foreach (var line in File.ReadLines(_path)){
            if (line == password){
                return true;
            }
        }
        return false;
    }
}
```

The source is added in `Program.cs` and is given to the check through the constructor:

```csharp
string rockYouPath = Path.Combine(AppContext.BaseDirectory, "rockyou.txt");
// ...
checks.Add(new BreachedPasswordCheck(new FileBreachedPasswordSource(rockYouPath)));
```

So the high level class (`BreachedPasswordCheck`) and the low level class (`FileBreachedPasswordSource`) both depend on the abstraction `IBreachedPasswordSource`. If later I want to check the passwords with an online API instead of rockyou, you only write a new class that implements `IBreachedPasswordSource` and change one line in `Program.cs`. `BreachedPasswordCheck` stays the same.

The same idea is used in `PasswordValidator`. It depends on `IPasswordCheck`, not on `MinLengthCheck` or `CharactersCheck`.

## Conclusion

In this laboratory I implemented a simple password checker and used 3 SOLID principles: SRP, OCP and DIP.

- Because of single responsability principle, the classes are small and each of them has one job, so the code is easy to read and to change.
- Because of Open/Closed Principle, I can add a new password rule with a new class, without changing `PasswordValidator`.
- Because of dependency inversion principle, the breach check does not depend on the `rockyou.txt` file, so the source of leaked passwords can be changed easily.

The project also follows LSP and ISP a bit. Every check can be used where an `IPasswordCheck` is expected, and both interfaces are small, with only one method.

I understood that SOLID makes the project have more files, but every file is simple. It is easier to extend the program without breaking what already works.
