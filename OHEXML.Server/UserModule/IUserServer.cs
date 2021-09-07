using OHEXML.Contracts.UserModule;
using OHEXML.Entity.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OHEXML.Server.UserModule
{
    public interface IUserServer
    {
        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        Task<List<UserInfo>> GetListUserInfoAsync();
        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="clientNo"></param>
        /// <param name="token"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task<(bool, string)> AddUserInfoAsync(UserDTO userDTO);
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="OldPassword"></param>
        /// <param name="NewPassword"></param>
        /// <param name="RePassword"></param>
        /// <returns></returns>
        Task<(bool, string)> UserResetPassWord(string UserId,string OldPassword,string NewPassword,string RePassword);
        /// <summary>
        /// 新增用户列表
        /// </summary>
        /// <param name="clientNo"></param>
        /// <param name="token"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task<(bool, string)> AddListUserInfoAsync(List<UserDTO> userDTO);
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        Task<(bool, string)> RemoveUserInfoAsync(string UserID);
        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        Task<(bool, string)> UpdateUserInfoAsync(UserDTO userDTO);

    }
}
