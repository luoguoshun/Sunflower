using OHEXML.Common.LogHelper;
using OHEXML.Contracts.UserModule;
using OHEXML.Entity.Entities;
using OHEXML.Repository.LogModule;
using OHEXML.Repository.UserModule;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OHEXML.Server.UserModule
{
    public class UserServer : IUserServer
    {
        #region 构造函数
        public readonly IUserRepository _userRepository;
        public readonly IAdminRepository _adminRepository;
        public readonly ILogRepository _logRepository;
        public UserServer(IUserRepository userRepository, IAdminRepository adminRepository, ILogRepository logRepository)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _logRepository = logRepository;
        }
        #endregion

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        public async Task<List<UserInfo>> GetListUserInfoAsync()
        {
           return await _userRepository.GetAllAsync();
          
        }
        /// <summary>
        /// 添加实体集合
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        public async Task<(bool, string)> AddListUserInfoAsync(List<UserDTO> userDTO)
        {
            foreach (var item in userDTO)
            {
                UserInfo data = await _userRepository.GetEntityAsync(x => x.UserID == item.UserID);
                if (!data.Equals(null))
                    continue;
                else
                {
                    UserInfo info = new UserInfo
                    {
                        UserID = item.UserID,
                        Name = item.Name,
                        PassWord = item.Password
                    };
                    await _userRepository.AddEntityAsync(info);
                }
            }
            return (await _userRepository.SaveChangeAsync(), "添加成功");
        }
        /// <summary>
        /// 添加用户实体
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        public async Task<(bool, string)> AddUserInfoAsync(UserDTO userDTO)
        {
            UserInfo data = await _userRepository.GetEntityAsync(x => x.UserID == userDTO.UserID);
            if (data != null)
            {
                return (false, "用户已存在!");
            }
            else if (userDTO.Password != userDTO.RePassword)
            {
                return (false, "两次密码不一致!");
            }
            else
            {
                UserInfo info = new UserInfo
                {
                    UserID = userDTO.UserID,
                    Name = userDTO.Name,
                    PassWord = userDTO.Password
                };
                await _userRepository.AddEntityAsync(info);
                if (await _userRepository.SaveChangeAsync())
                    return (true, "注册成功!");
                else
                {
                    Log4NetHelper.LogErr($"用户注册失败{DateTime.Now:G}");
                    Console.WriteLine($"用户注册失败{DateTime.Now:G}");
                    await _logRepository.AddEntityAsync(
                        new LogInfo()
                        {
                            ErroeMessage = "用户注册失败",
                            ErrorTime = DateTime.Now
                        });
                    await _userRepository.SaveChangeAsync();
                    return (false, "注册失败，请联系管理员");
                }

            }
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public async Task<(bool, string)> RemoveUserInfoAsync(string UserID)
        {
            UserInfo data = await _userRepository.GetEntityAsync(x => x.UserID == UserID);

            if (data != null)
            {
                _userRepository.DeleteEntity(data);
                return await _userRepository.SaveChangeAsync() ? (true, "删除成功!") : (false, "删除失败!");
            }
            return (false, "用户不存在");
        }
        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public async Task<(bool, string)> UpdateUserInfoAsync(UserDTO userDTO)
        {
            UserInfo data = await _userRepository.GetEntityAsync(x => x.UserID == userDTO.UserID);

            if (data == null)
            {
                return (false, "用户不存在");
            }
            else
            {
                data.UserID = userDTO.UserID;
                data.Name = userDTO.Name;
                _userRepository.UpdateEntity(data);
                return await _userRepository.SaveChangeAsync() ? (true, "修改成功!") : (false, "修改失败!");
            }
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="OldPassword"></param>
        /// <param name="NewPassword"></param>
        /// <param name="RePassword"></param>
        /// <returns></returns>
        public async Task<(bool, string)> UserResetPassWord(string UserId, string OldPassword, string NewPassword, string RePassword)
        {
            Task<UserInfo> userinfo = _userRepository.GetEntityAsync(x => x.UserID == UserId);
            if (userinfo.Result == null)
            {
                Log4NetHelper.LogErr($"用户查找失败{DateTime.Now:G}");
                Console.WriteLine($"用户查找失败{DateTime.Now:G}");
                await _logRepository.AddEntityAsync(
                    new LogInfo()
                    {
                        ErroeMessage = "用户查找失败",
                        ErrorTime = DateTime.Now
                    });
                await _userRepository.SaveChangeAsync();
                return (false, "出错了");
            }
            if (userinfo.Result.PassWord != OldPassword)
            {
                return (false, "旧密码错误");
            }
            else if (NewPassword != RePassword)
            {
                return (false, "两次密码不一致");
            }
            else
            {
                userinfo.Result.PassWord = NewPassword;

                return await _userRepository.SaveChangeAsync() ? (true, "修改成功！") : (false, "修改失败!");
            }
        }
       
    }
}
