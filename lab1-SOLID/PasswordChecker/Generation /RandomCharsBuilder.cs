// concrete builder 1
// random characters

public class RandomCharsBuilder : IPasswordBuilder{
    private int _length;
    private List<string> _sets = new();

    public void Reset(){
        _length = 12;
        _sets = new List<string>();
    }

    public void SetLength(int length){ _length = length; }
    public void AddLowercase(){ _sets.Add("abcdefghijklmnopqrstuvwxyz"); }
    public void AddUppercase(){ _sets.Add("ABCDEFGHIJKLMNOPQRSTUVWXYZ"); }
    public void AddDigits(){ _sets.Add("0123456789"); }
    public void AddSymbols(){ _sets.Add("!@#$%^&*-_=+?"); }

    public string GetResult(){
        string allChars = string.Concat(_sets);
        var password = new List<char>();

        foreach (string set in _sets){
            password.Add(set[RandomNumberGenerator.GetInt32(set.Length)]);
        }

        while (password.Count < _length){
            password.Add(allChars[RandomNumberGenerator.GetInt32(allChars.Length)]);
        }

        char[] result = password.ToArray();
        RandomNumberGenerator.Shuffle<char>(result);
        return new string(result);
    }
}
