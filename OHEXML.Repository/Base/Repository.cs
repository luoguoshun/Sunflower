using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using OHEXML.Entity.Base;
using OHEXML.Entity.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace OHEXML.Repository.Base
{
    /// <summary>
    /// 基本操作数据库类
    /// </summary>
    /// <typeparam name="TEntity">继承BaseEntity的实体类</typeparam>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        /// <summary>
        /// EF上下文
        /// </summary>
        protected OHEsystemContext _dbContext;
        /// <summary>
        /// 事务
        /// </summary>
        protected IDbContextTransaction _Transaction { get; set; }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected string ConnectionString
        {
            get
            {
                var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
                return builder.Build().GetConnectionString("SQLConnection1");
            }
        }
        public Repository(OHEsystemContext dbContext, IDbContextTransaction transaction)
        {
            _dbContext = dbContext;
            _Transaction = transaction;
        }

        public Repository(OHEsystemContext dbContext)
        {
           _dbContext = dbContext;
        }

        #region 添加
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task AddEntityAsync(TEntity entity)
        {
            await _dbContext.AddAsync(entity);
        }

        /// <summary>
        /// 批量添加实体
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task AddEntitiesAsync(IEnumerable<TEntity> entities)
        {
            await _dbContext.AddRangeAsync(entities);
        }
        #endregion

        #region 删除

        public void DeleteEntity(TEntity entity)
        {
            _dbContext.Remove(entity);
        }

        public void DeleteEntities(IEnumerable<TEntity> entities)
        {
            _dbContext.RemoveRange(entities);
        }

        #endregion 

        #region 修改

        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void UpdateEntity(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// 批量修改实体
        /// </summary>
        /// <param name="where"></param>
        /// <param name="entity"></param>
        public virtual void UpdateEntities(IEnumerable<TEntity> entities)
        {
            _dbContext.UpdateRange(entities);
        }

        #endregion 

        #region 查询
        /// <summary>
        /// 异步不跟踪查询所有
        /// </summary>
        /// <returns></returns>
        public async Task<List<TEntity>> GetAllAsync()
        {
            return await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();
        }        

        /// <summary>
        /// 根据条件异步跟踪查询实体
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<TEntity> GetEntityAsync(Expression<Func<TEntity, bool>> where)
        {
            return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(where);
        }

        /// <summary>
        /// 根据条件异步不跟踪查询实体
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<TEntity> GetEntityAsNoTrackingAsync(Expression<Func<TEntity, bool>> where)
        {
            return await _dbContext.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(where);
        }
        /// <summary>
        /// 根据条件异步跟踪查询实体列表
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<List<TEntity>> GetListEntityAsync(Expression<Func<TEntity, bool>> where)
        {
            return await _dbContext.Set<TEntity>().Where(where).ToListAsync();
        }
        /// <summary>
        /// 根据条件异步不跟踪查询实体列表
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<List<TEntity>> GetListAsNoTrackingAsync(Expression<Func<TEntity, bool>> where)
        {
            return await _dbContext.Set<TEntity>().AsNoTracking().Where(where).ToListAsync();
        }
        #endregion

        #region 操作
        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        public async Task BeginTransactionAsync()
        {
            await _dbContext.Database.BeginTransactionAsync();
            if (_Transaction is null)
            {
                _Transaction = await _dbContext.Database.BeginTransactionAsync();
            }
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        /// <returns></returns>
        public bool CommitTransaction()
        {
            if (_Transaction is null)
            {
                return false;
            }
            _Transaction.Commit();
            return true;
        }
        /// <summary>
        /// 事务回滚
        /// </summary>
        public void RollbackTransaction()
        {
            if (_Transaction != null)
            {
                _Transaction.Rollback();
            }
        }
        /// <summary>
        ///当前所有更改异步保存到数据库
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> SaveChangeAsync(CancellationToken cancellationToken = default)
        {           
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        #endregion

    }
}
