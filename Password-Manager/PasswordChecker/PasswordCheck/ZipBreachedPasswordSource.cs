// adapter
// makes rockyou.zip (a ZipArchive) look like an IBreachedPasswordSource
// so there is no need to unzip the 140 MB file

using System.IO.Compression;

public class ZipBreachedPasswordSource : IBreachedPasswordSource{
    private readonly string _zipPath;

    public ZipBreachedPasswordSource(string zipPath){
        _zipPath = zipPath;
    }

    public bool IsBreached(string password){
        using ZipArchive zip = ZipFile.OpenRead(_zipPath);
        using var reader = new StreamReader(zip.Entries[0].Open());

        while (reader.ReadLine() is string line){
            if (line == password){
                return true;
            }
        }
        return false;
    }
}
