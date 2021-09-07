using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OHEXML.Repository.Base
{
    /// <summary>
    /// 工作单位
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> SubmitChangeAsync(CancellationToken cancellationToken = default);
    }
}
