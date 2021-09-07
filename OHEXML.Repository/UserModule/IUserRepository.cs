using OHEXML.Contracts.UserModule;
using OHEXML.Entity.Entities;
using OHEXML.Repository.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OHEXML.Repository.UserModule
{
    public interface IUserRepository : IRepository<UserInfo>
    {
        /// <summary>
        /// 获取单个用户
        /// </summary>
        /// <param name="account"></param>
        /// <param name="PassWord"></param>
        /// <returns></returns>
        Task<UserDTO> GetUserAsync(string account,string PassWord);

        /// <summary>
        /// 查询用户列表
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<UserDTO>> QueryUsersAsync(string account, int page, int rows);
    }
}
