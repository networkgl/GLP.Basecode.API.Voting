using GLP.Basecode.API.Voting.Constant;

namespace GLP.Basecode.API.Voting.Handler
{
    public class OperationResult<T> //for CRUD
    {
        public string? ErrorMessage;
        public string? SuccessMessage;
        public ErrorCode Status;
        public T? Data;
    }

    public class OperationResultMessageResponse
    {
        public const string ADDED = "Data successfully added.";
        public const string UPDATED = "Data successfully updated.";
        public const string DELETED = "Data successfully deleted.";

        public static string NOT_FOUND(string tableName, object id) 
        {
            return $"No {tableName} found for id: {id}";
        }
    }
}
