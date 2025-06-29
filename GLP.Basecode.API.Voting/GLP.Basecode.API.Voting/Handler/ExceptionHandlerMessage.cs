namespace GLP.Basecode.API.Voting.Handler
{
    public class ExceptionHandlerMessage
    {
        public string? GetInnermostExceptionMessage(Exception ex)
        {
            if (ex == null) return null;

            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            return ex.Message;
        }
    }
}
