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
    public class AdminRepository : Repository<AdminInfo>, IAdminRepository
    {
        public AdminRepository(OHEsystemContext _dbContext) : base(_dbContext)
        {
            _dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
        }
        /// <summary>
        /// 获取管理员信息
        /// </summary>
        /// <param name="account"></param>
        /// <param name="PassWord"></param>
        /// <returns></returns>
        public async Task<AdminDTO> GetAdminInfoAsync(string account, string PassWord)
        {
            AdminInfo Info = await _dbContext.Set<AdminInfo>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.AdminNo == account && x.PassWord == PassWord);/*加载单条数据*/

            List<AdminInfo> info1 = await _dbContext.Set<AdminInfo>()
                .FromSqlRaw("SELECT * FROM AdminInfo where AdminNo=@account And PassWord=@PassWord"
                , new SqlParameter("account", account)
                , new SqlParameter("PassWord", PassWord))
                .AsNoTracking().ToListAsync(); /*使用FromSqlRaw进行原生SQL语句查询*/

            AdminDTO result = new AdminDTO
            {
                AdminNo = Info.AdminNo,
                PassWord = Info.PassWord,
            };
            return result;
        }
    }
}
