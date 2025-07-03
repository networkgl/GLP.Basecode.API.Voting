using System.Security.Cryptography;

namespace GLP.Basecode.API.Voting.Services
{
    public class OTPGenerator
    {

        public static int Generate(int digits = 5)
        {
            if (digits < 1 || digits > 9)
                throw new ArgumentOutOfRangeException(nameof(digits), "Digits must be between 1 and 9.");

            int min = (int)Math.Pow(10, digits - 1);
            int max = (int)Math.Pow(10, digits) - 1;

            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var value = BitConverter.ToUInt32(bytes, 0);

            return (int)(value % (max - min + 1)) + min;
        }

        // Return random number for OTP
        public static int code
        {
            get
            {
                Random r = new Random();
                return r.Next(10000, 99999);
            }
        }
    }
}
