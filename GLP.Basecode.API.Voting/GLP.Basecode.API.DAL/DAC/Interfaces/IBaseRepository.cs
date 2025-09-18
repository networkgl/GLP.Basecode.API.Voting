using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GLP.Basecode.API.Model.Enum;

namespace GLP.Basecode.API.DAL.DAC.Interfaces
{
    public interface IBaseRepository<T> 
    {
        public Task<T?> FindAsyncByPredicate(Expression<Func<T, bool>> predicate);
        public Task<OperationResult<T?>> GetAsyncById(object id);
        public Task<List<T>> GetAllAsync();
        public Task<OperationResult<ErrorCode>> CreateAsync(T entity);
        public Task<OperationResult<ErrorCode>> UpdateAsync(object id, T entity);
        public Task<OperationResult<ErrorCode>> DeleteAsync(object id);
    }

    public class OperationResult<T>
    {
        public ErrorCode Status;
        public T? Data;
        public string? ErrorMessage;
        public string? SuccessMessage;
    }

    public class OperationResultMessageResponse
    {
        public const string ADDED = "Data successfully added.";
        public const string UPDATED = "Data successfully updated.";
        public const string DELETED = "Data successfully deleted.";
        public const string DUPLICATE = "Error: Data is already exist.";

        public static string NOT_FOUND(string tableName, object id)
        {
            return $"No {tableName} found for id: {id}";
        }
    }
}
