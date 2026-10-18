// abstract product b
// encripts and decrypts data 

publci interface ICipher{
    byte[] Encrypt(byte[] key, byte[] data);
    byte[] Decrypt(byte[] key, byte[] encrypted);

}
