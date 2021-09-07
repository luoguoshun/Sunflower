using SqlSugar;
using System.Collections.Generic;
using static OHEXML.Contracts.HealthReportModule.HealthReportDTO;
namespace OHEXML.Repository.HealthReportModule.Implement
{
    public class DoctorServer : SugalRepository<DoctorProjectOutPut>
    {
        /// <summary>
        /// 获取医生负责项目信息
        /// </summary>
        /// <returns></returns>
        public List<DoctorProjectOutPut> GetDoctorProject()
        {
            List<DoctorProjectOutPut> OutPut = Context.Ado.SqlQuery<DoctorProjectOutPut>(
                @"select a.oper_code as 'DoctorId', Sex = CASE a.sex WHEN '1' THEN '男' WHEN '2' THEN '女' ELSE '未知' END,
                a.oper_name as 'DoctorName',c.comb_code as  'Comb_Code', c.comb_name as 'Comb_Name',c.cls_code as 'ProjectId',
                d.cls_name as 'ProjectName'
                from code_operator a,def_deptover b,code_itemcomb c,code_itemcls d 
                where a.oper_code=b.oper_code and b.comb_code =c.comb_code and c.cls_code=d.cls_code
                and c.prn_flag=@prn_flag and c.inst_flag =@inst_flag",
                 new List<SugarParameter>(){
                      new SugarParameter("@prn_flag","T"),
                      new SugarParameter("@inst_flag","T")
                });
            return OutPut.Count.Equals(0) ? null : OutPut;

        }
    }
}
