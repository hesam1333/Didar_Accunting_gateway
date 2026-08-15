namespace Gateway.Application.Services.Abstractions;

public interface ICredentialEncryptionService
{
    string Encrypt(string plaintext);

    string Decrypt(string encryptedValue);
}
