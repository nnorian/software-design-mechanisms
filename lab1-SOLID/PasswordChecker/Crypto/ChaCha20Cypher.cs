// ConcreteProduct b2 
// same idea as AesGcmCipher, another algorithm

public class ChaCha20Cipher : ICipher{
    public byte[] Encrypt(byte[] key, byte[] data){
        byte[] nonce = RandomNumberGenerator.GetBytes(12);
        byte[] tag = new byte[16];
        byte[] encrypted = new byte[data.Length];

        using var chacha = new ChaCha20Poly1305(key);
        chacha.Encrypt(nonce, data, encrypted, tag);

        return [.. nonce, .. tag, .. encrypted];
    }

    public byte[] Decrypt(byte[] key, byte[] encrypted){
        byte[] nonce = encrypted[..12];
        byte[] tag = encrypted[12..28];
        byte[] cipherText = encrypted[28..];
        byte[] data = new byte[cipherText.Length];

        using var chacha = new ChaCha20Poly1305(key);
        chacha.Decrypt(nonce, cipherText, tag, data);

        return data;
    }
}
