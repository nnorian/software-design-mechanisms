// concrete product a
// Password-Based Key Derivation Function 2

public class Pbkdf2KeyDerivation : IKeyDerivation{
    private readonly HashAlgorithmName _hash;
    private readonly int _iterations;

    public Pbkdf2KeyDerivation(HashAlgorithmName hash, int iterations){
        _hash = hash;
        _iterations = iterations;
    }

    public byte[] DeriveKey(string masterPassword, byte[] salt){
        return Rfc2898DeriveBytes.Pbkdf2(masterPassword, salt, _iterations, _hash, 32);
        
    }

}