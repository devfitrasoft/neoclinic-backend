using System;
using System.Security.Cryptography;
using System.Text;

namespace Shared.Common
{
    public class Utilities
    {
        public static Tuple<string, DateTime> DoGenerateHashedOtp(int expiryInMinutes)
        {
            string plainOtp = GenerateOtp();
            string hashedOtp = HashOtp(plainOtp);
            var otpExpiryMinutes = expiryInMinutes;
            var expiresAt = DateTime.UtcNow.AddMinutes(otpExpiryMinutes);

            return Tuple.Create(hashedOtp, expiresAt);
        }

        private static string GenerateOtp()
        {
            var bytes = new byte[6];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes).TrimEnd('=');
        }

        private static string HashOtp(string otp)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(otp);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
