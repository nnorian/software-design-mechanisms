# Password Manager

A console password manager written in C# for the Software Design Techniques and Mechanisms labs.

**Student:** Kushnirenko Ecaterina, FAF-243

It can:

- check a password (length, characters, username, leaked passwords from `rockyou`)
- generate passwords and passphrases
- create and open an encrypted vault
- add entries to the vault from templates

## Requirements

- .NET 8 SDK (or newer)

## Build and run

```bash
cd Password-Manager/PasswordChecker
dotnet run
```

`rockyou.zip` is copied next to the program at build time and is read directly, so nothing has to be unzipped.
If `rockyou.txt` is unzipped in the same folder, the program uses it instead, because it is a bit faster.

## Run the tests

From the `Password-Manager` folder:

```bash
cd Password-Manager
dotnet test
```

## Project structure

```
Password-Manager/
├── Password-Manager.sln
├── PasswordChecker/                 the program
│   ├── Program.cs                   console menu (client code)
│   ├── PasswordPolicy.cs            Facade
│   ├── PasswordPolicyFactory.cs     builds the default policy
│   ├── PasswordCheck/               Lab 0 checks, Adapter, Proxy
│   ├── Crypto/                      Abstract Factory
│   ├── Generation/                  Builder
│   ├── Entries/                     Prototype
│   └── Storage/                     the encrypted vault file
├── PasswordChecker.Tests/           unit tests (xUnit)
├── RAPORT.md                        Lab 1 — SOLID
├── RAPORT1.MD                       Lab 2 — Creational patterns
└── RAPORT-STRUCTURAL.md             Lab 3 — Structural patterns
```
