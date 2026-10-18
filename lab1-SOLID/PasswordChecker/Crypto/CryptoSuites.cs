// finds the factory based on id 
// kinda caches it so it is opened with the same suite it was saved with

public static class CryptoSuites{
    public static ICryptoSuiteFactory FromId(string id){
        if (id == "standard"){
            return new StandardSuiteFactory();
        }

        if (id == "hardened"){
            return new HardenedSuiteFactory();
        }

        throw new NotSupportedException("unknown suite: " + id);
    }

}