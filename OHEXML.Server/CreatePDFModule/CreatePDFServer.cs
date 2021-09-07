using HealthReport;
using HealthReport.CaseDTO;
using HealthReportHelper.HealthReportHelper;
using Microsoft.EntityFrameworkCore;
using OHEXML.Common.OftenString;
using OHEXML.Repository.HealthReportModule;
using OHEXML.Repository.HealthReportModule.Implement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static OHEXML.Contracts.HealthReportModule.HealthReportDTO;

namespace OHEXML.Server.CreatePDFModule
{
    public class CreatePDFServer : ICreatePDFServer
    {
        #region 构造函数
        private readonly CheckInfoResultServer _resultServer;
        private readonly CompanyUnitServer _companyUnitServer;
        private readonly DoctorServer _doctorServer;
        private readonly WordPathRepository _wordPathServer;
        private readonly string basePath;//程序集的相对路径
        private string savePath;//保存地址
        private DateTime SaveTime => DateTime.Now;
        public CreatePDFServer(CompanyUnitServer companyUnitServer, CheckInfoResultServer resultServer, DoctorServer doctorServer, WordPathRepository wordPathServer)
        {
            _companyUnitServer = companyUnitServer;
            _resultServer = resultServer;
            _doctorServer = doctorServer;
            _wordPathServer = wordPathServer;

            basePath = Environment.CurrentDirectory;
        }
        #endregion

        /// <summary>
        /// 生成简洁版报告
        /// </summary>
        /// <param name="UnitID"></param>
        /// <param name="CheckDate"></param>
        /// <returns></returns>
        public async Task<(bool, string, string)> CreatePDFAsync(string UnitID, string CheckDate)
        {
            #region ------【1】获取数据------------            
            var CompanyInfo =await _companyUnitServer.GetCompanyByCompanyIDAsync(UnitID);// 体检公司的详细信息           
            var UserCheckInfo =await _resultServer.GetUserCheckInfoAsync(UnitID, CheckDate);//用户体检信息登记表  
            if (CompanyInfo == null || UserCheckInfo == null)
            {
                //查不到体检数据便返回
                return (false, "暂无体检信息！", string.Empty);
            }
            var StrHazardousName = GetHazardousToCompany(UserCheckInfo); //该公司的危害因素          
            var FirstResult =await _resultServer.GetAllFirstResultAsync(UnitID, CheckDate);//获取用户初检所有项目明细结果            
            var ReviewResult =await _resultServer.GetAllReviewResultAsync(UnitID, CheckDate);//获取用户复检所有项目明细结果           
            var Conclusion = GetUserConclusion(FirstResult);//获取用户初检结论信息          
            var ReviewConclusion = GetUserReviewConclusion(ReviewResult, CheckDate); //获取用户复检结论信息           
            var ResultSummary = GetCheckResultSummary(Conclusion, ReviewConclusion); //合并初检和复检信息
            var HazardRusultCount = HazardousCount(UserCheckInfo, ResultSummary);//危害因素-病例统计

            #endregion

            #region ------【2】调取方法------------
            HealthReportAA healthReportAA = new HealthReportAA(CompanyInfo.CompanyName, SaveTime);
            healthReportAA.AddmainPage(CompanyInfo, CheckDate);           
            healthReportAA.TestAddImageToBody(this.basePath+ $@"\wwwroot\Image\Admin.png");
            healthReportAA.AddExplainPage(CompanyInfo, UserCheckInfo);
            healthReportAA.AddSummary(CompanyInfo, UserCheckInfo, ResultSummary, StrHazardousName);
            healthReportAA.AddHealthCard(CompanyInfo, HazardRusultCount);
            healthReportAA.AddCheckResult1(ResultSummary);
            healthReportAA.AddCheckResult2(ResultSummary);
            healthReportAA.AddCheckResult3(ResultSummary);
            healthReportAA.AddRredentials();
            healthReportAA.AddExplanation1();
            healthReportAA.AddExplanation2();
            healthReportAA.ApplyStyleToParagraph();
            #endregion

            #region ------【3】生成Word并返回-------
            //定义文件名称和保存路径
            var fielName = $"{CompanyInfo.CompanyName}({CheckDate.Replace(".", "")}简洁报告){SaveTime:yyyyMMddHHmmss}.docx";
            var savePath = Path.Combine($@"{basePath}\wwwroot\Templates\HealthReportCaches\", fielName);
            healthReportAA.SaveWord(savePath);
            //判断文件是否存在，返回路经
            if (File.Exists(savePath))
            {
                string Url = $@"\src\Templates\HealthReportCaches\{fielName}";
                _wordPathServer.SaveWord(CompanyInfo.CompanyName, Url, fielName, SaveTime.ToString("yyyyMMddHHmmss"));
                return (true, "生成成功！", Url);
            }
            else
            {
                return (true, "找不到文件!", string.Empty);
            }
            #endregion
        }
        /// <summary>
        /// 生成完整版报告
        /// </summary>
        /// <param name="UnitID"></param>
        /// <param name="CheckDate"></param>
        /// <returns></returns>
        public async Task<(bool, string, string)> CreateCompletePDFAsync(string UnitID, string CheckDate)
        {
            #region--------【1】获取数据----------            
            var CompanyInfo =await _companyUnitServer.GetCompanyByCompanyIDAsync(UnitID);//体检公司的详细信息            
            var UserCheckInfo =await _resultServer.GetUserCheckInfoAsync(UnitID, CheckDate);//用户体检信息登记表
            if (CompanyInfo==null || UserCheckInfo == null)
            {
                //查不到体检数据便返回
                return (false, "暂无体检信息！", string.Empty);
            }
            var StrHazardousName = GetHazardousToCompany(UserCheckInfo); //该公司的危害因素                     
            var FirstResult =await _resultServer.GetAllFirstResultAsync(UnitID, CheckDate);//获取用户初检所有项目明细结果
            var ReviewResult =await _resultServer.GetAllReviewResultAsync(UnitID, CheckDate);//获取用户复检所有项目明细结果            
            var Conclusion = GetUserConclusion(FirstResult);//获取用户初检结论信息
            var ReviewConclusion = GetUserReviewConclusion(ReviewResult, CheckDate);//获取用户复检结论信息            
            var ResultSummary = GetCheckResultSummary(Conclusion, ReviewConclusion);//合并初检和复检信息         
            var resultAnalyse = ResultAnalyse(FirstResult);// 统计用户不合格项目体检结果
            var projectDetailsResultAnalyse = ProjectDetailsResultAnalyse(FirstResult);//统计用户体检项目详细结果
            var getProjectCount = GetProjectCount(FirstResult);//获取各个项目分类体检人数的统计
            var getProjectDetailsCount = GetProjectDetailsCount(FirstResult);//获取各个项目明细体检人数的统计
            var HazardRusultCount = HazardousCount(UserCheckInfo, ResultSummary);//体检人员职业危害因素统计
            var getDoctorProject = _doctorServer.GetDoctorProject();//获取医生负责的项目组合
            var AllCombCodeCount = GetCombCodeCount(FirstResult);//获取此次体检的项目组合
            #endregion

            #region  --------【2】调取方法----------
            HealthReportBB healthReportBB = new HealthReportBB(CompanyInfo.CompanyName, SaveTime);
            healthReportBB.AddMainPage(CompanyInfo, CheckDate);
            healthReportBB.AddExplain(CompanyInfo, UserCheckInfo);
            healthReportBB.AddSummary1(CompanyInfo, UserCheckInfo, ResultSummary, StrHazardousName);
            healthReportBB.AddSummary2(resultAnalyse, getProjectCount, HazardRusultCount);
            healthReportBB.AddHealthCard(CompanyInfo, HazardRusultCount);
            healthReportBB.AddCheckResult1(ResultSummary);
            healthReportBB.AddCheckResult2(ReviewConclusion);
            healthReportBB.AddCheckResult3(projectDetailsResultAnalyse, getProjectDetailsCount);
            healthReportBB.AddCheckResult3_1(projectDetailsResultAnalyse, getProjectDetailsCount);
            healthReportBB.AddCheckResult4(ResultSummary);
            healthReportBB.AddRredentialst();
            healthReportBB.AddQualificationNumber(getDoctorProject, AllCombCodeCount);
            healthReportBB.AddExplanation1();
            healthReportBB.AddExplanation2();
            healthReportBB.AddExplanation3();
            healthReportBB.ApplyStyleToParagraph();
            #endregion

            #region --------【3】生成Word并返回-----
            //定义文件名称和保存路径
            var fielName = $"{CompanyInfo.CompanyName}({CheckDate.Replace(".", "")}总报告){SaveTime:yyyyMMddHHmmss}.docx";
            savePath = Path.Combine($@"{basePath}\wwwroot\Templates2\HealthReportCaches\", fielName);
            healthReportBB.SaveWord(savePath);
            //判断文件是否存在，返回路经
            if (File.Exists(savePath))
            {
                string Url= $@"\src\Templates2\HealthReportCaches\{fielName}";
                _wordPathServer.SaveWord(CompanyInfo.CompanyName, Url, fielName, SaveTime.ToString("yyyyMMddHHmmss"));
                return (true, "生成成功！",Url);
            }
            else
            {
                return (true, "找不到文件!", string.Empty);
            }
            
            #endregion
        }
        /// <summary>
        /// Word转PDF文件
        /// </summary>
        /// <param name="WordPath"></param>
        /// <returns></returns>
        public (bool, string) WordToPDF(string UnitName, string WordPath)
        {
            var fielName = $"({UnitName}{DateTime.Now:yyyyMMddHHmmss}).docx";
            var savePath = Path.Combine($@"{basePath}\wwwroot\Templates\FinishHealthTemplate\", fielName);//保存路径
            string message = WordHelper.WordToPDF(WordPath, savePath);
            return File.Exists(savePath) ? (true, savePath) : (false, message);
        }

         #region 数据分析
        /// <summary>
        /// 所有登记体检信息人员 危害因素--检查结果 统计
        /// </summary>
        /// <param name="UserCheckInfo">用户体检信息</param>
        /// <param name="ResultSummary"></param>
        /// <returns></returns>
        public List<UserHazardousCountOut> HazardousCount(List<UserCheckInfoOutPut> UserCheckInfo, List<CheckResultOut> ResultSummary)
        {
            #region 【1】统计 每个危害因素体检的人数
            //注意:
            //1.foreach不可以改变变量，即使用集合存变量也不可以
            //2.foreach可以改变对象的值，但不能删除或添加对象
            foreach (var item in UserCheckInfo)
            {
                if (item.HazardousName == null)
                {
                    item.HazardousName = "不详";
                }
                else
                {
                    if ((item.HazardousName.Contains(",") || item.HazardousName.Contains("，")) && !item.HazardousName.Contains("(") || !item.HazardousName.Contains(")"))
                        item.HazardousName = item.HazardousName.Replace(",", "、").Replace("，", "、");//把标点符号进行转化
                }
            }
            var HazardousGrp = (from p in UserCheckInfo.AsEnumerable()
                                group p by new
                                {
                                    p.HazardousName,
                                }).Select(x => new UserHazardousCountOut
                                {
                                    HazardousName = x.Key.HazardousName,
                                    ContactCount = x.Count()//每个危害因素接触的人数
                                }).ToList();
            #endregion

            #region 【2】根据危害因素名称，进行接触人数、职业禁忌症、疑似职业病赋值
            foreach (var item in ResultSummary)
            {
                if (item.HazardFactors == null)
                {
                    item.HazardFactors = "不详";
                }
                else
                {
                    if ((item.HazardFactors.Contains(",") || item.HazardFactors.Contains("，")) && !item.HazardFactors.Contains("(") || !item.HazardFactors.Contains(")"))
                        item.HazardFactors = item.HazardFactors.Replace(",", "、").Replace("，", "、");//把标点符号进行转化
                }
            }
            foreach (var item1 in HazardousGrp)
            {
                foreach (var item2 in ResultSummary)
                {
                    if (item1.HazardousName == item2.HazardFactors)
                    {
                        if (item2.CheckResult == StriingDTO.Result.Result6 || item2.ReviewResult == StriingDTO.Result.Result6)//初检结果为查职业禁忌症或者复检结果为职业禁忌症
                        {
                            item1.JinjiCount += 1;
                            continue;
                        }
                        else if (item2.CheckResult == StriingDTO.Result.Result5 && item2.ReviewResult == "暂无数据")//初检结果为疑似职业病但无复检信息
                        {
                            item1.YisiCount += 1;
                            continue;
                        }
                        else if (item2.ReviewResult == StriingDTO.Result.Result5)//初检结果为复查或者疑似职业病复检结果为疑似职业病
                        {
                            item1.YisiCount += 1;
                            continue;
                        }
                    }
                }
            }
            #endregion
            return HazardousGrp.ToList();
        }
        /// <summary>
        /// 获取此单位所有的危害因素
        /// </summary>
        /// <param name="UserCheckInfo"></param>
        /// <returns></returns>
        public string GetHazardousToCompany(List<UserCheckInfoOutPut> UserCheckInfo)
        {            
            string StrHazardousName = "";
            foreach (var item in UserCheckInfo)
            {
                //危害因素不为空
                if (item.HazardousName != null&&!StrHazardousName.Contains(item.HazardousName)&&!item.HazardousName.Contains("不详"))
                StrHazardousName += item.HazardousName + "、";
            }          
            List<string> List = StrHazardousName.Replace(",", "、").Replace("，", "、").Split("、").ToList(); //把标点符号进行转化分割

            StrHazardousName = "";          
            foreach (var item in List)
            {
                if(!StrHazardousName.Contains(item))
                    StrHazardousName += item + "、";
            }
            string NewStr= StrHazardousName.Remove(StrHazardousName.Length - 1);
            return NewStr;
        }

        /// <summary>
        /// 获取用户的初检结论、建议
        /// </summary>
        /// <param name="UserCheckInfo"></param>
        /// <returns></returns>
        public List<UserCheckOutPut> GetUserConclusion(List<UserCheckOutPut> FirstResult)
        {
            //注意:用户体检的明细项目可能没有=》纯音测听
            //如果没有的话而我通过这个 纯音测听 这个明细项目去获取用户项目详细结果和用户信息的话就会得不到的数据
            //所以就得分情况讨论
            List<UserCheckOutPut> OutPut = new List<UserCheckOutPut>();
            #region 【1】纯音测听这个体检项目的人员
            var grplinq = (from p in FirstResult.Where(x => x.Comb_Name.Contains("纯音测听")).AsEnumerable()
                           group p by new
                           {
                               p.UserCheckId,
                               p.UserName,
                               p.Sex,
                               p.Age,
                               p.CompanyId,
                               p.TypeName,
                               p.ChecksetMealName,
                               p.HazardousName,
                               p.IsCheckFinish,
                               p.Comb_Name,//组合名称
                               p.Comb_Result,//组合检查结果
                               p.Conclusion,
                               p.Opption,
                               p.CheckResult,
                           }).Select(x => new UserCheckOutPut
                           {
                               UserCheckId = x.Key.UserCheckId,
                               UserName = x.Key.UserName,
                               Sex = x.Key.Sex,
                               Age = x.Key.Age,
                               CompanyId = x.Key.CompanyId,
                               TypeName = x.Key.TypeName,
                               ChecksetMealName = x.Key.ChecksetMealName,
                               HazardousName = x.Key.HazardousName,
                               IsCheckFinish = x.Key.IsCheckFinish,
                               Comb_Name = x.Key.Comb_Name,
                               Comb_Result = x.Key.Comb_Result,
                               Conclusion = x.Key.Conclusion,
                               Opption = x.Key.Opption,
                               CheckResult = x.Key.CheckResult,
                           });//至获取到组合项目==》纯音测听的结果

            #endregion
            #region 【2】所有人员
            var grplinq2 = (from p in FirstResult.AsEnumerable()
                            group p by new
                            {
                                p.UserCheckId,
                                p.UserId,
                                p.UserName,
                                p.Sex,
                                p.Age,
                                p.CompanyId,
                                p.TypeName,
                                p.ChecksetMealName,
                                p.HazardousName,
                                p.IsCheckFinish,
                                p.Conclusion,
                                p.Opption,
                                p.CheckResult,
                            }).Select(x => new UserCheckOutPut
                            {
                                UserCheckId = x.Key.UserCheckId,
                                UserId = x.Key.UserId,
                                UserName = x.Key.UserName,
                                Sex = x.Key.Sex,
                                Age = x.Key.Age,
                                CompanyId = x.Key.CompanyId,
                                TypeName = x.Key.TypeName,
                                ChecksetMealName = x.Key.ChecksetMealName,
                                HazardousName = x.Key.HazardousName,
                                IsCheckFinish = x.Key.IsCheckFinish,
                                Comb_Result = "暂无体检数据",
                                Conclusion = x.Key.Conclusion,
                                Opption = x.Key.Opption,
                                CheckResult = x.Key.CheckResult,
                            });

            #endregion
            foreach (var item in grplinq)
            {
                OutPut.Add(item);
            }
            foreach (var item in grplinq2)
            {
                //若果集合中存在这个用户就不添加
                if (OutPut.Where(x => x.UserId == item.UserId || x.UserName == item.UserName).Count() == 0)
                    OutPut.Add(item);
            }
            return OutPut;
        }
        /// <summary>
        /// 获取用户的复检结论、建议
        /// </summary>
        /// <param name="ReviewResult"></param>
        /// <param name="NumberDate">在初检日期的30日内的复检信息</param>
        /// <returns></returns>
        public List<UserCheckOutPut> GetUserReviewConclusion(List<UserCheckOutPut> ReviewResult, string CheckDate)
        {
            #region 把复检日期转换成int类型用来获取最近的复检信息           
            int Date = Convert.ToInt32(Convert.ToDateTime(CheckDate).AddDays(30).ToString("yyyyMMdd"));
            foreach (var item in ReviewResult)
            {
                if (item.CheckDate != null || item.CheckDate != "")
                {
                    item.NumberDate = Convert.ToDateTime(item.CheckDate);
                }
            }
            #endregion

            #region 【1】通过体检号、项目名称进行分组合并数据
            var grplinq1 = (from p in ReviewResult.Where(x => Convert.ToInt32(x.NumberDate.ToString("yyyyMMdd")) < Date).AsEnumerable()//初检日期+30天内>复检日期
                            group p by new
                            {
                                p.UserCheckId,
                                p.UserId,
                                p.UserName,
                                p.Sex,
                                p.Age,
                                p.CompanyId,
                                p.CheckDate,//体检日期
                                p.TypeName,
                                p.ChecksetMealName,
                                p.HazardousName,
                                p.IsCheckFinish,
                                p.Conclusion,
                                p.Opption,
                                p.CheckResult,
                                p.ProjectName,
                            }).Select(x => new UserCheckOutPut
                            {
                                UserCheckId = x.Key.UserCheckId,
                                UserId = x.Key.UserId,
                                UserName = x.Key.UserName,
                                Sex = x.Key.Sex,
                                Age = x.Key.Age,
                                CompanyId = x.Key.CompanyId,
                                CheckDate = x.Key.CheckDate,
                                TypeName = x.Key.TypeName,
                                ChecksetMealName = x.Key.ChecksetMealName,
                                HazardousName = x.Key.HazardousName,
                                IsCheckFinish = x.Key.IsCheckFinish,
                                Conclusion = x.Key.Conclusion,
                                Opption = x.Key.Opption,
                                CheckResult = x.Key.CheckResult,
                                ProjectName = x.Key.ProjectName,
                            });
            #endregion

            #region 【2】对项目名称进行合并
            var grplinq2 = (from p in grplinq1.AsEnumerable()
                            group p by new
                            {
                                p.UserCheckId,
                                p.UserId,
                                p.UserName,
                                p.Sex,
                                p.Age,
                                p.CompanyId,
                                p.TypeName,
                                p.ChecksetMealName,
                                p.HazardousName,
                                p.IsCheckFinish,
                                p.Conclusion,
                                p.Opption,
                                p.CheckResult,
                            }).Select(x => new UserCheckOutPut
                            {
                                UserCheckId = x.Key.UserCheckId,
                                UserId = x.Key.UserId,
                                UserName = x.Key.UserName,
                                Sex = x.Key.Sex,
                                Age = x.Key.Age,
                                CompanyId = x.Key.CompanyId,
                                TypeName = x.Key.TypeName,
                                ChecksetMealName = x.Key.ChecksetMealName,
                                HazardousName = x.Key.HazardousName,
                                IsCheckFinish = x.Key.IsCheckFinish,
                                Conclusion = x.Key.Conclusion,
                                Opption = x.Key.Opption,
                                CheckResult = x.Key.CheckResult,
                                ProjectName = string.Join(",", x.Select(x => x.ProjectName).ToArray()),//对ProjectName进行处理，进行字符串拼接，并用逗号进行分隔
                            });
            #endregion

            return grplinq2.ToList();
        }
        /// <summary>
        /// 整理合并初次体检和复检的信息
        /// </summary>
        /// <param name="FirstCheckOut">初次体检</param>
        /// <param name="ReviewCheckOut">复检</param>
        /// <returns></returns>
        public List<CheckResultOut> GetCheckResultSummary(List<UserCheckOutPut> FirstCheckOut, List<UserCheckOutPut> ReviewCheckOut)
        {
            List<CheckResultOut> list = new List<CheckResultOut>();
            foreach (var item1 in FirstCheckOut)
            {
                //判断复检信息中是否含有这个人的信息
                var date = ReviewCheckOut.Where(x => x.UserId == item1.UserId).FirstOrDefault();
                if (date != null)//有复检信息就进行合并
                    list.Add(new CheckResultOut()
                    {
                        UserName = date.UserName,
                        Sex = date.Sex,
                        Age = item1.Age,
                        HazardFactors = item1.HazardousName,
                        TypeName = item1.TypeName,
                        CheckResult = item1.CheckResult,//初检结果
                        ReviewResult = date.CheckResult,//复检结果
                        Conclusion = date.Conclusion,
                        Opption = date.Opption,
                        PureToneAudiometryResults = item1.Comb_Result,//项目组合结果（纯音测听）
                    });
                else
                    list.Add(new CheckResultOut()
                    {
                        UserName = item1.UserName,
                        Sex = item1.Sex,
                        Age = item1.Age,
                        HazardFactors = item1.HazardousName,
                        TypeName = item1.TypeName,
                        CheckResult = item1.CheckResult,
                        ReviewResult = "暂无数据",
                        Conclusion = item1.Conclusion,
                        Opption = item1.Opption,
                        PureToneAudiometryResults = item1.Comb_Result,//项目组合结果（纯音测听）
                    });
            }
            return list;
        }
        /// <summary>
        /// 统计各项目明细不合格的人数
        /// </summary>
        /// <param name="FirstResult"></param>
        /// <returns></returns>
        public List<UserCheckOutPut> ResultAnalyse(List<UserCheckOutPut> FirstResult)
        {
            #region 先用体检号相同把名字相同的数据分组合并(防止用户表中可能有相同的数据=》多次读取结果)
            var grplinq1 = (from p in FirstResult.AsEnumerable().Where(x => x.IsStandard == "T")
                            group p by new
                            {
                                p.UserCheckId,
                                p.ProjectId,
                                p.ProjectName,
                                p.ProjectDetailsId,
                                p.ProjectDetailsName,
                            }).Select(x => new UserCheckOutPut
                            {
                                ProjectId = x.Key.ProjectId,
                                ProjectName = x.Key.ProjectName,
                                ProjectDetailsId = x.Key.ProjectDetailsId,
                                ProjectDetailsName = x.Key.ProjectDetailsName.Replace(" ", ""),
                                Count = x.Count()//分组后每一组的个数也就是我要统计的人数
                            });
            #endregion

            #region 分组统计
            var grplinq2 = (from p in grplinq1.AsEnumerable()
                            group p by new
                            {
                                p.ProjectId,
                                p.ProjectName,
                                p.ProjectDetailsId,
                                p.ProjectDetailsName,
                            }).Select(x => new UserCheckOutPut
                            {
                                ProjectId = x.Key.ProjectId,
                                ProjectName = x.Key.ProjectName,
                                ProjectDetailsId = x.Key.ProjectDetailsId,
                                ProjectDetailsName = x.Key.ProjectDetailsName.Replace(" ", ""),
                                Count = x.Count()//分组后每一组的个数也就是我要统计的人数
                            });
            #endregion

            return grplinq2.ToList();
        }
        /// <summary>
        /// 统计各项目明细结果不合格的人数
        /// </summary>
        /// <param name="FirstResult"></param>
        /// <returns></returns>
        public List<UserCheckOutPut> ProjectDetailsResultAnalyse(List<UserCheckOutPut> FirstResult)
        {
            #region 先用体检号相同把名字相同的数据分组合并(防止用户表中可能有相同的数据=》多次读取结果)
            var grplinq = (from p in FirstResult.AsEnumerable().Where(x => x.IsStandard == "T")
                           group p by new
                           {
                               p.UserCheckId,
                               p.ProjectId,
                               p.ProjectName,
                               p.ProjectDetailsId,
                               p.ProjectDetailsName,
                               p.ProjectResult,//项目详细结果
                           }).Select(x => new UserCheckOutPut
                           {
                               ProjectId = x.Key.ProjectId,
                               ProjectName = x.Key.ProjectName,
                               ProjectDetailsId = x.Key.ProjectDetailsId,
                               ProjectDetailsName = x.Key.ProjectDetailsName.Replace(" ", ""),
                               ProjectResult = x.Key.ProjectResult.Replace(" ", ""),
                               Count = x.Count()//分组后每一组的个数也就是我要统计的人数
                           });
            #endregion

            var grplinq1 = (from p in grplinq.AsEnumerable()
                            group p by new
                            {
                                p.ProjectId,
                                p.ProjectName,
                                p.ProjectDetailsId,
                                p.ProjectDetailsName,
                                p.ProjectResult,//项目详细结果
                            }).Select(x => new UserCheckOutPut
                            {
                                ProjectId = x.Key.ProjectId,
                                ProjectName = x.Key.ProjectName,
                                ProjectDetailsId = x.Key.ProjectDetailsId,
                                ProjectDetailsName = x.Key.ProjectDetailsName,
                                ProjectResult = x.Key.ProjectResult,
                                Count = x.Count()//分组后每一组的个数也就是我要统计的人数
                            });

            return grplinq.ToList();
        }

        /// <summary>
        /// 获取各个项目体检人数的统计
        /// </summary>
        /// <returns></returns>
        public List<UserCheckOutPut> GetProjectCount(List<UserCheckOutPut> FirstResult)
        {
            #region 每项项目分类体检人数统计
            //SQL语句==》select ProjectId,ProjectName, count(UserID)  from linq  group by ProjectId,ProjectName
            //说明：先按ProjectId和ProjectName进行归类(此时把ProjectId和ProjectName看成是一组==》每一行的ProjectId和ProjectName的值相同即可看成一组)，
            //【1】获取获取每个人的体检项目
            var grplinq1 = (from p in FirstResult.AsEnumerable()
                            group p by new
                            {
                                p.UserCheckId,
                                p.ProjectId,
                                p.ProjectName,
                            }).Select(x => new UserCheckOutPut
                            {
                                UserCheckId = x.Key.UserCheckId,
                                ProjectName = x.Key.ProjectName,
                                ProjectId = x.Key.ProjectId,
                            });
            //【2】每项项目分类体检人数统计
            var grplinq2 = (from p in grplinq1.AsEnumerable()
                            group p by new
                            {
                                p.ProjectId,
                                p.ProjectName,
                            }).Select(x => new UserCheckOutPut
                            {
                                ProjectName = x.Key.ProjectName,
                                ProjectId = x.Key.ProjectId,
                                Count = x.Count()//分组后每一组的个数也就是我要统计的人数
                            });
            #endregion
            return grplinq2.ToList();
        }

        /// <summary>
        /// 获取各个项目明细体检人数的统计
        /// </summary>
        /// <returns></returns>
        public List<UserCheckOutPut> GetProjectDetailsCount(List<UserCheckOutPut> FirstResult)
        {
            //【1】对用户体检的项目明细进行分组
            var grplinq1 = (from p in FirstResult.AsEnumerable()
                            group p by new
                            {
                                p.UserCheckId,
                                p.ProjectId,
                                p.ProjectName,
                                p.ProjectDetailsId,
                                p.ProjectDetailsName,
                            }).Select(x => new UserCheckOutPut
                            {
                                UserCheckId = x.Key.UserCheckId,
                                ProjectName = x.Key.ProjectName,
                                ProjectId = x.Key.ProjectId,
                                ProjectDetailsId = x.Key.ProjectDetailsId,
                                ProjectDetailsName = x.Key.ProjectDetailsName,
                            });
            //【2】分组统计每个明细项目有多少人
            var grplinq2 = (from p in grplinq1.AsEnumerable()
                            group p by new
                            {
                                p.ProjectId,
                                p.ProjectName,
                                p.ProjectDetailsId,
                                p.ProjectDetailsName,
                            }).Select(x => new UserCheckOutPut
                            {
                                ProjectName = x.Key.ProjectName,
                                ProjectId = x.Key.ProjectId,
                                ProjectDetailsId = x.Key.ProjectDetailsId,
                                ProjectDetailsName = x.Key.ProjectDetailsName,
                                Count = x.Count()//分组后每一组的个数也就是我要统计的人数
                            });
            return grplinq2.ToList();
        }
        /// <summary>
        /// 获取此次体检的项目组合
        /// </summary>
        /// <param name="FirstResult"></param>
        /// <returns></returns>
        public List<UserCheckOutPut> GetCombCodeCount(List<UserCheckOutPut> FirstResult)
        {
            //【1】通过组合代码和组合名称进行分组
            var grplinq1 = (from p in FirstResult.AsEnumerable()
                            group p by new
                            {
                                p.Comb_Code,
                                p.Comb_Name,
                            }).Select(x => new UserCheckOutPut
                            {
                                Comb_Code = x.Key.Comb_Code,
                                Comb_Name = x.Key.Comb_Name,
                            });

            return grplinq1.ToList();
        }
        #endregion
    }
}
