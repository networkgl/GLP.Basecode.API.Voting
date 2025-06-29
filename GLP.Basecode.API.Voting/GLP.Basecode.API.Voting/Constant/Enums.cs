namespace GLP.Basecode.API.Voting.Constant
{
    public enum RoleType
    {
        SuperAdmin = 1,
        Admin = 2,
        Student = 3, //User
    }

    public enum LoginResult
    {
        InvalidPassword,
        UserNotFound,
        Success
    }

    public enum AccountCreationResult
    {
        Success,
        DuplicateIdNumber,
        Error
    }


    public enum ErrorCode
    {
        Error,
        Success,
        NotFound,
        Duplicate,
        GenericError
    }
    
}
