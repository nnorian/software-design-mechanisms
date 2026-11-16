# Laboratory Work Nr. 3 — Structural Design Patterns

**Student:** Kushnirenko Ecaterina
**Academic group:** FAF-243

## Introduction

Structural design patterns show how classes and objects can be put together into bigger structures, while the structures stay simple and flexible.

There are 7 structural patterns:

- Adapter: converts the interface of a class into another interface that the client expects.
- Bridge: splits one big class into two hierarchies (abstraction and implementation) that can change separately.
- Composite: puts objects into tree structures and lets the client use one object and a group of objects in the same way.
- Decorator: adds new behavior to an object by wrapping it in another object with the same interface.
- Facade: gives one simple interface to a complicated set of classes.
- Flyweight: shares the common part of many objects, so they use less memory.
- Proxy: an object with the same interface as the real object, that controls the access to it (for example lazy loading, caching or protection).

In this laboratory I had to implement 3 of these patterns. I chose Adapter, Proxy and Facade.

## The project

The project is the password manager from the previous laboratories. The old features stay the same: check a password, generate a password, create and open an encrypted vault, add entries from templates.

The new patterns are used in the part that checks passwords (Lab 0). There were 2 problems there:

1. The leaked passwords list `rockyou.txt` is 140 MB, so the repository has only `rockyou.zip`. Before, the user had to unzip it by hand. If the file was not unzipped, the leaked passwords check was skipped without any message.
2. Every time a password was checked, the program searched the whole list again, even for a password that was already checked.

Also, `Program.cs` built the list of checks itself and filtered the failed results in two different places (`CheckPassword` and `CreateVault`).

The 3 patterns solve these problems:

| Pattern | Class                                         | What it does                                                         |
|---------|-----------------------------------------------|----------------------------------------------------------------------|
| Adapter | `PasswordCheck/ZipBreachedPasswordSource.cs`  | reads the passwords directly from `rockyou.zip`, no unzip is needed   |
| Proxy   | `PasswordCheck/CachedBreachedPasswordSource.cs` | remembers the answers, so every password is searched only once      |
| Facade  | `PasswordPolicy.cs`                           | one method `Problems(credentials)` instead of all the check classes   |

The objects work together like a chain:

```
Program.cs
   │  policy.Problems(credentials)
   ▼
PasswordPolicy                    (Facade)
   │  creates the checks and the PasswordValidator
   ▼
BreachedPasswordCheck             (Lab 0, not changed)
   │  IBreachedPasswordSource
   ▼
CachedBreachedPasswordSource      (Proxy)
   │  IBreachedPasswordSource
   ▼
ZipBreachedPasswordSource         (Adapter)
   │  ZipArchive
   ▼
rockyou.zip
```

New and changed files:

```
PasswordChecker/
├── Program.cs                              changed: uses the facade
├── PasswordChecker.csproj                  changed: copies rockyou.zip to the output folder
├── PasswordPolicy.cs                       new: Facade
└── PasswordCheck/
    ├── ZipBreachedPasswordSource.cs        new: Adapter
    └── CachedBreachedPasswordSource.cs     new: Proxy
```

All the other classes did not change.

### How to run

Now there is no need to unzip anything:

```bash
cd PasswordChecker
dotnet run
```

If `rockyou.txt` is unzipped next to the program, it is used, because reading a normal file is a bit faster. If not, the program reads `rockyou.zip`.

Example (the password is in rockyou, so the second check gives the answer from the cache):

```
> 1
Username: bob
Password: ABCabc123!@#
password was found in a list of leaked passwords
Bad password

> 1
Username: bob
Password: ABCabc123!@#
password was found in a list of leaked passwords
Bad password
```

## Adapter

The client (`BreachedPasswordCheck`) needs an `IBreachedPasswordSource`:

```csharp
public interface IBreachedPasswordSource{
    bool IsBreached(string password);
}
```

The .NET class `ZipArchive` can read the zip, but it has a completely different interface: it has entries, streams and so on. It does not know anything about passwords. The adapter connects them. It implements `IBreachedPasswordSource` (the target) and inside it uses `ZipArchive` (the adaptee):

```csharp
using System.IO.Compression;

public class ZipBreachedPasswordSource : IBreachedPasswordSource{
    private readonly string _zipPath;

    public ZipBreachedPasswordSource(string zipPath){
        _zipPath = zipPath;
    }

    public bool IsBreached(string password){
        using ZipArchive zip = ZipFile.OpenRead(_zipPath);
        using var reader = new StreamReader(zip.Entries[0].Open());

        while (reader.ReadLine() is string line){
            if (line == password){
                return true;
            }
        }
        return false;
    }
}
```

The zip has only one file inside, so the adapter opens `zip.Entries[0]`. The file is unpacked while it is read, line by line, so the 140 MB are never kept in memory and nothing is written to the disk.

`BreachedPasswordCheck` did not change. It does not know that the passwords now come from a zip. This was possible because of the Dependency Inversion Principle from Lab 0: the check depends on the interface, so any new source can be plugged in.

To make the zip available next to the program, one line was added to `PasswordChecker.csproj`:

```xml
<None Update="rockyou.zip" CopyToOutputDirectory="PreserveNewest" />
```

## Proxy

Searching 14 million lines takes about a second. The same password is often checked more than once: for example the user checks it with option 1, and then uses it as the master password with option 3.

`CachedBreachedPasswordSource` is a caching proxy. It has the same interface as the real source, so the check can not see the difference. It keeps the real source inside and calls it only for passwords that it has not seen before:

```csharp
public class CachedBreachedPasswordSource : IBreachedPasswordSource{
    private readonly IBreachedPasswordSource _realSource;
    private readonly Dictionary<string, bool> _cache = new();

    public CachedBreachedPasswordSource(IBreachedPasswordSource realSource){
        _realSource = realSource;
    }

    public bool IsBreached(string password){
        if (!_cache.ContainsKey(password)){
            _cache[password] = _realSource.IsBreached(password);
        }
        return _cache[password];
    }
}
```

The proxy works with every source, with `FileBreachedPasswordSource` and with `ZipBreachedPasswordSource`. I tested it: checking the same password 4 times takes the same time as checking it once, because the list is searched only the first time.

The Proxy and the Decorator have the same structure (a wrapper with the same interface). The difference is the purpose. A Decorator adds new behavior to the result. This class does not change the result at all, it only controls when the real object is called. That is why it is a Proxy.

## Facade

Before, `Program.cs` had to know all the classes from Lab 0: the 4 checks, `PasswordValidator`, `FileBreachedPasswordSource`, `CheckResult`. It also had to filter the failed results in two places.

`PasswordPolicy` hides all of this behind one constructor and one method:

```csharp
public class PasswordPolicy{
    private readonly PasswordValidator _validator;

    public PasswordPolicy(string folder){
        var checks = new List<IPasswordCheck>{
            new MinLengthCheck(12),
            new CharactersCheck(),
            new UsernameDuplicateCheck()
        };

        string txtPath = Path.Combine(folder, "rockyou.txt");
        string zipPath = Path.Combine(folder, "rockyou.zip");

        IBreachedPasswordSource? source = null;
        if (File.Exists(txtPath)) source = new FileBreachedPasswordSource(txtPath);
        else if (File.Exists(zipPath)) source = new ZipBreachedPasswordSource(zipPath);

        if (source != null){
            checks.Add(new BreachedPasswordCheck(new CachedBreachedPasswordSource(source)));
        }

        _validator = new PasswordValidator(checks);
    }

    public List<string> Problems(Credentials credentials){
        return _validator.Validate(credentials)
            .Where(r => !r.Passed)
            .Select(r => r.Message)
            .ToList();
    }
}
```

The facade is also the place where the Adapter and the Proxy are connected. It is created only once in `Program.cs`, so the cache of the proxy is kept while the program runs. Before, the checks were created again at every call.

Now `Program.cs` only talks to the facade:

```csharp
var policy = new PasswordPolicy(AppContext.BaseDirectory);

void CheckPassword(){
    List<string> problems = policy.Problems(new LoginReader().Read());
    problems.ForEach(Console.WriteLine);
    Console.WriteLine(problems.Count == 0 ? "Good password" : "Bad password");
}
```

and in `CreateVault`:

```csharp
List<string> problems = policy.Problems(new Credentials("", masterPassword));
if (problems.Count > 0){
    problems.ForEach(Console.WriteLine);
    Console.WriteLine("Choose a stronger master password");
    return;
}
```

The old local function `Validate` and the variable `rockYouPath` were removed from `Program.cs`, so `Program.cs` became shorter. The classes behind the facade did not change. They can still be used directly if someone needs the full `CheckResult` list.

## Conclusion

In this laboratory I added 3 structural patterns to the password manager: Adapter, Proxy and Facade.

- Because of the Adapter, the program reads the leaked passwords directly from `rockyou.zip`, so the user does not have to unzip a 140 MB file and the check is not skipped anymore.
- Because of the Proxy, every password is searched in the list only once, and the next checks are instant.
- Because of the Facade, `Program.cs` uses one class and one method instead of the whole checking subsystem, and the duplicated code was removed.

The 3 patterns work together as a chain, and none of the old classes had to change. The Adapter and the Proxy were easy to add because `BreachedPasswordCheck` depends on the interface `IBreachedPasswordSource` (DIP from Lab 0). I understood that structural patterns are mostly about wrapping objects: the Adapter changes the interface, the Proxy keeps the interface but controls the access, and the Facade makes a new, simpler interface for many objects.
