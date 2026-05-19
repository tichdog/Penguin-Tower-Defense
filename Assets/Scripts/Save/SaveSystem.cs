using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace SaveSystem
{
    public static class SaveSystem
    {
        public const int SAVE_VERSION = 1;

        private const string AES_SALT = "PTD_AES_Salt_v1";
        private const string HMAC_SALT = "PTD_HMAC_Salt_v1";

        private const int KEY_SIZE_BYTES = 32;
        private const int IV_SIZE_BYTES = 16;
        private const int HMAC_SIZE = 32;

        public static byte[] Serialize(SaveData data, string secret)
        {
            if (data == null)
                data = new SaveData();

            data.Normalize();
            data.saveVersion = SAVE_VERSION;

            string json = JsonUtility.ToJson(data, false);
            return SerializeJson(json, secret);
        }

        public static SaveData Deserialize(byte[] raw, string secret)
        {
            string json = DeserializeJson(raw, secret);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            if (data == null)
                throw new SaveCorruptedException("Failed to deserialize save JSON.");

            data.Normalize();

            if (data.saveVersion > SAVE_VERSION)
                Debug.LogWarning($"[SaveSystem] Save version {data.saveVersion} is newer than current {SAVE_VERSION}.");

            return data;
        }

        public static byte[] Serialize<T>(T data, string secret)
        {
            string json = JsonUtility.ToJson(data, false);
            return SerializeJson(json, secret);
        }

        public static T Deserialize<T>(byte[] raw, string secret)
        {
            string json = DeserializeJson(raw, secret);
            return JsonUtility.FromJson<T>(json);
        }

        public static byte[] SerializeJson(string json, string secret)
        {
            if (string.IsNullOrEmpty(secret))
                throw new ArgumentException("Save secret cannot be empty.", nameof(secret));

            byte[] jsonBytes = Encoding.UTF8.GetBytes(json ?? "");

            DeriveKeys(secret, out byte[] aesKey, out byte[] hmacKey);

            byte[] iv = GenerateIV();
            byte[] encrypted = AesEncrypt(jsonBytes, aesKey, iv);
            byte[] hmac = ComputeHmac(encrypted, iv, hmacKey);

            byte[] result = new byte[IV_SIZE_BYTES + HMAC_SIZE + encrypted.Length];
            Buffer.BlockCopy(iv, 0, result, 0, IV_SIZE_BYTES);
            Buffer.BlockCopy(hmac, 0, result, IV_SIZE_BYTES, HMAC_SIZE);
            Buffer.BlockCopy(encrypted, 0, result, IV_SIZE_BYTES + HMAC_SIZE, encrypted.Length);

            return result;
        }

        public static string DeserializeJson(byte[] raw, string secret)
        {
            if (raw == null || raw.Length < IV_SIZE_BYTES + HMAC_SIZE + 1)
                throw new SaveCorruptedException("Save file is too small or empty.");

            if (string.IsNullOrEmpty(secret))
                throw new ArgumentException("Save secret cannot be empty.", nameof(secret));

            byte[] iv = new byte[IV_SIZE_BYTES];
            byte[] storedHmac = new byte[HMAC_SIZE];
            byte[] encrypted = new byte[raw.Length - IV_SIZE_BYTES - HMAC_SIZE];

            Buffer.BlockCopy(raw, 0, iv, 0, IV_SIZE_BYTES);
            Buffer.BlockCopy(raw, IV_SIZE_BYTES, storedHmac, 0, HMAC_SIZE);
            Buffer.BlockCopy(raw, IV_SIZE_BYTES + HMAC_SIZE, encrypted, 0, encrypted.Length);

            DeriveKeys(secret, out byte[] aesKey, out byte[] hmacKey);

            byte[] expectedHmac = ComputeHmac(encrypted, iv, hmacKey);
            if (!CryptographicEquals(storedHmac, expectedHmac))
                throw new SaveCorruptedException("HMAC mismatch. Save file is corrupted or modified.");

            byte[] jsonBytes = AesDecrypt(encrypted, aesKey, iv);
            return Encoding.UTF8.GetString(jsonBytes);
        }

        private static byte[] AesEncrypt(byte[] data, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var ms = new MemoryStream();
            using var encryptor = aes.CreateEncryptor();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }

        private static byte[] AesDecrypt(byte[] data, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var ms = new MemoryStream();
            using var decryptor = aes.CreateDecryptor();
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }

        private static byte[] ComputeHmac(byte[] encrypted, byte[] iv, byte[] hmacKey)
        {
            byte[] combined = new byte[iv.Length + encrypted.Length];
            Buffer.BlockCopy(iv, 0, combined, 0, iv.Length);
            Buffer.BlockCopy(encrypted, 0, combined, iv.Length, encrypted.Length);

            using var hmac = new HMACSHA256(hmacKey);
            return hmac.ComputeHash(combined);
        }

        private static void DeriveKeys(string secret, out byte[] aesKey, out byte[] hmacKey)
        {
            using var aesDerive = new Rfc2898DeriveBytes(
                secret, Encoding.UTF8.GetBytes(AES_SALT), 10000, HashAlgorithmName.SHA256);
            aesKey = aesDerive.GetBytes(KEY_SIZE_BYTES);

            using var hmacDerive = new Rfc2898DeriveBytes(
                secret, Encoding.UTF8.GetBytes(HMAC_SALT), 10000, HashAlgorithmName.SHA256);
            hmacKey = hmacDerive.GetBytes(KEY_SIZE_BYTES);
        }

        private static byte[] GenerateIV()
        {
            byte[] iv = new byte[IV_SIZE_BYTES];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(iv);
            return iv;
        }

        private static bool CryptographicEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];

            return diff == 0;
        }
    }

    public class SaveCorruptedException : Exception
    {
        public SaveCorruptedException(string message) : base(message) { }
    }
}
