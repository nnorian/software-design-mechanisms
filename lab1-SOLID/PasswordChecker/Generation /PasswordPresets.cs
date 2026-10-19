// director
// the recipies 

public class PasswordPresets{
    public string Strong(IPasswordBuilder builder){
        builder.Reset();
        builder.SetLength(20);
        builder.AddLowercase();
        builder.AddUppercase();
        builder.AddDigits();
        builder.AddSymbols();
        return builder.GetResult();
    }

    public string Memorable(IPasswordBuilder builder){
        builder.Reset();
        builder.SetLength(16);
        builder.AddUppercase();
        builder.AddDigits();
        return builder.GetResult();
    }

    public string Pin(IPasswordBuilder builder){
        builder.Reset();
        builder.SetLength(6);
        builder.AddDigits();
        return builder.GetResult();
    }
}