// concrete product b1
// nonce (12 bytes) + tag (16 bytes) + encrypted data

public class AesGcmCipher : ICipher{
    public byte[] Encrypt(byte[] key, byte[] data){
        byte[] nonce = RandomNumberGenerator.GetBytes(12);
        byte[] tag = new byte[16];
        byte[] encrypted = new byte[data.Length];

        using var aes = new AesGcm(key, 16);
        aes.Encrypt(nonce, data, encrypted, tag);

        return [.. nonce, .. tag, .. encrypted];
    }

    public byte[] Decrypt(byte[] key, byte[] encrypted){
        byte[] nonce = encrypted[..12];
        byte[] tag = encrypted[12..28];
        byte[] cipherText = encrypted[28..];
        byte[] data = new byte[cipherText.Length];

        using var aes = new AesGcm(key, 16);
        aes.Decrypt(nonce, cipherText, tag, data); // throws if the key is wrong

        return data;
    }
}
