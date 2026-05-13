using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class SaveManager
{
    private static readonly string path = Path.Combine(Application.persistentDataPath, "save.dat");

    // ДОЛЖНЫ БЫТЬ 16/24/32 байта
    private static readonly string aesKey = "12345678901234567890123456789012";

    private static readonly string aesIV = "1234567890123456";

    // SAVE
    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data);

        string encrypted = Encrypt(json);

        string hash = GenerateHash(encrypted);

        SaveFile saveFile = new SaveFile
        {
            encryptedData = encrypted,
            hash = hash
        };

        string finalJson = JsonUtility.ToJson(saveFile, true);

        File.WriteAllText(path, finalJson);

        Debug.Log("Saved: " + path);
    }

    // LOAD
    public static SaveData Load()
    {
        if (!File.Exists(path))
        {
            Debug.Log("Save file not found");

            return null;
        }

        string finalJson = File.ReadAllText(path);

        SaveFile saveFile =
            JsonUtility.FromJson<SaveFile>(finalJson);

        string currentHash =
            GenerateHash(saveFile.encryptedData);

        // Проверка целостности
        if (currentHash != saveFile.hash)
        {
            Debug.LogError("Save file was modified!");

            return null;
        }

        string decrypted =
            EncryptDecrypt(saveFile.encryptedData, false);

        SaveData data =
            JsonUtility.FromJson<SaveData>(decrypted);

        return data;
    }

    // HASH
    private static string GenerateHash(string input)
    {
        using SHA256 sha = SHA256.Create();

        byte[] bytes =
            sha.ComputeHash(Encoding.UTF8.GetBytes(input));

        StringBuilder builder = new();

        foreach (byte b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }

    // ENCRYPT
    private static string Encrypt(string plainText)
    {
        return EncryptDecrypt(plainText, true);
    }

    private static string EncryptDecrypt(string text, bool encrypt)
    {
        using Aes aes = Aes.Create();

        aes.Key = Encoding.UTF8.GetBytes(aesKey);
        aes.IV = Encoding.UTF8.GetBytes(aesIV);

        ICryptoTransform transform =
            encrypt
            ? aes.CreateEncryptor()
            : aes.CreateDecryptor();

        byte[] inputBytes =
            encrypt
            ? Encoding.UTF8.GetBytes(text)
            : Convert.FromBase64String(text);

        byte[] resultBytes =
            transform.TransformFinalBlock(
                inputBytes,
                0,
                inputBytes.Length);

        return encrypt
            ? Convert.ToBase64String(resultBytes)
            : Encoding.UTF8.GetString(resultBytes);
    }
}
