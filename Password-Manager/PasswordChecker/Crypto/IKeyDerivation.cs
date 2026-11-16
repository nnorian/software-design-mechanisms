// abstract product a 
// turns master key into encryption key

public interface IKeyDerivation{
    byte[] DeriveKey(string masterPassword, byte[] salt);
    
}