using GLP.Basecode.API.Voting.Constant;

namespace GLP.Basecode.API.Voting.Handler
{
    public class LoginResultResponse //Validation, Login
    {
        public LoginResult Result;
        public string Message = null!;
    }

    public class LoginResultMessageResponse
    {
        public const string SUCCESSFULL = "Login successful.";
        public const string USER_NOT_FOUND = "User not found.";
        public const string INVALID_PASSWORD = "Invalid password.";
    }
}
