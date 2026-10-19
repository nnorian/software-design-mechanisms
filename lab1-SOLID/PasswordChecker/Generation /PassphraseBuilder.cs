// concrete builder 2
// e.g. "Tiger-Orbit-Velvet-Mango7"

public class PassphraseBuilder : IPasswordBuilder{
    private int _wordCount;
    private bool _uppercase;
    private bool _digit;
    private string _separator = "-";

    public void Reset(){
        _wordCount = 4;
        _uppercase = false;
        _digit = false;
        _separator = "-";
    }


    public void SetLength(int length){ _wordCount = Math.Max(3, length / 5); }
    public void AddLowercase(){ }                              
    public void AddUppercase(){ _uppercase = true; }
    public void AddDigits(){ _digit = true; }
    public void AddSymbols(){ _separator = "#"; }

    public string GetResult(){
        var words = new List<string>();
        for (int i = 0; i < _wordCount; i++){
            string word = WordList.Words[RandomNumberGenerator.GetInt32(WordList.Words.Length)];
            if (_uppercase){
                word = char.ToUpper(word[0]) + word[1..];
            }
            words.Add(word);
        }
        if (_digit){
            words[^1] += RandomNumberGenerator.GetInt32(10);
        }
        return string.Join(_separator, words);
    }
}