namespace GLP.Basecode.API.Voting.Services
{
    public class OTPGenerator
    {
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
