using System;
using System.Security.Cryptography;
using System.Text;

public static class SimpleCrypto
{
  private static readonly byte[] Key = Encoding.UTF8.GetBytes("0123456789abcdef"); // 16byte
  private static readonly byte[] IV = Encoding.UTF8.GetBytes("abcdef0123456789");  // 16byte

  public static string Encrypt(string plainText)
  {
    using var aes = Aes.Create();
    aes.Key = Key;
    aes.IV = IV;
    var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

    var plainBytes = Encoding.UTF8.GetBytes(plainText);
    var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

    return Convert.ToBase64String(encryptedBytes);
  }

  public static string Decrypt(string encryptedText)
  {
    using var aes = Aes.Create();
    aes.Key = Key;
    aes.IV = IV;
    var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

    var encryptedBytes = Convert.FromBase64String(encryptedText);
    var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

    return Encoding.UTF8.GetString(decryptedBytes);
  }
}
