using System;

namespace OHEXML.Contracts.HealthReportModule
{
    public class HealthReportDTO
    {
        /// <summary>
        /// 体检公司的体检记录信息
        /// </summary>
        public class CompanyOutPut
        {
            /// <summary>
            /// 公司编号
            /// </summary>
            public string CompanyCode { get; set; }
            /// <summary>
            /// 公司名称
            /// </summary>
            public string CompanyName { get; set; }
            /// <summary>
            /// 地址
            /// </summary>
            public string Address { get; set; }
            /// <summary>
            /// /行业
            /// </summary>
            public string Industry { get; set; }
            /// <summary>
            /// 经济类型
            /// </summary>
            public string EconomicType { get; set; }
            /// <summary>
            /// 公司总人数
            /// </summary>
            public int CountPeople { get; set; }
            /// <summary>
            ///  公司联系电话
            /// </summary>
            public string Telephone { get; set; }
            /// <summary>
            /// 检查时间
            /// </summary>
            public DateTime? CheckDate { get; set; }
            /// <summary>
            /// 复检时间
            /// </summary>
            public DateTime? ReviewerCheckDate { get; set; }
            /// <summary>
            /// 报告时间
            /// </summary>
            public DateTime? ReportDate { get; set; }

            /// <summary>
            /// 总体检人数
            /// </summary>
            public int CountNumber { get; set; }
            /// <summary>
            /// 危害因素
            /// </summary>
            public string HazardousName { get; set; }
            /// <summary>
            /// 职工总数
            /// </summary>
            public int SumNumber { get; set; }
            /// <summary>
            /// 女职工人数
            /// </summary>
            public int WomenWrkers { get; set; }
            /// <summary>
            /// 生产工人总数
            /// </summary>
            public int ProWorkers { get; set; }
            /// <summary>
            /// 女生产工人数
            /// </summary>
            public int WomenProWorkers { get; set; }
            /// <summary>
            /// 有害作业总人数
            /// </summary>
            public int HarmfulWork { get; set; }
            /// <summary>
            /// 有毒作业女生产人数
            /// </summary>
            public int HarmfulWomenProWorkers { get; set; }
            /// <summary>
            /// 负责人
            /// </summary>
            public string UserName { get; set; }
        }

        /// <summary>
        /// 获取医生负责的项目以及资质号
        /// </summary>
        public class DoctorProjectOutPut
        {
            /// <summary>
            /// 医生编号
            /// </summary>
            public string DoctorId { get; set; }
            /// <summary>
            /// 姓名
            /// </summary>
            public string DoctorName { get; set; }
            /// <summary>
            /// 性别
            /// </summary>
            public string Sex { get; set; }
            /// <summary>
            /// 组合代码
            /// </summary>
            public string Comb_Code { get; set; }
            /// <summary>
            /// 组合名称
            /// </summary>
            public string Comb_Name { get; set; }
            /// <summary>
            /// 项目编号
            /// </summary>
            public string ProjectId { get; set; }
            /// <summary>
            /// 负责项目名称
            /// </summary>
            public string ProjectName { get; set; }
            /// <summary>
            /// 医生资质号
            /// </summary>
            public string Qualification { get; set; }

        }

        /// <summary>
        /// 获取项目明细
        /// </summary>
        public class ProjectOutPut
        {
            /// <summary>
            /// 体检项目
            /// </summary>
            public string ProjectName { get; set; }
            /// <summary>
            /// 体检项目编号
            /// </summary>
            public string ProjectId { get; set; }
            /// <summary>
            /// 体检项目明细
            /// </summary>
            public string ProjectDetailsName { get; set; }
            /// <summary>
            /// 体检项目明细编号
            /// </summary>
            public string ProjectDetailsId { get; set; }
        }

        /// <summary>
        /// 用户的体检信息(体检类别、体检套餐)
        /// </summary>
        public class UserCheckInfoOutPut
        {

            /// <summary>
            /// 用户名字
            /// </summary>
            public string UserName { get; set; }
            /// <summary>
            /// 性别
            /// </summary>
            public string Sex { get; set; }
            /// <summary>
            /// 年龄
            /// </summary>
            public int Age { get; set; }
            /// <summary>
            /// 体检日期
            /// </summary>
            public string CheckDate { get; set; }
            /// <summary>
            /// 检查类别
            /// </summary>
            public string TypeName { get; set; }

            /// <summary>
            /// 体检套餐
            /// </summary>
            public string ChecksetMealName { get; set; }
            /// <summary>
            /// 危害因素名称
            /// </summary>
            public string HazardousName { get; set; }
            /// <summary>
            /// 危害因素统计
            /// </summary>
            public int Count { get; set; }
        }

        /// <summary>
        /// 用户初检体检结果(结果 结论 危害因素)
        /// </summary>
        public class UserCheckOutPut
        {
            /// <summary>
            /// 体检编号
            /// </summary>
            public string UserCheckId { get; set; }

            /// <summary>
            /// 用户编号
            /// </summary>
            public string UserId { get; set; }

            /// <summary>
            /// 用户名字
            /// </summary>
            public string UserName { get; set; }
            /// <summary>
            /// 性别
            /// </summary>
            public string Sex { get; set; }
            /// <summary>
            /// 年龄
            /// </summary>
            public int Age { get; set; }
            /// <summary>
            /// 个人工作危害因素
            /// </summary>
            public string HazardousName { get; set; }
            /// <summary>
            /// 体检类别（岗前、在岗、岗后）
            /// </summary>
            public string TypeName { get; set; }
            /// <summary>
            /// 套餐名称(2017常规+肝2+B超\放射诊断套餐)
            /// </summary>
            public string ChecksetMealName { get; set; }
            /// <summary>
            /// 复查标识
            /// </summary>
            public string ReviewSign { get; set; }
            /// <summary>
            /// 体检日期(数据库中获取)
            /// </summary>
            public string CheckDate { get; set; }
            /// <summary>
            /// 体检日期  CheckDate转换过来的值 用于判断
            /// </summary>
            public DateTime NumberDate { get; set; }
            /// <summary>
            /// 公司编号
            /// </summary>
            public string CompanyId { get; set; }
            /// <summary>
            /// 是否完成体检
            /// </summary>
            public bool IsCheckFinish { get; set; }
            /// <summary>
            /// 初检结果
            /// </summary>
            public string CheckResult { get; set; }
            /// <summary>
            /// 初检结论
            /// </summary>
            public string Conclusion { get; set; }
            /// <summary>
            /// 初检建议
            /// </summary>
            /// 
            public string Opption { get; set; }
            /// <summary>
            /// 组名编号
            /// </summary>
            public string Comb_Code { get; set; }
            /// <summary>
            /// 项目组合名称
            /// </summary>
            public string Comb_Name { get; set; }
            /// <summary>
            /// 项目组合结果
            /// </summary>
            public string Comb_Result { get; set; }

            /// <summary>
            /// 项目分类编号
            /// </summary>
            public string ProjectId { get; set; }
            /// <summary>
            /// 项目分类名称
            /// </summary>
            public string ProjectName { get; set; }
            /// <summary>
            /// 项目明细编号
            /// </summary>
            public string ProjectDetailsId { get; set; }
            /// <summary>
            /// 项目详细名称
            /// </summary>
            public string ProjectDetailsName { get; set; }
            /// <summary>
            /// 项目结果
            /// </summary>
            public string ProjectResult { get; set; }
            /// <summary>
            /// 异常标识
            /// </summary>
            public string IsStandard { get; set; }


            /// <summary>
            /// 用于统计数量
            /// </summary>
            public int Count { get; set; }

        }

    }

    public class CreatePdfDTO
    {
        public string UnitID { get; set; }
        public DateTime CheckDate { get; set; }
    }
    public class Test
    {
        public string UnitID { get; set; }
        public DateTime CheckDate { get; set; }
    }
}
