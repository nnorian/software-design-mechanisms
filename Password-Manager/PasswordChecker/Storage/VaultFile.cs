// saves and opens the encrypted vault
// It is the Client of the Abstract Factory 
// it only uses the interfaces so it does not know which algorithms it is using

public record VaultFileData(string SuiteId, byte[] Salt, byte[] EncryptedEntries);

public class VaultFile{
    private readonly string _path;

    public VaultFile(string path){
        _path = path;
    }

    public bool Exists(){
        return File.Exists(_path);
    }

    public void Save(List<VaultEntry> entries, string masterPassword, ICryptoSuiteFactory suite){
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] key = suite.CreateKeyDerivation().DeriveKey(masterPassword, salt);

        byte[] json = JsonSerializer.SerializeToUtf8Bytes(entries);
        byte[] encrypted = suite.CreateCipher().Encrypt(key, json);

    
        var data = new VaultFileData(suite.SuiteId, salt, encrypted);
        File.WriteAllText(_path, JsonSerializer.Serialize(data));
    }

    public List<VaultEntry> Open(string masterPassword, out ICryptoSuiteFactory suite){
        var data = JsonSerializer.Deserialize<VaultFileData>(File.ReadAllText(_path))!;

        suite = CryptoSuites.FromId(data.SuiteId); 
        byte[] key = suite.CreateKeyDerivation().DeriveKey(masterPassword, data.Salt);
        byte[] json = suite.CreateCipher().Decrypt(key, data.EncryptedEntries);

        return JsonSerializer.Deserialize<List<VaultEntry>>(json)!;
    }
}