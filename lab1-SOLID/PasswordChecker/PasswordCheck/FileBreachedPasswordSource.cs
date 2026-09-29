// check if the introduced password is found in the rockyou wordlist

public class FileBreachedPasswordSource: IBreachedPasswordSource{
    private readonly string _path;

    public FileBreachedPasswordSource(string path){
        _path = path;
    }

    public bool IsBreached(string password){
        foreach (var line in File.ReadLines(_path)){
            if (line == password){
                return true;
            }
        }
        return false;
    }
}