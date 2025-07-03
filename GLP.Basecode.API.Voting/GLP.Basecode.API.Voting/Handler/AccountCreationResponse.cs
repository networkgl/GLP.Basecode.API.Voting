using GLP.Basecode.API.Voting.Constant;

namespace GLP.Basecode.API.Voting.Handler
{
    public class AccountCreationResponse
    {
        public AccountCreationResult Result;
        public string Message = null!;
    }

    public class AccountCreationMessageResponse
    {
        public const string DUPLICATE_USER = "User already exist.";
        public const string DUPLICATE_USER_EMAIL = "User already exist.";
        public const string STUDENT_CREATED_SUCCESSFULLY = "Student successfully added.";

    }
}
