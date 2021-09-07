using Dapper;
using Microsoft.EntityFrameworkCore;
using OHEXML.Contracts.UserModule;
using OHEXML.Entity.Context;
using OHEXML.Entity.Entities;
using OHEXML.Repository.Base;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace OHEXML.Repository.UserModule.Implement
{
    public class UserRepository : Repository<UserInfo>, IUserRepository
    {
        public UserRepository(OHEsystemContext _dbContext) : base(_dbContext)
        {
        }
        /// <summary>
        /// 通过账号密码获取用户信息
        /// </summary>
        /// <param name="account"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public async Task<UserDTO> GetUserAsync(string account, string passWord)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                DynamicParameters paramters = new DynamicParameters();
                paramters.Add("@UserID", account);
                paramters.Add("@PassWord", passWord);
                UserDTO result = await connection.QueryFirstAsync<UserDTO>($@"
                                select * from UserInfo 
                                where UserID=@UserID and PassWord=@PassWord ", paramters);
                return result;
            };
        }
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="account"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UserDTO>> QueryUsersAsync(string account, int page, int rows)
        {
            List<UserInfo> userInfos = await _dbContext.Set<UserInfo>()
                                                     .Where(x => x.UserID.Contains("account"))
                                                     .ToListAsync();

            List<UserDTO> result = userInfos.Select(x => new UserDTO
            {
                UserID = x.UserID,
                Name = x.Name,
            }).ToList();

            return result;
        }
  
    }
}
