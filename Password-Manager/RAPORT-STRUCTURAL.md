# Laboratory Work Nr. 3 — Structural Design Patterns

**Student:** Kushnirenko Ecaterina
**Academic group:** FAF-243

## Introduction

Structural design patterns show how classes and objects can be put together into bigger structures. The structures should still stay simple and easy to change.

There are 7 structural patterns:

- Adapter: makes a class with one interface work where another interface is expected.
- Bridge: splits a big class into two parts, the abstraction and the implementation, so they can change separately.
- Composite: puts objects in a tree, and a group of objects is used the same way as one object.
- Decorator: wraps an object in another object with the same interface to add new behavior.
- Facade: gives one simple interface to a group of classes.
- Flyweight: many objects share the same data, so they use less memory.
- Proxy: an object with the same interface as the real object, that controls the access to it.

In this laboratory I had to implement 3 of these patterns. I chose Adapter, Proxy and Facade.

## The project

The project is the password manager from the previous laboratories. Everything that was there stays the same. The user can still check a password, generate a password, create and open a vault and add entries from templates.

I added the new patterns in the part that checks passwords (Lab 0), because there were some problems there:

1. `rockyou.txt` is 140 MB, so in the repository there is only `rockyou.zip`. The user had to unzip it by hand. If they forgot, the leaked passwords check was just skipped and the program did not say anything.
2. Every check searched the whole list again, even for a password that was already checked before.
3. `Program.cs` created all the checks itself and filtered the failed results in two places (`CheckPassword` and `CreateVault`), so the code was repeated.

Project structure (only the new and changed files):

```
PasswordChecker/
├── Program.cs                              changed, uses the facade
├── PasswordChecker.csproj                  changed, copies rockyou.zip next to the program
├── PasswordPolicy.cs                       Facade
└── PasswordCheck/
    ├── ZipBreachedPasswordSource.cs        Adapter
    └── CachedBreachedPasswordSource.cs     Proxy
```

The other classes did not change.

The new classes work together like a chain:

```
Program.cs
   ▼
PasswordPolicy                    Facade
   ▼
BreachedPasswordCheck             Lab 0, not changed
   ▼
CachedBreachedPasswordSource      Proxy
   ▼
ZipBreachedPasswordSource         Adapter
   ▼
rockyou.zip
```

### How to run

Now nothing has to be unzipped:

```bash
cd PasswordChecker
dotnet run
```

If `rockyou.txt` is already unzipped, the program uses it, because a normal file is a bit faster to read. If not, it reads `rockyou.zip`.

Example:

```
> 1
Username: bob
Password: ABCabc123!@#
password was found in a list of leaked passwords
Bad password
```

## Where the patterns are used

| Pattern | Where                                           | What it does                                                    |
|---------|-------------------------------------------------|-----------------------------------------------------------------|
| Adapter | `PasswordCheck/ZipBreachedPasswordSource.cs`    | reads the passwords directly from `rockyou.zip`                 |
| Proxy   | `PasswordCheck/CachedBreachedPasswordSource.cs` | remembers the answers, so the same password is searched once    |
| Facade  | `PasswordPolicy.cs`                             | one method `Problems(credentials)` instead of all the checks    |

## Adapter

`BreachedPasswordCheck` needs an `IBreachedPasswordSource`:

```csharp
public interface IBreachedPasswordSource{
    bool IsBreached(string password);
}
```

The .NET class `ZipArchive` can read a zip, but it has a totally different interface. It works with entries and streams and does not know anything about passwords. So I made an adapter. It implements `IBreachedPasswordSource` (the target) and inside it uses `ZipArchive` (the adaptee):

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

There is only one file in the zip, so the adapter opens `zip.Entries[0]`. The file is unpacked line by line while it is read, so the 140 MB are not kept in memory and nothing is saved on the disk.

`BreachedPasswordCheck` stays the same. It does not know that the passwords come from a zip now. This works because of DIP from Lab 0: the check depends on the interface, so I can give it any source.

To have the zip next to the program, I added one line in `PasswordChecker.csproj`:

```xml
<None Update="rockyou.zip" CopyToOutputDirectory="PreserveNewest" />
```

## Proxy

Searching 14 million lines takes almost a second. Sometimes the same password is checked more than once, for example the user checks it with option 1 and then uses it as the master password with option 3, or tries the same password again.

`CachedBreachedPasswordSource` is a caching proxy. It has the same interface as the real source, so the check can not see the difference. It keeps the real source inside and calls it only for passwords it did not see before:

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

The proxy works with both sources, `FileBreachedPasswordSource` and `ZipBreachedPasswordSource`. I tested it: checking the same password 4 times takes the same time as checking it once. A new password is still searched one time, the proxy only helps when a password comes again.

The Proxy and the Decorator look the same, both wrap an object with the same interface. The difference is what they are for. A Decorator adds something new to the result. This class does not change the result at all, it only decides when the real object is called. That is why it is a Proxy.

## Facade

Before, `Program.cs` had to know all the Lab 0 classes: the 4 checks, `PasswordValidator`, `FileBreachedPasswordSource` and `CheckResult`. It also filtered the failed results in two places.

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

The facade is also the place where the Adapter and the Proxy are connected. It is created only once in `Program.cs`, so the cache of the proxy is kept while the program runs. Before, the checks were created again at every call, so a cache would not even be possible there.

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

I removed the local function `Validate` and the variable `rockYouPath` from `Program.cs`, so it became shorter. The classes behind the facade did not change and can still be used directly if someone needs the full list of `CheckResult`.

## Conclusion

In this laboratory I added 3 structural patterns to the password manager: Adapter, Proxy and Facade.

- Because of Adapter, the program reads the leaked passwords directly from `rockyou.zip`, so the user does not have to unzip a 140 MB file and the check is not skipped anymore.
- Because of Proxy, a password that was already checked is not searched again.
- Because of Facade, `Program.cs` uses one class and one method instead of all the checking classes, and the repeated code is gone.

The 3 patterns work together as a chain and none of the old classes changed. The Adapter and the Proxy were easy to add because `BreachedPasswordCheck` depends on the interface `IBreachedPasswordSource`, the same DIP from Lab 0.

I understood that structural patterns are mostly about wrapping objects. The Adapter changes the interface, the Proxy keeps the same interface but controls the access, and the Facade makes a new simpler interface for many objects.
