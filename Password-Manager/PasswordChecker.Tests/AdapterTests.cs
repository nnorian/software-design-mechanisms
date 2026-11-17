// Adapter: ZipBreachedPasswordSource translates IsBreached(password)
// into reading the lines of the file inside a ZipArchive

public class AdapterTests{

    [Fact]
    public void IsBreached_ReturnsTrue_ForPasswordInsideTheZip(){
        using var zip = new TempZip("123456", "qwerty", "iloveyou");
        IBreachedPasswordSource source = new ZipBreachedPasswordSource(zip.Path);

        Assert.True(source.IsBreached("qwerty"));
    }

    [Fact]
    public void IsBreached_ReturnsFalse_ForPasswordNotInTheZip(){
        using var zip = new TempZip("123456", "qwerty", "iloveyou");
        IBreachedPasswordSource source = new ZipBreachedPasswordSource(zip.Path);

        Assert.False(source.IsBreached("Xk9#mP2$vL7!"));
    }

    [Fact]
    public void IsBreached_ComparesWholeLines_NotParts(){
        using var zip = new TempZip("password123");
        IBreachedPasswordSource source = new ZipBreachedPasswordSource(zip.Path);

        Assert.False(source.IsBreached("password"));
        Assert.True(source.IsBreached("password123"));
    }

    [Fact]
    public void Adapter_WorksWithTheLab0Check_WithoutChangingIt(){
        using var zip = new TempZip("qwerty");
        var check = new BreachedPasswordCheck(new ZipBreachedPasswordSource(zip.Path));

        Assert.False(check.Check(new Credentials("bob", "qwerty")).Passed);
        Assert.True(check.Check(new Credentials("bob", "Xk9#mP2$vL7!")).Passed);
    }
}
