using OHEXML.Contracts.UserModule;
using OHEXML.Entity.Entities;
using OHEXML.Repository.Base;
using System.Threading.Tasks;

namespace OHEXML.Repository.UserModule
{
    public interface IAdminRepository : IRepository<AdminInfo>
    {
        /// <summary>
        /// 获取单个用户
        /// </summary>
        /// <param name="account"></param>
        /// <param name="PassWord"></param>
        /// <returns></returns>
        Task<AdminDTO> GetAdminInfoAsync(string account, string PassWord);
    }
}
