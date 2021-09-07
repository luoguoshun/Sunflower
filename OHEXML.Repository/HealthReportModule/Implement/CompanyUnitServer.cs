using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OHEXML.Contracts.HealthReportModule.HealthReportDTO;

namespace OHEXML.Repository.HealthReportModule.Implement
{
    /// <summary>
    /// 体检单位仓储
    /// </summary>
    public class CompanyUnitServer : SugalRepository<CompanyOutPut>
    {
        /// <summary>
        /// 获取所有体检单位信息
        /// </summary>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public async Task<CompanyOutPut> GetCompanyByCompanyIDAsync(string CompanyID)
        {
            List<SugarParameter> parameters = new List<SugarParameter> {
                               new SugarParameter("@CompanyId",CompanyID)
            };
            List<CompanyOutPut> OutPut = await Context.Ado.SqlQueryAsync<CompanyOutPut>(
                @"select a.lnc_code as 'CompanyId',a.lnc_name as 'CompanyName',a.duty_oper as 'UserName',a.tel,a.postcode as 'CompanyCode' ,
                  a.addr as 'Address',a.lnc_class as 'EconomicType',a.fax as 'Fax',a.work_opers as 'CountPeople'
                  from tj_lnc a, tj_patient b
                  where a.lnc_code=b.lnc_code and a.lnc_code=@CompanyId", parameters);

            return OutPut.Count.Equals(0) ? null : OutPut.FirstOrDefault();
        }
    }
}
