string rockYouPath = Path.Combine(AppContext.BaseDirectory, "rockyou.txt");

var checks = new List<IPasswordCheck>();
checks.Add(new MinLengthCheck(12));
checks.Add(new CharactersCheck());
checks.Add(new UsernameDuplicateCheck());
checks.Add(new BreachedPasswordCheck(new FileBreachedPasswordSource(rockYouPath)));

var reader = new LoginReader();
Credentials credentials = reader.Read();

var validator = new PasswordValidator(checks);
List<CheckResult> results = validator.Validate(credentials);

bool isGood = true;
foreach (var result in results){
    if (!result.Passed){
        Console.WriteLine( result.Message);
        isGood = false;
    }
}

if (isGood){
    Console.WriteLine("Good password");
}
else {
    Console.WriteLine("Bad password");
}