// concrete factory 2
// PBKDF2-SHA512 + ChaCha20-Poly1305

public class HardenedSuiteFactory : ICryptoSuiteFactory {
    public string SuiteId => "hardened";

    public IKeyDerivation CreateKeyDerivation(){
        return new Pbkdf2KeyDerivation(HashAlgorithmName.SHA512, 210000);

    }

    public IChipher CreateCipher(){
        return new ChaCha20Cipher();
    }
}