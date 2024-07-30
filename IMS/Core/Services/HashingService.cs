using IMS.Models;
using System;
using System.Security.Cryptography;
using System.Text;

namespace IMS.Core.Services
{
    public class HashingService : IHashingService
    {
        public HashedPassword HashPassword(string passwordText)
        {
            byte[] salt = GenerateSalt();
            byte[] hash = HashPasswordWithSalt(passwordText, salt);

            var hashedPassword = new HashedPassword
            {
                salt = Convert.ToBase64String(salt),
                passwordHash = Convert.ToBase64String(hash)
            };
            return hashedPassword;
        }
        public bool VerifyPassword(string enteredPassword, string storedSaltBase64, string storedHashBase64)
        {
            byte[] salt = Convert.FromBase64String(storedSaltBase64);
            byte[] storedHash = Convert.FromBase64String(storedHashBase64);

            byte[] enteredHash = HashPasswordWithSalt(enteredPassword, salt);

            return CompareHashes(storedHash, enteredHash);
        }
        private static byte[] GenerateSalt(int size = 32)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(size);
            return salt;
        }
        private static byte[] HashPasswordWithSalt(string password, byte[] salt)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] passwordWithSaltBytes = new byte[passwordBytes.Length + salt.Length];

            Buffer.BlockCopy(passwordBytes, 0, passwordWithSaltBytes, 0, passwordBytes.Length);
            Buffer.BlockCopy(salt, 0, passwordWithSaltBytes, passwordBytes.Length, salt.Length);

            return SHA256.HashData(passwordWithSaltBytes);
        }
        private static bool CompareHashes(byte[] hash1, byte[] hash2)
        {
            if (hash1.Length != hash2.Length) 
                return false;

            for (int i = 0; i < hash1.Length; i++)
            {
                if (hash1[i] != hash2[i])
                    return false;
            }

            return true;
        }

    }
}