// concrete factory 1
// PBKDF2-SHA256 + AES-GCM

public class StandardSuiteFactory : ICryptoSuiteFactory{
    public string SuiteId => "standard";

    public IKeyDerivation CreateKeyDerivation(){
        return new Pbkdf2KeyDerivation(HashAlgorithmName.SHA256, 600_000);
    }

    public ICipher CreateCipher(){
        return new AesGcmCipher();
    }
}
