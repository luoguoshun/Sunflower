using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;
using static OHEXML.Contracts.HealthReportModule.HealthReportDTO;

namespace OHEXML.Repository.HealthReportModule.Implement
{
    public class CheckInfoResultServer : SugalRepository<UserCheckInfoOutPut>
    {
        /// <summary>
        /// 获取用户的体检信息(体检套餐、体检类别.危害因素统计)
        /// </summary>
        /// <returns></returns>
        public async Task<List<UserCheckInfoOutPut>> GetUserCheckInfoAsync(string UnitId, string CheckDate)
        {
            List<SugarParameter> parameters = new List<SugarParameter> {
                      new SugarParameter("@CompanyId",UnitId),
                      new SugarParameter("@CheckDate", CheckDate)
            };
            List<UserCheckInfoOutPut> OutPut = await Context.Ado.SqlQueryAsync<UserCheckInfoOutPut>(
               @"select a.reg_no as 'UserCheckId' ,HazardousName=CASE a.note WHEN '' THEN '不详' ELSE a.note END, b.pat_name as 'UserName', 
                b.age as 'Age',c.cls_type_name as 'TypeName',a.clus_name as 'ChecksetMealName',a.reg_date as 'CheckDate'
                from tj_register a,tj_patient b,tj_cls_type c
                where a.pat_code=b.pat_code and a.tj_cls_type=c.cls_type_code 
                and b.lnc_code=@CompanyId and a.reg_date=@CheckDate
                group by a.reg_no,a.note, b.pat_name,b.sex,b.age,a.clus_name,a.reg_date,c.cls_type_name", parameters);

            return OutPut.Count.Equals(0) ? null : OutPut;

        }
        /// <summary>
        /// 获取所有用户初检结论信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<UserCheckOutPut>> GetAllFirstResultAsync(string UnitId, string CheckDate)
        {
            List<SugarParameter> parameters = new List<SugarParameter> {
                      new SugarParameter("@ReviewSign","F"),
                      new SugarParameter("@CompanyId",UnitId),
                      new SugarParameter("@CheckDate", CheckDate)
            };
            List<UserCheckOutPut> OutPut = await Context.Ado.SqlQueryAsync<UserCheckOutPut>(
               @"select a.reg_no as 'UserCheckId',a.reg_time as 'CheckDate', HazardousName=CASE a.note WHEN '' THEN '不详' ELSE a.note END,a.b_flag as 'ReviewSign', b.lnc_code as 'CompanyId',
                b.pat_code as 'UserId',b.pat_name as 'UserName',c.rec_no, Sex = CASE b.sex WHEN '1' THEN '男' WHEN '2' THEN '女' ELSE '未知' END,
                b.age as 'Age',a.clus_name as 'ChecksetMealName',h.cls_type_name as 'TypeName',d.resultup as 'CheckResult',
                d.sumup as 'Conclusion', d.sugg_tag as 'Opption1`',d.advice as 'Opption',e.comb_code as 'Comb_Code', e.comb_name as 'Comb_Name',c.res_tag as 'comb_Result',
                f.cls_code as 'ProjectId',f.cls_name 'ProjectName',g.item_code as 'ProjectDetailsId',g.item_name as 'ProjectDetailsName',i.rec_result as 'ProjectResult',i.ab_flag as 'IsStandard'
                from tj_register a,tj_patient b,tj_record c,tj_record_entry i,tj_record_con d,code_itemcomb e,code_itemcls f,code_item g,tj_cls_type h
                where a.pat_code=b.pat_code and a.reg_no=c.reg_no and  c.rec_no=i.rec_no and c.reg_no=d.reg_no and a.reg_no=d.reg_no and i.comb_code=e.comb_code
                and g.cls_code=f.cls_code and  a.tj_cls_type=h.cls_type_code and i.item_code=g.item_code 
                and a.b_flag =@ReviewSign and b.lnc_code=@CompanyId and a.reg_date=@CheckDate", parameters);

            return OutPut.Count.Equals(0) ? null : OutPut;

        }
        /// <summary>
        /// 获取用户复检的信息
        /// </summary>
        /// <returns></returns>
        /// 
        public async Task<List<UserCheckOutPut>> GetAllReviewResultAsync(string UnitId, string CheckDate)
        {
            List<SugarParameter> parameters = new List<SugarParameter> {
                      new SugarParameter("@ReviewSign","T"),
                      new SugarParameter("@CompanyId",UnitId),
                      new SugarParameter("@CheckDate",CheckDate)
            };
            List<UserCheckOutPut> OutPut = await Context.Ado.SqlQueryAsync<UserCheckOutPut>(
               @"select a.reg_no as 'UserCheckId',a.reg_time as 'CheckDate',HazardousName=CASE a.note WHEN '' THEN '不详' ELSE a.note END,a.b_flag as 'ReviewSign', b.lnc_code as 'CompanyId',
                b.pat_code as 'UserId',b.pat_name as 'UserName',c.rec_no, Sex = CASE b.sex WHEN '1' THEN '男' WHEN '2' THEN '女' ELSE '未知' END,
                b.age as 'Age',a.clus_name as 'ChecksetMealName',h.cls_type_name as 'TypeName',d.resultup as 'CheckResult',
                d.sumup as 'Conclusion', d.sugg_tag as 'Opption1`',d.advice as 'Opption',e.comb_code as 'Comb_Code', e.comb_name as 'Comb_Name',c.res_tag as 'comb_Result',
                f.cls_code as 'ProjectId',f.cls_name 'ProjectName',g.item_code as 'ProjectDetailsId',g.item_name as 'ProjectDetailsName',i.rec_result as 'ProjectResult',i.ab_flag as 'IsStandard'
                from tj_register a,tj_patient b,tj_record c,tj_record_entry i,tj_record_con d,code_itemcomb e,code_itemcls f,code_item g,tj_cls_type h
                where a.pat_code=b.pat_code and a.reg_no=c.reg_no and  c.rec_no=i.rec_no and c.reg_no=d.reg_no and a.reg_no=d.reg_no and i.comb_code=e.comb_code
                and g.cls_code=f.cls_code and  a.tj_cls_type=h.cls_type_code and i.item_code=g.item_code 
                and a.b_flag =@ReviewSign and b.lnc_code=@CompanyId and a.reg_date=@CheckDate", parameters);

            return OutPut.Count.Equals(0) ? null : OutPut;

        }
    }
}
