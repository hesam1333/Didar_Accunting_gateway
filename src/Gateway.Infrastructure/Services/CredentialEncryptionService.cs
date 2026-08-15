using System.Security.Cryptography;
using System.Text;
using Gateway.Application.Services.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Gateway.Infrastructure.Services;

public class CredentialEncryptionService : ICredentialEncryptionService
{
    private const int NonceSizeInBytes = 12;
    private const int TagSizeInBytes = 16;

    private readonly byte[] _key;

    public CredentialEncryptionService(IConfiguration configuration)
    {
        var keyValue = configuration["Encryption:Key"]
            ?? throw new InvalidOperationException("Encryption:Key is not configured.");

        _key = Convert.FromBase64String(keyValue);

        if (_key.Length != 32)
        {
            throw new InvalidOperationException("Encryption:Key must be a base64-encoded 256-bit key.");
        }
    }

    public string Encrypt(string plaintext)
    {
        var nonce = RandomNumberGenerator.GetBytes(NonceSizeInBytes);
        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        var ciphertext = new byte[plaintextBytes.Length];
        var tag = new byte[TagSizeInBytes];

        using var aesGcm = new AesGcm(_key, TagSizeInBytes);
        aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);

        return $"v1:{Convert.ToBase64String(nonce)}:{Convert.ToBase64String(ciphertext)}:{Convert.ToBase64String(tag)}";
    }

    public string Decrypt(string encryptedValue)
    {
        var parts = encryptedValue.Split(':');

        if (parts.Length != 4 || parts[0] != "v1")
        {
            throw new InvalidOperationException("Unsupported encrypted value format.");
        }

        var nonce = Convert.FromBase64String(parts[1]);
        var ciphertext = Convert.FromBase64String(parts[2]);
        var tag = Convert.FromBase64String(parts[3]);
        var plaintext = new byte[ciphertext.Length];

        using var aesGcm = new AesGcm(_key, TagSizeInBytes);
        aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);

        return Encoding.UTF8.GetString(plaintext);
    }
}
