using GLP.Basecode.API.Voting.Constant;
using GLP.Basecode.API.Voting.Handler;
using System.Linq.Expressions;

namespace GLP.Basecode.API.Voting.Interfaces
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
}
