using OHEXML.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OHEXML.Repository.Base
{
    /// <summary>
    /// 仓储基类中定义的公共的方法
    /// where T: BaseEntity 实体T来自BaseEntity(或者BaseEntity的子集)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T: BaseEntity
    {
        #region 新增

        /// <summary>
        /// 异步新增实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task AddEntityAsync(T entity);

        /// <summary>
        /// 异步批量新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task AddEntitiesAsync(IEnumerable<T> entities);

        #endregion 

        #region 删除

        /// <summary>
        /// 异步删除实体
        /// </summary>
        /// <param name="where">实体</param>
        /// <returns></returns>
        void DeleteEntity(T entity);

        void DeleteEntities(IEnumerable<T> entities);

        #endregion 删除

        #region 修改

        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void UpdateEntity(T entity);

        /// <summary>
        /// 异步批量修改
        /// </summary>
        /// <param name="where"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        void UpdateEntities(IEnumerable<T> entities);

        #endregion 修改

        #region 查询

        /// <summary>
        /// 异步不跟踪查询所有
        /// </summary>
        /// <returns></returns>
        Task<List<T>> GetAllAsync();       
        /// <summary>
        /// 根据条件异步跟踪查询实体
        /// </summary>
        /// <param name="where">lamda表达式</param>
        /// <returns></returns>
        Task<T> GetEntityAsync(Expression<Func<T, bool>> where);
        /// <summary>
        /// 根据条件异步不跟踪查询实体
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<T> GetEntityAsNoTrackingAsync(Expression<Func<T, bool>> where);
        /// <summary>
        /// 根据条件异步不跟踪查询实体列表
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<List<T>> GetListAsNoTrackingAsync(Expression<Func<T, bool>> where);
        /// <summary>
        ///  根据条件异步跟踪查询实体列表
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<List<T>> GetListEntityAsync(Expression<Func<T, bool>> where);

        #endregion 查询

        #region 操作
        Task BeginTransactionAsync();
        bool CommitTransaction();
        void RollbackTransaction();

        /// <summary>
        /// 当前所有更改异步保存到数据库
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> SaveChangeAsync(CancellationToken cancellationToken = default);
        #endregion

    }
}
