// makes a small zip with one text file inside, like rockyou.zip
// the file is deleted after the test

using System.IO.Compression;

public class TempZip : IDisposable{
    public string Path { get; }

    public TempZip(params string[] lines){
        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid() + ".zip");

        using ZipArchive zip = ZipFile.Open(Path, ZipArchiveMode.Create);
        using var writer = new StreamWriter(zip.CreateEntry("rockyou.txt").Open());
        foreach (string line in lines){
            writer.WriteLine(line);
        }
    }

    public void Dispose(){
        File.Delete(Path);
    }
}
