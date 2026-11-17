// steps to build a password 

public interface IPasswordBuilder{
    void Reset();
    void SetLength(int length);
    void AddLowercase();
    void AddUppercase();
    void AddDigits();
    void AddSymbols();
    string GetResult();
}
