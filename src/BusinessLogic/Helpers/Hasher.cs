﻿using System;
using System.Security.Cryptography;
using LegnicaIT.BusinessLogic.Helpers.Interfaces;

namespace LegnicaIT.BusinessLogic.Helpers
{
    public class Hasher : IHasher
    {
        private static int keySize = 192; //256 chars
        private static int saltSize = 96; //128 chars
        private static int iterations = 10000;

        public string CreateHash(string password, string salt)
        {
            if (password.Length > 0 && salt != null)
            {
                var byteArraySalt = Convert.FromBase64String(salt);
                var deriveBytes = new Rfc2898DeriveBytes(password, byteArraySalt, iterations);
                var key = deriveBytes.GetBytes(keySize);
                return Convert.ToBase64String(key);
            }
            return null;
        }

        public string GenerateRandomSalt()
        {
            var rng = RandomNumberGenerator.Create();
            var buffor = new byte[saltSize];
            rng.GetBytes(buffor);
            return Convert.ToBase64String(buffor);
        }
    }
}