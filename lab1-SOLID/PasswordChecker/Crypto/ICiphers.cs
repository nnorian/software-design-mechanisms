// abstract product b
// encripts and decrypts data 

public interface ICipher{
    byte[] Encrypt(byte[] key, byte[] data);
    byte[] Decrypt(byte[] key, byte[] encrypted);

}
