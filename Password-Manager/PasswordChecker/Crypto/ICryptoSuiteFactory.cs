// abstract factory 
// creates matching pair of algorithms

public interface ICryptoSuiteFactory{
    string SuiteId { get; }
    IKeyDerivation CreateKeyDerivation();
    ICipher CreateCipher();
}