string rockYouPath = Path.Combine(AppContext.Basedirectory, "rockyou.txt");

var check = new List<IPasswordCheck>();
check.Add(new MinLengthCheck(12));
check.Add(new CharactersCheck());
check.Add(new UsernameDuplicateCheck());
check.Add(new BreachedPasswordCheck(new FileBreachedPassowrdSource(rockYouPath)));

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