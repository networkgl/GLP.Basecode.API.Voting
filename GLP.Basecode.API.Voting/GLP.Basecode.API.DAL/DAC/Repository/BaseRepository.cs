using GLP.Basecode.API.DAL.DAC.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GLP.Basecode.API.DAL.Data;
using GLP.Basecode.API.Model.Enum;
using GLP.Basecode.API.Helper;
using System.Linq.Expressions;


namespace GLP.Basecode.API.DAL.DAC.Repository
{
    public class BaseRepository<T> : IBaseRepository<T>
        where T : class    
    {
        private DbSet<T> _table;
        private DbContext _db;
        private readonly ExceptionMessageHelper _errMsg;

        public BaseRepository(VotingContext dbContext, ExceptionMessageHelper errMsg)
        {
            _db = dbContext; // use DI-provided instance
            _table = _db.Set<T>();
            _errMsg = errMsg;
        }
        public async Task<List<T>> GetAllAsync()
        {
            var retVal = await _table.ToListAsync();
            return retVal;
        }

        public async Task<OperationResult<T?>> GetAsyncById(object id)
        {
            var opRes = new OperationResult<T?>();

            try
            {
                var entity = await _table.FindAsync(id);

                if (entity is null)
                {
                    opRes.ErrorMessage = OperationResultMessageResponse.NOT_FOUND(typeof(T).Name, id);
                    opRes.Status = ErrorCode.NotFound;
                    opRes.Data = null;

                    return opRes;
                }

                opRes.SuccessMessage = "Data successfully retrieved.";
                opRes.Status = ErrorCode.Success;
                opRes.Data = entity;

                return opRes;
            }
            catch (Exception e)
            {
                opRes.ErrorMessage = _errMsg.GetInnermostExceptionMessage(e);
                opRes.Status = ErrorCode.Error;
                opRes.Data = null;
                return opRes;
            }

        }

        public async Task<OperationResult<ErrorCode>> CreateAsync(T entity)
        {
            var opRes = new OperationResult<ErrorCode>();
            try
            {
                await _table.AddAsync(entity);
                await _db.SaveChangesAsync();

                opRes.SuccessMessage = OperationResultMessageResponse.ADDED;
                opRes.Status = ErrorCode.Success;

                return opRes;
            }
            catch (Exception e)
            {
                opRes.ErrorMessage = _errMsg.GetInnermostExceptionMessage(e);
                opRes.Status = ErrorCode.Error;
                return opRes;
            }
        }

        public async Task<OperationResult<ErrorCode>> UpdateAsync(object id, T entity)
        {
            var opRes = new OperationResult<ErrorCode>();
            try
            {
                var oldEntity = await _table.FindAsync(id);

                if (oldEntity is not null)
                {
                    _db.Entry(oldEntity).CurrentValues.SetValues(entity);
                    await _db.SaveChangesAsync();
                    opRes.SuccessMessage = OperationResultMessageResponse.UPDATED;
                    opRes.Status = ErrorCode.Success;

                    return opRes;
                }

                //Assuming error throws
                opRes.ErrorMessage = OperationResultMessageResponse.NOT_FOUND(typeof(T).Name, id);
                opRes.Status = ErrorCode.NotFound;

                return opRes;
            }
            catch (Exception e)
            {
                opRes.ErrorMessage = _errMsg.GetInnermostExceptionMessage(e);
                opRes.Status = ErrorCode.Error;

                return opRes;
            }
        }

        public async Task<OperationResult<ErrorCode>> DeleteAsync(object id)
        {
            var opRes = new OperationResult<ErrorCode>();
            try
            {
                var entity = await _table.FindAsync(id);
                if (entity is null)
                {
                    opRes.ErrorMessage = $"No {_table.GetType().Name.ToString()} found for id: {id}";
                    opRes.Status = ErrorCode.NotFound;

                    return opRes;
                }

                _table.Remove(entity);
                await _db.SaveChangesAsync();

                opRes.Status = ErrorCode.Success;
                opRes.SuccessMessage = OperationResultMessageResponse.DELETED;

                return opRes;
            }
            catch (Exception e)
            {
                opRes.ErrorMessage = _errMsg.GetInnermostExceptionMessage(e);
                opRes.Status = ErrorCode.Error;

                return opRes;
            }
        }

        public async Task<T?> FindAsyncByPredicate(Expression<Func<T, bool>> predicate)
        {
            return await _table.FirstOrDefaultAsync(predicate);
        }
    }
}
