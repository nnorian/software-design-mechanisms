// Lab 1: creational patterns
//   Abstract Factory -> Crypto/     
//   Builder          -> Generation/
//   Prototype        -> Entries/

string rockYouPath = Path.Combine(AppContext.BaseDirectory, "rockyou.txt");

var vaultFile = new VaultFile("vault.json");
var presets = new PasswordPresets();
var charsBuilder = new RandomCharsBuilder();
var phraseBuilder = new PassphraseBuilder();
var templates = EntryTemplateRegistry.CreateDefault();

List<VaultEntry>? entries = null;  
string masterPassword = "";
ICryptoSuiteFactory? suite = null;

while (true){
    Console.WriteLine();
    Console.WriteLine("1. Check a password (Lab 0)");
    Console.WriteLine("2. Generate a password");
    Console.WriteLine("3. Create a vault");
    Console.WriteLine("4. Open the vault");
    Console.WriteLine("5. Add an entry from a template");
    Console.WriteLine("6. Show entries");
    Console.WriteLine("0. Exit");
    Console.Write("> ");
    string choice = Console.ReadLine() ?? "0";

    if (choice == "0") break;
    else if (choice == "1") CheckPassword();
    else if (choice == "2") GeneratePassword();
    else if (choice == "3") CreateVault();
    else if (choice == "4") OpenVault();
    else if (choice == "5") AddEntry();
    else if (choice == "6") ShowEntries();
}

void CheckPassword(){
    Credentials credentials = new LoginReader().Read();
    bool isGood = true;
    foreach (var result in Validate(credentials)){
        if (!result.Passed){
            Console.WriteLine(result.Message);
            isGood = false;
        }
    }
    Console.WriteLine(isGood ? "Good password" : "Bad password");
}

// Builder: the same recipe (Director) can be used with both builders
void GeneratePassword(){
    Console.WriteLine("1. Strong password");
    Console.WriteLine("2. Strong passphrase");
    Console.WriteLine("3. Memorable passphrase");
    Console.WriteLine("4. PIN");
    Console.Write("> ");
    string choice = Console.ReadLine() ?? "";

    string password;
    if (choice == "1") password = presets.Strong(charsBuilder);
    else if (choice == "2") password = presets.Strong(phraseBuilder);
    else if (choice == "3") password = presets.Memorable(phraseBuilder);
    else if (choice == "4") password = presets.Pin(charsBuilder);
    else return;

    Console.WriteLine("Generated: " + password);
}

// Abstract Factory: the user chooses the family of algorithms
void CreateVault(){
    Console.WriteLine("1. Standard (PBKDF2-SHA256 + AES-GCM)");
    Console.WriteLine("2. Hardened (PBKDF2-SHA512 + ChaCha20-Poly1305)");
    Console.Write("> ");
    suite = Console.ReadLine() == "2" ? new HardenedSuiteFactory() : new StandardSuiteFactory();

    Console.Write("New master password: ");
    masterPassword = Console.ReadLine() ?? "";

    // the master password must pass the Lab 0 rules
    var failed = Validate(new Credentials("", masterPassword)).Where(r => !r.Passed).ToList();
    if (failed.Count > 0){
        foreach (var result in failed){
            Console.WriteLine(result.Message);
        }
        Console.WriteLine("Choose a stronger master password");
        return;
    }

    entries = new List<VaultEntry>();
    vaultFile.Save(entries, masterPassword, suite);
    Console.WriteLine("Vault created");
}

void OpenVault(){
    if (!vaultFile.Exists()){
        Console.WriteLine("Create a vault first");
        return;
    }
    Console.Write("Master password: ");
    string password = Console.ReadLine() ?? "";
    try{
        entries = vaultFile.Open(password, out ICryptoSuiteFactory usedSuite);
        suite = usedSuite;
        masterPassword = password;
        Console.WriteLine("Vault opened (" + suite.SuiteId + " suite, " + entries.Count + " entries)");
    }
    catch (CryptographicException){
        Console.WriteLine("Wrong master password");
    }
}

// Prototype: a new entry is a copy of a template
void AddEntry(){
    if (entries == null){
        Console.WriteLine("Open the vault first");
        return;
    }
    List<string> names = templates.Names();
    for (int i = 0; i < names.Count; i++){
        Console.WriteLine((i + 1) + ". " + names[i]);
    }
    Console.Write("> ");
    if (!int.TryParse(Console.ReadLine(), out int number) || number < 1 || number > names.Count){
        return;
    }

    VaultEntry entry = templates.Create(names[number - 1]);

    if (entry is LoginEntry login){
        Console.Write("Username: ");
        login.Username = Console.ReadLine() ?? "";
        login.Password = presets.Strong(charsBuilder); // Builder and Prototype work together
        Console.WriteLine("Generated password: " + login.Password);
    }
    else if (entry is NoteEntry note){
        Console.Write("Note text: ");
        note.Content = Console.ReadLine() ?? "";
    }

    entries.Add(entry);
    vaultFile.Save(entries, masterPassword, suite!);
    Console.WriteLine("Saved: " + entry);
}

void ShowEntries(){
    if (entries == null){
        Console.WriteLine("Open the vault first");
        return;
    }
    foreach (var entry in entries){
        Console.WriteLine(entry);
    }
}

List<CheckResult> Validate(Credentials credentials){
    var checks = new List<IPasswordCheck>();
    checks.Add(new MinLengthCheck(12));
    checks.Add(new CharactersCheck());
    checks.Add(new UsernameDuplicateCheck());
    if (File.Exists(rockYouPath)){
        checks.Add(new BreachedPasswordCheck(new FileBreachedPasswordSource(rockYouPath)));
    }
    return new PasswordValidator(checks).Validate(credentials);
}