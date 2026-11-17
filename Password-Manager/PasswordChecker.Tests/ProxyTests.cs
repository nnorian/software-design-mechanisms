// Proxy: CachedBreachedPasswordSource has the same interface as the real source,
// gives the same answers, but calls the real source only once per password

public class ProxyTests{

    [Fact]
    public void SamePasswordManyTimes_RealSourceIsCalledOnce(){
        var real = new CountingSource("qwerty");
        IBreachedPasswordSource proxy = new CachedBreachedPasswordSource(real);

        proxy.IsBreached("qwerty");
        proxy.IsBreached("qwerty");
        proxy.IsBreached("qwerty");

        Assert.Equal(1, real.Calls);
    }

    [Theory]
    [InlineData("qwerty")]
    [InlineData("Xk9#mP2$vL7!")]
    public void Proxy_GivesTheSameAnswerAsTheRealSource(string password){
        var real = new CountingSource("qwerty");
        IBreachedPasswordSource proxy = new CachedBreachedPasswordSource(new CountingSource("qwerty"));

        Assert.Equal(real.IsBreached(password), proxy.IsBreached(password));
        Assert.Equal(real.IsBreached(password), proxy.IsBreached(password)); // second time from the cache
    }

    [Fact]
    public void DifferentPasswords_AreEachSearchedOnce(){
        var real = new CountingSource("qwerty");
        IBreachedPasswordSource proxy = new CachedBreachedPasswordSource(real);

        proxy.IsBreached("qwerty");
        proxy.IsBreached("abc");
        proxy.IsBreached("qwerty");
        proxy.IsBreached("abc");

        Assert.Equal(2, real.Calls);
    }

    [Fact]
    public void Proxy_WritesToTheLog_WhenTheCacheIsUsed(){
        var log = new StringWriter();
        IBreachedPasswordSource proxy = new CachedBreachedPasswordSource(new CountingSource(), log);

        proxy.IsBreached("abc");
        proxy.IsBreached("abc");

        Assert.Contains("taken from the cache", log.ToString());
    }
}
