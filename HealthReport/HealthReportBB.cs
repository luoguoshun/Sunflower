using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HealthReport.CaseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using static OHEXML.Contracts.HealthReportModule.HealthReportDTO;
using A = DocumentFormat.OpenXml.Drawing;

namespace HealthReport
{
    public class HealthReportBB
    {
        #region 全局变量
        //当前程序集的相对路径(在构造函数初始化)
        private readonly string basePath;
        //空模板的地址(在构造函数初始化)
        private readonly WordprocessingDocument _document;
        //设置页面word的DocProperties(文档属性)元素（uint则是不带符号的，表示范围是：2^32即0到4294967295）
        public uint docId;
        //分页页数统计
        public int PageCount;

        //获取空模板的主文档
        protected MainDocumentPart NullMainPart => _document.MainDocumentPart;
        //获取空模板的身体部分（不包括页眉、页脚、图片(属于引用部件)）
        protected Body NullBody => NullMainPart.Document.Body;
        public DateTime TimeNow { get; set; }
        public string UnitName { get; set; }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public HealthReportBB(string UnitName, DateTime TimeNow)
        {
            // 使用path获取当前应用程序集的执行目录的上级的上级目录==》basePath = Path.GetFullPath("../..")
            //获取模板当前的相对路径
            basePath = Environment.CurrentDirectory + @"\wwwroot\Templates2";
            _document = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\Base.docx");
            this.TimeNow = TimeNow;
            this.UnitName = UnitName;
        }
        /// <summary>
        /// 添加首页
        /// </summary>
        /// <param name="companyOutOut"></param>
        /// <param name="CheckDate">体检日期</param>
        /// <returns></returns>
        public HealthReportBB AddMainPage(CompanyOutPut companyOutOut, string CheckDate)
        {
            using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\1-Main.docx");
            var Templatebody = template.MainDocumentPart.Document.Body;
            FisrtTable();
            return AddPage(template);
            void FisrtTable()
            {
                //把获取到的数据放到list集合中
                List<string> list = new List<string>()
                    {
                        companyOutOut.CompanyName.ToString(),
                        "广东省职业病防治院",
                        "在岗期间职业健康检查 ",
                        "粤职健协职检2016099号",
                        CheckDate.Replace(".","/"),
                        TimeNow.ToString("yyyy/MM/dd"),
                        "020-34063261",
                        "020-34063261",
                        "020-84456797",
                    };
                var rowList = Templatebody.Descendants<Table>().First().Descendants<TableRow>().ToList();
                for (int i = 0; i < list.Count(); i++)
                {
                    //设置表二第三个单元格
                    rowList[i].Descendants<TableCell>().ElementAt(2).Descendants<Text>().First().Text = list[i];
                }

            }
        }
        /// <summary>
        /// 添加说明页
        /// </summary>
        /// <returns></returns>
        public HealthReportBB AddExplain(CompanyOutPut companyUnitOutOut, List<UserCheckInfoOutPut> CheckInfo)
        {using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\2-Explain.docx");
            var Templatebody = template.MainDocumentPart.Document.Body;
            var PLast = Templatebody.Descendants<Paragraph>().Last().Descendants<Run>();
            List<string> list = new List<string> { companyUnitOutOut.CompanyName, CheckInfo.Count().ToString(), };
            int i = 0;
            foreach (var item in PLast)
            {
                if (item.Descendants<Text>().First().Text.Contains("*"))
                {
                    item.Descendants<Text>().First().Text = list[i];
                    i++;
                }
            }
            return AddPage(template);
        }
        /// <summary>
        /// 添加总结报告
        /// </summary>
        /// <param name="companyUnitOutOut">体检单位信息</param>
        /// <param name="AllCheckInfo">此次体检登记的用户</param>
        /// <param name="ResultSummary">此次完成体检用户的体检结果</param>
        /// <param name="StrHazardousName">该公司的危害因素</param>
        /// <returns></returns>
        public HealthReportBB AddSummary1(CompanyOutPut companyUnitOutOut, List<UserCheckInfoOutPut> AllCheckInfo, List<CheckResultOut> ResultSummary, string StrHazardousName)
        {using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\3-summary1.docx");
            var Templatebody = template.MainDocumentPart.Document.Body;
            var ParagraphList = Templatebody.Descendants<Paragraph>().ToList();
            string[] Datestr = null;
            if (AllCheckInfo.FirstOrDefault().CheckDate != null)
            {
                Datestr = AllCheckInfo.FirstOrDefault().CheckDate.Split(".");
            }
            List<string> P5list = new List<string> { companyUnitOutOut.CompanyName, companyUnitOutOut.CompanyName, StrHazardousName, companyUnitOutOut.CompanyName, Datestr[0] ?? "--", Datestr[1] ?? "--", Datestr[2] ?? "--", Datestr[1] ?? "--", Datestr[2] ?? "--", companyUnitOutOut.CompanyName, companyUnitOutOut.Address ?? "xxxx", "xxxxxxxx", AllCheckInfo.Count().ToString(), ResultSummary.Count().ToString(), (ResultSummary.Count() * 100 / AllCheckInfo.Count).ToString("0.0"), AllCheckInfo.Where(x => x.TypeName.Contains("岗前")).Count().ToString(), AllCheckInfo.Where(x => x.TypeName.Contains("在岗期间")).Count().ToString(), AllCheckInfo.Where(x => x.TypeName.Contains("离岗")).Count().ToString(), companyUnitOutOut.CompanyName, StrHazardousName, AllCheckInfo.Count().ToString(), };
            int k = 0;
            foreach (var item in ParagraphList)
            {
                if (item.InnerText.Contains("*"))
                {
                    var RunList = item.Descendants<Run>().ToList();
                    for (int j = 0; j < RunList.Count; j++)
                    {
                        if (RunList[j].InnerText.Contains("*"))
                        {
                            RunList[j].Descendants<Text>().First().Text = P5list[k];
                            k++;
                        }
                    }
                }
            }
            return AddPage(template);
        }
        /// <summary>
        /// 添加总结报告2
        /// </summary>
        /// <param name="ResultAnalyse"></param>
        /// <param name="getProjectCount"></param>
        /// <param name="HazardousCount"></param>
        /// <returns></returns>
        public HealthReportBB AddSummary2(List<UserCheckOutPut> ResultAnalyse, List<UserCheckOutPut> getProjectCount, List<UserHazardousCountOut> HazardousCount)
        {using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\4-summary2.docx");
            var Templatebody = template.MainDocumentPart.Document.Body;
            var ParagraphList = Templatebody.Descendants<Paragraph>().ToList();
            int yisi = HazardousCount.Where(x => x.YisiCount != 0).Count();
            int jinji = HazardousCount.Where(x => x.JinjiCount != 0).Count();
            var text1 = ParagraphList.ElementAt(1).Descendants<Run>().ElementAt(1).Descendants<Text>().First();
            var text2 = ParagraphList.ElementAt(2).Descendants<Run>().ElementAt(1).Descendants<Text>().First();
            text1.Text = "本次年度检查发现有";
            text2.Text = "本次年度检查发现有";
            if (yisi.Equals(0))
            {
                text1.Text = "本次年度检查未发现相应危害因素的疑似职业病";
            }
            else
            {
                foreach (var item in HazardousCount.Where(x => x.YisiCount != 0))
                {
                    text1.Text += $@"{item.YisiCount}例{item.HazardousName}危害因素的疑似职业病、";
                }
            }
            if (jinji.Equals(0))
            {
                text2.Text = "本次年度检查未发现相应危害因素的职业禁忌症";
            }
            else
            {
                foreach (var item in HazardousCount.Where(x => x.JinjiCount != 0))
                {
                    text2.Text += $@"{item.JinjiCount}例{item.HazardousName}危害因素的疑似职业病、";
                }
            }
            Tige();
            XinDainTu();
            GabDanBi();
            fangSheKe();
            ShiyanShi();
            void Tige()
            {
                var p5_2 = ParagraphList.ElementAt(4).Descendants<Run>().ElementAt(2).Descendants<Text>().First();
                var p5_5 = ParagraphList.ElementAt(4).Descendants<Run>().Last().Descendants<Text>().First();
                p5_5.Text = "";
                var NeiKe = getProjectCount.Where(x => x.ProjectName.Contains("内科") || x.ProjectId == "01").FirstOrDefault();
                var result = ResultAnalyse.Where(x => x.ProjectName.Contains("内科") || x.ProjectId == "01").ToList();
                if (NeiKe == null)
                {
                    p5_2.Text = "0";
                    p5_5.Text = "目前未见异常";
                }
                else
                {
                    p5_2.Text = NeiKe.Count.ToString();
                    if (result.Count() != 0)
                    {
                        int XueyaCount = 0;
                        foreach (var item in result)
                        {
                            if (item.ProjectDetailsName.Contains("舒张压") || item.ProjectDetailsName.Contains("收缩压"))
                            {
                                XueyaCount += item.Count;
                            }
                        }
                        p5_5.Text = $@"血压偏高（包括单纯收缩压、舒张压偏高或收缩压、舒张压均偏高）{XueyaCount}人，占受检人数的 {XueyaCount / (double)NeiKe.Count * 100:0.0}%;";
                        foreach (var item in result)
                        {
                            if (!item.ProjectDetailsName.Contains("收缩压") && !item.ProjectDetailsName.Contains("舒张压"))
                            {
                                string text = $@"检出{item.ProjectDetailsName}异常{item.Count}人，占受检人数的 {(item.Count / (double)NeiKe.Count) * 100:0.0} %;";
                                p5_5.Text += text;
                            }
                        }
                    }
                    else
                    {
                        p5_5.Text = "目前未见异常";
                    }
                }
            };
            void XinDainTu()
            {
                var p6_3 = ParagraphList.ElementAt(5).Descendants<Run>().ElementAt(3).Descendants<Text>().First();
                var p6_6 = ParagraphList.ElementAt(5).Descendants<Run>().Last().Descendants<Text>().First();
                p6_6.Text = "";
                var XinDianTu = getProjectCount.Where(x => x.ProjectName == "心电图" || x.ProjectId == "17").FirstOrDefault();
                var result = ResultAnalyse.Where(x => x.ProjectName.Contains("心电图") || x.ProjectId == "17").ToList();
                if (XinDianTu == null)
                {
                    p6_3.Text = "0";
                    p6_6.Text = "目前未见异常";
                }
                else
                {
                    p6_3.Text = XinDianTu.Count.ToString();
                    if (result.Count() != 0)
                    {
                        foreach (var item in result)
                        {
                            string text = $@"检出{item.ProjectDetailsName}异常{item.Count}人，占受检人数的 {(item.Count / (double)XinDianTu.Count) * 100:0.0} %;";
                            p6_6.Text += text;
                        }
                    }
                    else
                    {
                        p6_6.Text = "目前未见异常";
                    }
                }
            }
            void GabDanBi()
            {
                var p7_6 = ParagraphList.ElementAt(6).Descendants<Run>().ElementAt(5).Descendants<Text>().First();
                var p7_9 = ParagraphList.ElementAt(6).Descendants<Run>().Last().Descendants<Text>().First();
                p7_9.Text = "";
                var BiChao = getProjectCount.Where(x => x.ProjectName.Contains("彩超") || x.ProjectId == "15").FirstOrDefault();
                var result = ResultAnalyse.Where(x => x.ProjectName.Contains("彩超") || x.ProjectId == "15").ToList();
                if (BiChao == null)
                {
                    p7_6.Text = "0";
                    p7_9.Text = "目前未见异常";
                }
                else
                {
                    p7_6.Text = BiChao.Count.ToString();
                    if (result.Count() != 0)
                    {
                        foreach (var item in result)
                        {
                            string text = $@"检出{item.ProjectDetailsName}异常{item.Count}人，占受检人数的 {((double)item.Count / (double)Convert.ToInt32(p7_6.Text)) * 100:0.0} %;";
                            p7_9.Text += text;
                        }
                    }
                    else
                    {
                        p7_9.Text = "目前未见异常";
                    }
                }
            }
            void fangSheKe()
            {
                var p8_4 = ParagraphList.ElementAt(7).Descendants<Run>().ElementAt(3).Descendants<Text>().First();
                var p8_7 = ParagraphList.ElementAt(7).Descendants<Run>().Last().Descendants<Text>().First();
                p8_7.Text = "";
                var FangSheKe = getProjectCount.Where(x => x.ProjectName.Contains("放射科") || x.ProjectId == "19").FirstOrDefault();
                var result = ResultAnalyse.Where(x => x.ProjectName.Contains("放射科") || x.ProjectId == "19").ToList();
                if (FangSheKe == null)
                {
                    p8_4.Text = "0";
                    p8_7.Text = "目前未见异常";
                }
                else
                {
                    p8_4.Text = FangSheKe.Count.ToString();
                    if (result.Count() != 0)
                    {
                        foreach (var item in result)
                        {
                            string text = $@"检出{item.ProjectDetailsName}异常{item.Count}人，占受检人数的 {(item.Count / (double)FangSheKe.Count) * 100:0.0} %;";
                            p8_7.Text += text;
                        }
                    }
                    else
                    {
                        p8_7.Text = "目前未见异常";
                    }
                }
            }
            void ShiyanShi()
            {
                var p9_3 = ParagraphList.ElementAt(8).Descendants<Run>().ElementAt(2).Descendants<Text>().First();
                var p9_6 = ParagraphList.ElementAt(8).Descendants<Run>().Last().Descendants<Text>().First();
                p9_6.Text = "";
                var ShiYanShi = getProjectCount.Where(x => x.ProjectName.Contains("检验科") || x.ProjectId == "06").FirstOrDefault();
                var result = ResultAnalyse.Where(x => x.ProjectName.Contains("检验科") || x.ProjectId == "06").ToList();
                p9_3.Text = ShiYanShi.Count.ToString();
                if (ShiYanShi == null)
                {
                    p9_3.Text = "0";
                    p9_6.Text = "目前未见异常";
                }
                if (result.Count != 0)
                {
                    p9_3.Text = ShiYanShi.Count.ToString();
                    foreach (var item in result)
                    {
                        string text = $@"检出{item.ProjectDetailsName}异常{item.Count}人，占受检人数的 {((double)item.Count / (double)Convert.ToInt32(p9_3.Text)) * 100:0.0} %;";
                        p9_6.Text += text;
                    }
                }
                else
                {
                    p9_6.Text = "目前未见异常";
                }
            }
            var p12_5 = ParagraphList.ElementAt(11).Descendants<Run>().ElementAt(4).Descendants<Text>().First();
            p12_5.Text = yisi.ToString();
            var p13_2 = ParagraphList.ElementAt(12).Descendants<Run>().ElementAt(1).Descendants<Text>().First();
            p13_2.Text = jinji.ToString();
            ;
            return AddPage(template);
        }
        /// <summary>
        /// 添加健康卡页面
        /// </summary>
        /// <returns></returns>
        public HealthReportBB AddHealthCard(CompanyOutPut companyOutPut, List<UserHazardousCountOut> HazardousCount)
        {using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\6-HealthCard.docx");
            var Templatebody = template.MainDocumentPart.Document.Body;
            FirstTable();
            SecondTable();
            ThirdTable();
            return AddPage(template);
            void FirstTable()
            {
                var table = Templatebody.Descendants<Table>().ElementAt(0);
                var RowList = table.Descendants<TableRow>().ToList();
                List<string> list = new List<string> { "广东省职业病防治院", "45585827-3", companyOutPut.CompanyName.ToString(), companyOutPut.CompanyCode ?? "请填写", };
                RowList[0].Descendants<TableCell>().ElementAt(1).Descendants<Text>().First().Text = list[0];
                RowList[0].Descendants<TableCell>().ElementAt(3).Descendants<Text>().First().Text = list[1];
                RowList[1].Descendants<TableCell>().ElementAt(1).Descendants<Text>().First().Text = list[2];
                RowList[1].Descendants<TableCell>().ElementAt(3).Descendants<Text>().First().Text = list[3];
            }
            void SecondTable()
            {
                var table = Templatebody.Descendants<Table>().ElementAt(1);
                var RowList = table.Descendants<TableRow>().ToList();
                List<string> list = new List<string> { companyOutPut.Address ?? "请填写", companyOutPut.CompanyCode ?? "请填写", companyOutPut.UserName ?? "请填写", companyOutPut.Telephone ?? "请填写", companyOutPut.EconomicType ?? "请填写", companyOutPut.Industry ?? "请填写", };
                for (int i = 0; i < RowList.Count() - 4; i++)
                {
                    RowList[i].Descendants<TableCell>().ElementAt(1).Descendants<Text>().First().Text = list[i];
                }
                if (companyOutPut.CountPeople > 1000)
                {
                    RowList[7].Remove();
                    RowList[8].Remove();
                    RowList[9].Remove();
                }
                else if (500 <= companyOutPut.CountPeople && companyOutPut.CountPeople <= 1000)
                {
                    RowList[6].Remove();
                    RowList[8].Remove();
                    RowList[9].Remove();
                }
                else if (0 < companyOutPut.CountPeople && companyOutPut.CountPeople < 500)
                {
                    RowList[6].Remove();
                    RowList[7].Remove();
                    RowList[9].Remove();
                }
                else
                {
                    RowList[6].Remove();
                    RowList[7].Remove();
                    RowList[8].Remove();
                }
            }
            void ThirdTable()
            {
                var plist = Templatebody.Descendants<Paragraph>().ToList();
                var p26 = plist.ElementAt(26).Descendants<Run>();
                List<string> list = new List<string> { companyOutPut.SumNumber == 0 ? "0" : companyOutPut.SumNumber.ToString(), companyOutPut.WomenWrkers == 0 ? "0" : companyOutPut.WomenWrkers.ToString(), companyOutPut.ProWorkers == 0 ? "0" : companyOutPut.ProWorkers.ToString(), companyOutPut.WomenProWorkers == 0 ? "0" : companyOutPut.WomenProWorkers.ToString(), companyOutPut.HarmfulWork == 0 ? "0" : companyOutPut.HarmfulWork.ToString(), companyOutPut.HarmfulWomenProWorkers == 0 ? "0" : companyOutPut.HarmfulWomenProWorkers.ToString() };
                int k = 0;
                foreach (var item in p26.ToList())
                {
                    if (item.InnerText.Contains("*"))
                    {
                        for (int j = 0; j < p26.ToList().Count(); j++)
                        {
                            if (p26.ToList()[j].InnerText.Contains("*"))
                            {
                                p26.ToList()[j].Descendants<Text>().First().Text = list[k];
                                k++;
                            }
                        }
                    }
                }
                var table = Templatebody.Descendants<Table>().ElementAt(2);
                var templateRow = table.Descendants<TableRow>().ElementAt(1);
                templateRow.Remove();
                foreach (var item in HazardousCount)
                {
                    var CloneRow = templateRow.CloneNode(true);
                    var CellList = templateRow.Descendants<TableCell>().ToList();
                    List<string> list2 = new List<string> { item.HazardousName.ToString(), item.ContactCount.ToString(), item.NeedCount.ToString(), item.ActualCount.ToString(), item.YisiCount.ToString(), item.JinjiCount.ToString(), item.JinjiCount.ToString(), };
                    for (int i = 0; i < CellList.Count; i++)
                    {
                        (CloneRow.Descendants<TableCell>().ToList())[i].Descendants<Text>().First().Text = list2[i];
                    }
                    table.AppendChild(CloneRow);
                }
            }
        }
        /// <summary>
        /// 添加检查结果页面1
        /// </summary>
        /// <param name="userCheckOuts"></param>
        /// <param name="userReviewCheckOutPuts"></param>
        /// <returns></returns>
        public HealthReportBB AddCheckResult1(IEnumerable<CheckResultOut> ResultSummary)
        {using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\7-CheckResult.docx");
            var Templatebody = template.MainDocumentPart.Document.Body;
            FirstTable();
            SecondAddThirdTable();
            void FirstTable()
            {
                int count = ResultSummary.Count();
                Templatebody.Descendants<Paragraph>().ElementAt(1).Descendants<Run>().ElementAt(1).Descendants<Text>().First().Text = count.ToString();
                var table = Templatebody.Descendants<Table>().ElementAt(0);
                var RowList = table.Descendants<TableRow>().ToList();
                List<int> ResultNumber = new List<int>();
                int normal = 0, review = 0, probably = 0, taboo = 0, other = 0;
                foreach (var item in ResultSummary)
                {
                    if (item.CheckResult.Contains("复查"))
                    {
                        review++;
                        continue;
                    }
                    else if (item.CheckResult == "疑似职业病")
                    {
                        probably++;
                        continue;
                    }
                    else if (item.CheckResult == "职业禁忌证")
                    {
                        taboo++;
                        continue;
                    }
                    else if (item.CheckResult.Contains("其他疾病或异常"))
                    {
                        other++;
                        continue;
                    }
                    else if (item.CheckResult.Contains("目前未见异常"))
                    {
                        normal++;
                        continue;
                    }
                }
                ResultNumber.Add(normal);
                ResultNumber.Add(review);
                ResultNumber.Add(probably);
                ResultNumber.Add(taboo);
                ResultNumber.Add(other);
                for (int p = 1; p < RowList.Count(); p++)
                {
                    RowList[p].Descendants<TableCell>().ElementAt(1).Descendants<Text>().First().Text = ResultNumber[p - 1].ToString();
                    RowList[p].Descendants<TableCell>().ElementAt(2).Descendants<Text>().First().Text = ((ResultNumber[p - 1] / (double)count) * 100).ToString("00.0") + "%";
                }
            }
            void SecondAddThirdTable()
            {
                var table2 = Templatebody.Descendants<Table>().ElementAt(1);
                var Rowtemplate2 = table2.Descendants<TableRow>().ElementAt(1);
                var table3 = Templatebody.Descendants<Table>().ElementAt(2);
                var Rowtemplate3 = table3.Descendants<TableRow>().ElementAt(1);
                if (ResultSummary.Where(x => x.CheckResult == "疑似职业病").Count() != 0)
                    Rowtemplate2.Remove();
                if (ResultSummary.Where(x => x.CheckResult == "职业禁忌证").Count() != 0)
                    Rowtemplate3.Remove();
                foreach (var item in ResultSummary)
                {
                    if (item.CheckResult == "疑似职业病")
                    {
                        var row = Rowtemplate2.CloneNode(true);
                        var CellCount = row.Descendants<TableCell>().ToList();
                        List<string> list = new List<string> { item.UserName.ToString(), item.Sex.ToString(), item.Age.ToString(), item.HazardFactors.ToString(), item.Conclusion.ToString(), item.CheckResult, item.ReviewResult, item.CheckResult.ToString(), item.Opption.ToString() };
                        for (int i = 0; i < CellCount.Count(); i++)
                        {
                            CellCount[i].Descendants<Text>().First().Text = list[i];
                        }
                        table2.Append(row);
                        break;
                    }
                    else if (item.CheckResult == "职业禁忌证")
                    {
                        var row = Rowtemplate3.CloneNode(true);
                        var CellCount = row.Descendants<TableCell>().ToList();
                        List<string> list = new List<string> { item.UserName.ToString(), item.Sex.ToString(), item.Age.ToString(), item.HazardFactors.ToString(), item.Conclusion.ToString(), item.CheckResult.ToString(), item.Opption.ToString() };
                        for (int i = 0; i < CellCount.Count(); i++)
                        {
                            CellCount[i].Descendants<Text>().First().Text = list[i];
                        }
                        table3.Append(row);
                        break;
                    }
                };
            }
            return AddPage(template);
        }
        /// <summary>
        /// 添加复查人员汇总页面
        /// </summary>
        /// <param name="reviewWorkers"></param>
        /// <returns></returns>
        public HealthReportBB AddCheckResult2(List<UserCheckOutPut> ReviewOutPut)
        {using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\8-ReviewCheck.docx");
            var Templatebody = template.MainDocumentPart.Document.Body;
            var table = Templatebody.Descendants<Table>().First();
            var tempalteRow = table.Descendants<TableRow>().ElementAt(1);
            if (ReviewOutPut.Count != 0)
            {
                tempalteRow.Remove();
            }
            foreach (UserCheckOutPut item in ReviewOutPut)
            {
                var cloneRow = tempalteRow.CloneNode(true);
                var Rowlist = cloneRow.Descendants<TableCell>().ToList();
                List<string> list = new List<string> { item.UserName.ToString(), item.Sex.ToString(), item.Age.ToString(), item.HazardousName.ToString(), item.TypeName.ToString(), item.CheckResult.ToString(), item.ProjectName.ToString(), };
                for (int i = 0; i < Rowlist.Count(); i++)
                {
                    Rowlist[i].Descendants<Text>().First().Text = list[i];
                };
                table.AppendChild(cloneRow);
            }
            return AddPage(template);
        }

        /// <summary>
        /// 规检查及物理检查结果统计数量页面1
        /// </summary>
        /// <param name="resultAnalyse"></param>
        /// <param name="getProjectDetailsCount"></param>
        /// <returns></returns>
        public HealthReportBB AddCheckResult3(IEnumerable<UserCheckOutPut> resultAnalyse, IEnumerable<UserCheckOutPut> getProjectDetailsCount)
        {
            using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\9-1CheckResult.docx");
            var Templatebody = template.MainDocumentPart.Document.Body;
            SetPEtable(Templatebody, resultAnalyse, getProjectDetailsCount);
            return AddPage(template);
        }

        /// <summary>
        /// 化验室检查阳性结果统计
        /// </summary>
        /// <param name="resultAnalyse"></param>
        /// <param name="getProjectDetailsCount"></param>
        /// <returns></returns>
        public HealthReportBB AddCheckResult3_1(IEnumerable<UserCheckOutPut> resultAnalyse, IEnumerable<UserCheckOutPut> getProjectDetailsCount)
        {
            using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\9-2CheckResult.docx");
            var Templatebody = template.MainDocumentPart.Document.Body;
            SetLEtable(Templatebody, resultAnalyse, getProjectDetailsCount);
            return AddPage(template);
        }
        /// <summary>
        /// 添加全部参检人员体检结果及结论页面
        /// </summary>
        /// <param name="checkResult"></param>
        /// <returns></returns>
        public HealthReportBB AddCheckResult4(IEnumerable<CheckResultOut> ResultSummary)
        {using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\10-CheckResult.docx");
            var Templatebody = template.MainDocumentPart.Document.Body;
            var table = Templatebody.Descendants<Table>().First();
            var tempalteRow = table.Descendants<TableRow>().ElementAt(1);
            tempalteRow.Remove();
            var Firstp = Templatebody.Descendants<Paragraph>().Where(x => x.InnerText.Contains("*")).FirstOrDefault();
            Firstp.Descendants<Run>().Where(x => x.InnerText.Contains("*")).FirstOrDefault().Descendants<Text>().First().Text = ResultSummary.Count().ToString();
            int sort = 1;
            foreach (var item in ResultSummary)
            {
                OpenXmlElement RowClone = tempalteRow.CloneNode(true);
                List<TableCell> TableCellList = RowClone.Descendants<TableCell>().ToList();
                List<string> list = new List<string>();
                if (item.ReviewResult == "暂无数据")
                {
                    list.Add(sort.ToString());
                    list.Add(item.UserName.ToString());
                    list.Add(item.Sex.ToString());
                    list.Add(item.Age.ToString());
                    list.Add(item.HazardFactors.ToString());
                    list.Add(item.CheckResult.ToString());
                    list.Add(item.Conclusion.ToString());
                    list.Add(item.Opption.ToString());
                    list.Add(sort.ToString());
                }
                else
                {
                    list.Add(sort.ToString());
                    list.Add(item.UserName.ToString());
                    list.Add(item.Sex.ToString());
                    list.Add(item.Age.ToString());
                    list.Add(item.HazardFactors.ToString());
                    list.Add(item.ReviewResult.ToString());
                    list.Add(item.Conclusion.ToString());
                    list.Add(item.Opption.ToString());
                    list.Add(sort.ToString());
                }
                for (int i = 0; i < TableCellList.Count(); i++)
                {
                    TableCellList[i].Descendants<Text>().First().Text = list[i];
                };
                table.AppendChild(RowClone);
                sort++;
            }
            return AddPage(template);
        }
        /// <summary>
        /// 添加资质证书
        /// </summary>
        /// <returns></returns>
        public HealthReportBB AddRredentialst()
        {
            using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\11-Rredentialst.docx");
            return AddPage(template);
        }
        /// <summary>
        /// 添加体检项目主要负责医生姓名及资质号页面
        /// </summary>
        /// <param name="doctorProjectOutPuts"></param>
        /// <param name="AllCombCodeCount"></param>
        /// <returns></returns>
        public HealthReportBB AddQualificationNumber(IEnumerable<DoctorProjectOutPut> doctorProjectOutPuts, IEnumerable<UserCheckOutPut> AllCombCodeCount)
        {using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\12-QualificationNumber.docx");
            var Templatebody = template.MainDocumentPart.Document.Body;
            var table = Templatebody.Descendants<Table>().First();
            var tempalteRow = table.Descendants<TableRow>().ElementAt(1);
            table.Descendants<TableRow>().ElementAt(1).Remove();
            Dictionary<string, List<string>> keyValuePairs = new Dictionary<string, List<string>>();
            foreach (var item in AllCombCodeCount)
            {
                var Data = doctorProjectOutPuts.Where(x => x.Comb_Code == item.Comb_Code && x.Comb_Name == item.Comb_Name).ToList();
                foreach (var item1 in Data)
                {
                    if (!keyValuePairs.ContainsKey(item1.ProjectName) || !keyValuePairs[item1.ProjectName].Contains(item1.DoctorName.ToString()))
                    {
                        OpenXmlElement RowClone = tempalteRow.CloneNode(true);
                        List<TableCell> TableCellList = RowClone.Descendants<TableCell>().ToList();
                        List<string> list = new List<string> { item1.ProjectName.ToString(), item1.DoctorName.ToString(), item1.Qualification == null ? "暂无数据" : Data.FirstOrDefault().ToString(), "", };
                        for (int i = 0; i < TableCellList.Count(); i++)
                        {
                            TableCellList[i].Descendants<Text>().First().Text = list[i];
                        };
                        table.AppendChild(RowClone);
                    }
                    if (!keyValuePairs.ContainsKey(item1.ProjectName))
                        keyValuePairs.Add(item1.ProjectName, new List<string> { item1.DoctorName.ToString() });
                    if (!keyValuePairs[item1.ProjectName].Contains(item1.DoctorName.ToString()))
                    {
                        List<string> NewList = keyValuePairs[item1.ProjectName].ToList();
                        NewList.Add(item1.DoctorName);
                        keyValuePairs[item1.ProjectName] = NewList;
                    }
                }
            }
            return AddPage(template);
        }
        /// <summary>
        /// 添加疾病解释说明页面1
        /// </summary>
        /// <returns></returns>
        public HealthReportBB AddExplanation1()
        {using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\13-Explanation1.docx");
            return AddPage(template);
        }
        /// <summary>
        /// 添加疾病解释说明页面2
        /// </summary>
        /// <returns></returns>
        public HealthReportBB AddExplanation2()
        {using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\14-Explanation2.docx");
            return AddPage(template);
        }
        /// <summary>
        /// 添加疾病解释说明页面3
        /// </summary>
        /// <returns></returns>
        public HealthReportBB AddExplanation3()
        {using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\15-Explanation3.docx");
            return AddPage(template);
        }
        /// <summary>
        /// 分页方法
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public HealthReportBB AddPage(WordprocessingDocument template)
        {
            #region 设置分页
            //添加新段落用于分页
            if (PageCount > 0)
            {
                var breakP = new Paragraph();
                breakP.AppendChild(new Run())
                    .AppendChild(new RunProperties())
                    .AppendChild(new Break { Type = new EnumValue<BreakValues>(BreakValues.Page) });
                NullBody.InsertAt(breakP, NullBody.ChildElements.Count - 1);
            }
            //如果页数大于0，则PageCount自增
            PageCount++;
            #endregion

            var body = template.MainDocumentPart.Document.Body;
            //删除模板的sectionpropertis(截面属性)元素
            body.RemoveAllChildren<SectionProperties>();

            #region 添加页眉
            //为标题文本声明一个字符串。                   
            string newHeaderText = @$"{UnitName}                                粤职检{TimeNow:yyyyMMdd}
                                    {TimeNow:yyyy}年职业健康检查总结报告                                                      共{PageCount}页";
            if (PageCount == 1)
            {
                //【2】创建一个新的头部分并获取其关系id。
                HeaderPart newHeaderPart = NullMainPart.AddNewPart<HeaderPart>();
                string rId = NullMainPart.GetIdOfPart(newHeaderPart);
                //【3】调用GeneratePageHeaderPart助手方法,传入标题文本，以创建标题标记，然后保存
                GeneratePageHeaderPart(newHeaderText).Save(newHeaderPart);
                //【4】遍历文档中的所有节属性==》SectionProperties
                foreach (var sectProperties in NullMainPart.Document.Descendants<SectionProperties>())
                {
                    //【4-1】 //在节属性里找到所有对头部引用的属性==》HeaderReference进行删除
                    foreach (var headerReference in sectProperties.Descendants<HeaderReference>())
                    {
                        sectProperties.RemoveChild(headerReference);
                    }
                    //【5】创建指向新标题的新标题引用
                    //标题部分并将其添加到节属性
                    var newHeaderReference = new HeaderReference() { Id = rId, Type = HeaderFooterValues.Default };
                    sectProperties.Append(newHeaderReference);
                }
                //保存对主文档部分的更改。
                _document.MainDocumentPart.Document.Save();
            }
            if (PageCount > 1)
            {
                //设置页眉文本
                var HeaderParts = NullMainPart.HeaderParts.ToList();
                HeaderParts.Last().Header
                    .Descendants<Paragraph>().Where(x => x.InnerText.Contains("总结报告")).First()
                      .Descendants<Text>().First()
                       .Text = newHeaderText;
            }
            #endregion

            #region 设置图片部件
            //遍历添加页面word的ImageParts
            foreach (var part in template.MainDocumentPart.ImageParts)
            {
                var oldId = template.MainDocumentPart.GetIdOfPart(part);/*获取template图片部件ID*/
                var newPart = NullMainPart.AddPart(part); /*把template的图片部件添加到Base模板中*/
                var newId = NullMainPart.GetIdOfPart(newPart);/*在Base模板获取新的图片部件ID*/
                //把template里Blip.Embed绑定的图片部件id 替换成 在Base新添加的图片部件id ,在下面从telement复制节点到Base模板时
                //Blip.Embed绑定的图片部件id 正确指向添加新的图片部件
                foreach (var blip in body.Descendants<A.Blip>().Where(b => b.Embed == new StringValue(oldId)))
                {
                    blip.Embed = new StringValue(newId);
                }
            }
            #endregion

            #region 重新设置文档属性Id
            // 遍历设置页面word的DocProperties(文档属性)元素，重新设置它们的Id（防止Id重复）
            foreach (var docPr in body.Descendants<DocProperties>())
            {
                docPr.Id = new UInt32Value(++docId);
            }
            #endregion

            #region 插入数据
            //循环遍历添加word页面
            foreach (var element in body.ChildElements)
            {
                //把编辑好的模板添加到空模板上
                NullBody.InsertAt(element.CloneNode(true), NullBody.ChildElements.Count() - 1);
            }
            #endregion
            return this;
        }

        /// <summary>
        /// 保存word
        /// </summary>
        /// <param name="SavePath"></param>
        public void SaveWord(string SaveWordPath)
        {
            //把可编辑的模板保存到savePath
            //_document是全局变量，在以上一系列操作中，已经把编辑好的==》可编辑的模板添加到==》空白模板上
            var newDocument = _document.SaveAs(SaveWordPath);
            _document.Close();
            _document.Dispose();
            newDocument.Close();
            newDocument.Dispose();
        }

        #region 辅助方法

        /// <summary>
        /// 资源释放
        /// </summary>
        public void Dispose() => _document.Dispose();
        /// <summary>
        /// 修改段落字体
        /// </summary>
        /// <param name="_document"></param>
        public void ApplyStyleToParagraph()
        {
            //获取所有段落集合
            var pList = NullBody.Descendants<Paragraph>().ToList();
            for (int i = 0; i < pList.Count(); i++)
            {
                //假如段落文本不为空
                if (!pList[i].InnerText.Equals(""))
                {
                    //循环段落里的Run文本对RunProperties进行设置值
                    foreach (var item in pList[i].Descendants<Run>().ToList())
                    {
                        //判断运行文本属性是否存在                      
                        if (item.Descendants<RunProperties>().FirstOrDefault() == null)
                        {
                            item.AppendChild(new RunProperties() { });
                        }
                        var runProperties = item.Descendants<RunProperties>().FirstOrDefault();
                        var runFonts = runProperties.Descendants<RunFonts>().FirstOrDefault();
                        if (runFonts == null)
                        {
                            runFonts = new RunFonts() { Ascii = "宋体", HighAnsi = "宋体", EastAsia = "宋体" };
                        }
                        else
                        {
                            runFonts.Ascii = "宋体";
                            runFonts.HighAnsi = "宋体";
                            runFonts.EastAsia = "宋体";
                        }
                    }
                    //保存对主文档部分的更改。
                    _document.Save();
                }
            }
        }
        /// <summary>
        /// 创建头实例并添加其子实例
        /// </summary>
        /// <param name="HeaderText">头部文本</param>
        /// <returns></returns>
        private Header GeneratePageHeaderPart(string HeaderText)
        {
            //将位置设为中心
            PositionalTab pTab = new PositionalTab()
            {
                Alignment = AbsolutePositionTabAlignmentValues.Center,
                RelativeTo = AbsolutePositionTabPositioningBaseValues.Margin,
                Leader = AbsolutePositionTabLeaderCharValues.None
            };
            var element =
              new Header(
                  new Paragraph(
                  new ParagraphProperties(
                    new ParagraphStyleId() { Val = "Header" }),
                     new Run(pTab, new Text(HeaderText))
                )
              );
            return element;
        }
        /// <summary>
        /// 设置物理检查表格
        /// </summary>
        /// <param name="body"></param>
        /// <param name="resultAnalyse"></param>
        /// <param name="getProjectDetailsCount"></param>
        private void SetPEtable(Body body, IEnumerable<UserCheckOutPut> resultAnalyse, IEnumerable<UserCheckOutPut> getProjectDetailsCount)
        {
            //【1】获取表格所有行
            var table = body.Descendants<Table>().First();
            var RowList = table.Descendants<TableRow>().ToList();
            //【2】循环集合获取到第一个单元格文本进行判断
            string[] Str = { "检查项目", "内外五官科检查", "心电图检查", "彩色B超（肝胆脾）", "彩色B超（泌尿系）", "X射线胸片检查", "纯音测听" };
            #region 设置但四个单元格
            //项目详细编号集合(每一个编号对应一个单元格，除了行的InnerText包含在Str数组中)
            List<string> StrProjectDetailsId = new List<string>() {
            "010044、010164、010045、010165",//高血压(收缩压+或舒张压)
            "010045、010165",//舒张压
            "010044、010164",//收缩压
            "320005",//咽部  
            "320004",//鼻部
            "030001",//辨色力(双眼红绿色盲/色弱)
            "030023",//眼底 视乳头生理凹陷扩大
            "030024",//晶状体 晶体混浊
            "170001",//心电图 窦性心动过速
            "170001",//心电图 ST或T波异常
            "170001",//心电图 房室传导阻滞
            "150006",//肝脏  脂肪肝
            "150005",//胆囊 胆囊息肉
            "150005",//胆囊 胆囊结石/肝内胆管结石
            "150006",//肝脏 肝血管瘤
            "150006" ,//肝脏 肝(多发)囊肿   
            "150001",//肾脏 肾（多发）结石
            "150001",//肾脏 肾（多发）囊肿
            "400001、400014",//DR胸部正位+粉尘DR肺纹理增多增粗紊乱
            "400001",//DR胸部正位 陈旧性肺结核
            "400001",//DR胸部正位 纤维条索灶伴胸膜增厚粘连
            "040028",//纯音测听 高频听阈提高
            "040028",//纯音测听 听力损失
            "040028",//纯音测听 语频听阈提高
            };
            int index = 0;
            foreach (TableRow item in RowList)
            {
                if (Str.Contains(item.Descendants<TableCell>().ElementAt(0).InnerText)) continue;
                var Cell = item.Descendants<TableCell>().ToList();
                foreach (var item2 in getProjectDetailsCount)
                {
                    if (StrProjectDetailsId[index].Contains(item2.ProjectDetailsId))
                    {
                        Cell.ElementAt(3).Descendants<Text>().First().Text = (item2.Count).ToString();
                        break;// 设置完直接跳出当前循环 进行下一行的设置
                    }
                }
                index += 1;
            }
            #endregion

            #region 设置但二个单元格
            foreach (var item1 in RowList)
            {
                //判断第一个单元格的文本不在数组中
                if (Str.Contains(item1.Descendants<TableCell>().ElementAt(0).InnerText)) continue;
                string BaiFenBi = "";
                int count = 0;//总人数
                var Cell = item1.Descendants<TableCell>().ToList();
                switch (Cell[0].Descendants<Text>().First().Text)
                {
                    #region 高血压(收缩压和/或舒张压)
                    case "高血压":
                        foreach (var item in resultAnalyse.Where(x => x.ProjectDetailsName.Contains("收缩压") || x.ProjectDetailsName.Contains("收缩压")))
                        {
                            count = Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) + item.Count;
                            Cell.ElementAt(1).Descendants<Text>().First().Text = count.ToString();
                        }
                        //检出人数/体检该项目的人数
                        if (Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text) != 0)
                        {
                            BaiFenBi = (Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) * 100 / Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text)).ToString();
                            Cell.ElementAt(2).Descendants<Text>().First().Text = BaiFenBi;
                        }
                        else
                        {
                            Cell.ElementAt(2).Descendants<Text>().First().Text = 0.ToString();
                        }
                        break;
                    #endregion
                    #region 双眼红绿色盲/色弱
                    case "双眼红绿色盲/色弱":
                        foreach (var item in resultAnalyse.Where(x => x.ProjectResult.Contains("双眼红绿色弱") || x.ProjectResult.Contains("双眼红绿色盲")))
                        {
                            count = Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) + item.Count;
                            Cell.ElementAt(1).Descendants<Text>().First().Text = count.ToString();
                        }

                        //检出人数/体检该项目的人数
                        if (Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text) != 0)
                        {
                            BaiFenBi = (Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) * 100 / Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text)).ToString();
                            Cell.ElementAt(2).Descendants<Text>().First().Text = BaiFenBi;
                        }
                        else
                        {
                            Cell.ElementAt(2).Descendants<Text>().First().Text = 0.ToString();
                        }
                        break;
                    #endregion
                    #region 晶体混浊
                    case "晶体混浊":
                        foreach (var item in resultAnalyse.Where(x => x.ProjectResult.Contains("晶体") && x.ProjectResult.Contains("混浊")))
                        {
                            count = Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) + item.Count;
                            Cell.ElementAt(1).Descendants<Text>().First().Text = count.ToString();
                        }

                        //检出人数/体检该项目的人数
                        if (Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text) != 0)
                        {
                            BaiFenBi = (Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) * 100 / Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text)).ToString();
                            Cell.ElementAt(2).Descendants<Text>().First().Text = BaiFenBi;
                        }
                        else
                        {
                            Cell.ElementAt(2).Descendants<Text>().First().Text = 0.ToString();
                        }
                        break;
                    #endregion
                    #region ST或T波异常
                    case "ST或T波异常":
                        foreach (var item in resultAnalyse.Where(x => x.ProjectResult.Contains("非特异性ST") || x.ProjectResult.Contains("T波异常")))
                        {
                            count = Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) + item.Count;
                            Cell.ElementAt(1).Descendants<Text>().First().Text = count.ToString();
                        }
                        //检出人数/体检该项目的人数
                        if (Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text) != 0)
                        {
                            BaiFenBi = (Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) * 100 / Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text)).ToString();
                            Cell.ElementAt(2).Descendants<Text>().First().Text = BaiFenBi;
                        }
                        else
                        {
                            Cell.ElementAt(2).Descendants<Text>().First().Text = 0.ToString();
                        }
                        break;
                    #endregion
                    #region 胆囊结石/肝内胆管结石
                    case "胆囊结石/肝内胆管结石":
                        foreach (var item in resultAnalyse.Where(x => x.ProjectResult.Contains("胆囊结石") || x.ProjectResult.Contains("肝内胆管结石")))
                        {
                            count = Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) + item.Count;
                            Cell.ElementAt(1).Descendants<Text>().First().Text = count.ToString();
                        }
                        //检出人数/体检该项目的人数
                        if (Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text) != 0)
                        {
                            BaiFenBi = (Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) * 100 / Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text)).ToString();
                            Cell.ElementAt(2).Descendants<Text>().First().Text = BaiFenBi;
                        }
                        else
                        {
                            Cell.ElementAt(2).Descendants<Text>().First().Text = 0.ToString();
                        }
                        break;
                    #endregion
                    #region 肝(多发)囊肿
                    case "肝(多发)囊肿":
                        foreach (var item in resultAnalyse.Where(x => x.ProjectResult.Contains("肝囊肿") || x.ProjectResult.Contains("肝多发囊肿")))
                        {
                            count = Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) + item.Count;
                            Cell.ElementAt(1).Descendants<Text>().First().Text = count.ToString();
                        }
                        //检出人数/体检该项目的人数
                        if (Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text) != 0)
                        {
                            BaiFenBi = (Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) * 100 / Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text)).ToString();
                            Cell.ElementAt(2).Descendants<Text>().First().Text = BaiFenBi;
                        }
                        else
                        {
                            Cell.ElementAt(2).Descendants<Text>().First().Text = 0.ToString();
                        }
                        break;
                    #endregion
                    #region 肾（多发）结石
                    case "肾（多发）结石":
                        foreach (var item in resultAnalyse.Where(x => x.ProjectResult.Contains("右肾结石") || x.ProjectResult.Contains("肾多发结石")))
                        {
                            count = Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) + item.Count;
                            Cell.ElementAt(1).Descendants<Text>().First().Text = count.ToString();
                        }
                        //检出人数/体检该项目的人数
                        if (Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text) != 0)
                        {
                            BaiFenBi = (Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) * 100 / Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text)).ToString();
                            Cell.ElementAt(2).Descendants<Text>().First().Text = BaiFenBi;
                        }
                        else
                        {
                            Cell.ElementAt(2).Descendants<Text>().First().Text = 0.ToString();
                        }
                        break;
                    #endregion
                    #region 肾（多发）囊肿
                    case "肾（多发）囊肿":
                        foreach (var item in resultAnalyse.Where(x => x.ProjectResult.Contains("右肾囊肿") || x.ProjectResult.Contains("肾多发囊肿")))
                        {
                            count = Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) + item.Count;
                            Cell.ElementAt(1).Descendants<Text>().First().Text = count.ToString();
                        }
                        //检出人数/体检该项目的人数
                        if (Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text) != 0)
                        {
                            BaiFenBi = (Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) * 100 / Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text)).ToString();
                            Cell.ElementAt(2).Descendants<Text>().First().Text = BaiFenBi;
                        }
                        else
                        {
                            Cell.ElementAt(2).Descendants<Text>().First().Text = 0.ToString();
                        }
                        break;
                    #endregion
                    #region 单耳轻度传导性听力损失
                    case "单耳轻度传导性听力损失":
                        foreach (var item in resultAnalyse.Where(x => x.ProjectDetailsName.Contains("听力损失")))
                        {
                            count += item.Count;
                            Cell.ElementAt(1).Descendants<Text>().First().Text = count.ToString();
                        }
                        //检出人数/体检该项目的人数
                        if (Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text) != 0)
                        {
                            BaiFenBi = (Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) * 100 / Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text)).ToString();
                            Cell.ElementAt(2).Descendants<Text>().First().Text = BaiFenBi;
                        }
                        else
                        {
                            Cell.ElementAt(2).Descendants<Text>().First().Text = 0.ToString();
                        }
                        break;
                    #endregion
                    #region 房室传导阻滞
                    case "房室传导阻滞":
                        foreach (var item in resultAnalyse.Where(x => x.ProjectDetailsName.Contains("传导阻滞")))
                        {
                            count += item.Count;
                            Cell.ElementAt(1).Descendants<Text>().First().Text = count.ToString();
                        }
                        //检出人数/体检该项目的人数
                        if (Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text) != 0)
                        {
                            BaiFenBi = (Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) * 100 / Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text)).ToString();
                            Cell.ElementAt(2).Descendants<Text>().First().Text = BaiFenBi;
                        }
                        else
                        {
                            Cell.ElementAt(2).Descendants<Text>().First().Text = 0.ToString();
                        }
                        break;
                    #endregion
                    #region 其他
                    default:
                        foreach (var item in resultAnalyse)//设置第二个单元格
                        {
                            if (item.ProjectResult != null && item.ProjectResult.Contains(Cell.ElementAt(0).InnerText))
                            {
                                count = Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) + item.Count;
                                Cell.ElementAt(1).Descendants<Text>().First().Text = count.ToString();
                            }
                        }
                        //检出人数/体检该项目的人数
                        if (Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text) != 0)
                        {
                            BaiFenBi = (Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) * 100 / Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text)).ToString();
                            Cell.ElementAt(2).Descendants<Text>().First().Text = BaiFenBi;
                        }
                        else
                        {
                            Cell.ElementAt(2).Descendants<Text>().First().Text = 0.ToString();
                        }
                        break;
                        #endregion
                }
            }
            #endregion

        }

        /// <summary>
        /// 设置化学检查表格
        /// </summary>
        /// <param name="body"></param>
        /// <param name="resultAnalyse"></param>
        /// <param name="getProjectDetailsCount"></param>
        private void SetLEtable(Body body, IEnumerable<UserCheckOutPut> resultAnalyse, IEnumerable<UserCheckOutPut> getProjectDetailsCount)
        {
            //【1】获取表格所有行
            var table = body.Descendants<Table>().First();
            var RowList = table.Descendants<TableRow>().ToList();
            //【2】循环集合获取到第一个单元格文本进行判断
            string[] Str = { "检查项目", "血常规检查", "肝功能检查", "生化项目检查", "尿常规检查", "女性妇科检查", "白带常规检查" };
            #region 设置但四个单元格
            //项目详细编号集合(每一个编号对应一个单元格，除了行的InnerText包含在Str数组中)
            List<string> StrProjectDetailsId = new List<string>() {
            "061001、061017",//白细胞 白细胞偏高
            "061001、061017",//白细胞 白细胞偏低
            "061044",//血红蛋白（Hb）血红蛋白偏低
            "061045",//血小板 血小板偏低  
            "061045",//血小板 血小板偏低  
            "061057",// 中性粒细胞绝对值 中性粒细胞绝对值偏低
            "xxxxxx",//谷丙转氨酶 谷丙转氨酶偏高
            "xxxxxx",//谷草转氨酶 谷草转氨酶偏高
            "062021",//尿酸(UA) 尿酸偏高
            "068041",//血糖(餐后2小时) 血糖偏高
            "062025",//甘油三酯(TG) 甘油三酯偏高
            "062024",//胆固醇(CH) 胆固醇偏高
            "062027",//低密度脂蛋白 低密度脂蛋白偏高
            "062026",//高密度脂蛋白 高密度脂蛋白偏低
            "064007、064009",//甲状腺(血清甲状腺素(T4)+血清促甲状腺激素(TSH)) 甲状腺功能异常
            "061079" ,//尿蛋白质 尿蛋白（+）及以上   
            "010149",//尿血 尿隐血（+）及以上
            "061076",//尿白细胞 尿白细胞（+）及以上
            "xxxxxx",//尿酮体 尿酮体（+）及以上
            "270002",//阴道 细菌性阴道炎/念珠菌阴道炎
            "270003",//宫颈 宫颈纳氏囊肿
            "270003",//宫颈 非典型鳞状细胞
            "270002",//白带常规检查-白细胞（+++）
            "061003",//白带常规检查-念珠菌 
            };
            int index = 0;
            foreach (TableRow item in RowList)
            {
                if (Str.Contains(item.Descendants<TableCell>().ElementAt(0).InnerText)) continue;
                var Cell = item.Descendants<TableCell>().ToList();
                foreach (var item2 in getProjectDetailsCount)
                {
                    if (StrProjectDetailsId[index].Contains(item2.ProjectDetailsId))
                    {
                        Cell.ElementAt(3).Descendants<Text>().First().Text = (item2.Count).ToString();
                        break;// 设置完直接跳出当前循环 进行下一行的设置
                    }
                }
                index += 1;
            }
            #endregion

            #region 设置但二个单元格
            foreach (var item1 in RowList)
            {
                //判断第一个单元格的文本不在数组中
                if (Str.Contains(item1.Descendants<TableCell>().ElementAt(0).InnerText)) continue;
                string BaiFenBi = "";
                int count = 0;//总人数
                var Cell = item1.Descendants<TableCell>().ToList();
                switch (Cell[0].Descendants<Text>().First().Text)
                {
                    #region 尿蛋白（+）及以上
                    case "尿蛋白（+）及以上":
                        foreach (var item in resultAnalyse.Where(x => x.ProjectDetailsName.Contains("尿蛋白") && x.ProjectDetailsName.Contains("+")))
                        {
                            count = Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) + item.Count;
                            Cell.ElementAt(1).Descendants<Text>().First().Text = count.ToString();
                        }
                        //检出人数/体检该项目的人数
                        if (Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text) != 0)
                        {
                            BaiFenBi = (Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) * 100 / Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text)).ToString();
                            Cell.ElementAt(2).Descendants<Text>().First().Text = BaiFenBi;
                        }
                        else
                        {
                            Cell.ElementAt(2).Descendants<Text>().First().Text = 0.ToString();
                        }
                        break;
                    #endregion
                    #region 尿隐血（+）及以上
                    case "尿隐血（+）及以上":
                        foreach (var item in resultAnalyse.Where(x => x.ProjectResult.Contains("尿隐血") || x.ProjectResult.Contains("+")))
                        {
                            count = Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) + item.Count;
                            Cell.ElementAt(1).Descendants<Text>().First().Text = count.ToString();
                        }

                        //检出人数/体检该项目的人数
                        if (Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text) != 0)
                        {
                            BaiFenBi = (Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) * 100 / Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text)).ToString();
                            Cell.ElementAt(2).Descendants<Text>().First().Text = BaiFenBi;
                        }
                        else
                        {
                            Cell.ElementAt(2).Descendants<Text>().First().Text = 0.ToString();
                        }
                        break;
                    #endregion
                    #region 尿白细胞（+）及以上
                    case "尿白细胞（+）及以上":
                        foreach (var item in resultAnalyse.Where(x => x.ProjectResult.Contains("尿白细胞") && x.ProjectResult.Contains("+")))
                        {
                            count = Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) + item.Count;
                            Cell.ElementAt(1).Descendants<Text>().First().Text = count.ToString();
                        }

                        //检出人数/体检该项目的人数
                        if (Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text) != 0)
                        {
                            BaiFenBi = (Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) * 100 / Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text)).ToString();
                            Cell.ElementAt(2).Descendants<Text>().First().Text = BaiFenBi;
                        }
                        else
                        {
                            Cell.ElementAt(2).Descendants<Text>().First().Text = 0.ToString();
                        }
                        break;
                    #endregion
                    #region 尿酮体（+）及以上
                    case "尿酮体（+）及以上":
                        foreach (var item in resultAnalyse.Where(x => x.ProjectResult.Contains("尿酮体") && x.ProjectResult.Contains("+")))
                        {
                            count = Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) + item.Count;
                            Cell.ElementAt(1).Descendants<Text>().First().Text = count.ToString();
                        }

                        //检出人数/体检该项目的人数
                        if (Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text) != 0)
                        {
                            BaiFenBi = (Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) * 100 / Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text)).ToString();
                            Cell.ElementAt(2).Descendants<Text>().First().Text = BaiFenBi;
                        }
                        else
                        {
                            Cell.ElementAt(2).Descendants<Text>().First().Text = 0.ToString();
                        }
                        break;
                    #endregion
                    #region 细菌性阴道炎/念珠菌阴道炎
                    case "细菌性阴道炎/念珠菌阴道炎":
                        foreach (var item in resultAnalyse.Where(x => x.ProjectResult.Contains("细菌性阴道炎") || x.ProjectResult.Contains("念珠菌阴道炎")))
                        {
                            count = Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) + item.Count;
                            Cell.ElementAt(1).Descendants<Text>().First().Text = count.ToString();
                        }

                        //检出人数/体检该项目的人数
                        if (Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text) != 0)
                        {
                            BaiFenBi = (Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) * 100 / Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text)).ToString();
                            Cell.ElementAt(2).Descendants<Text>().First().Text = BaiFenBi;
                        }
                        else
                        {
                            Cell.ElementAt(2).Descendants<Text>().First().Text = 0.ToString();
                        }
                        break;
                    #endregion
                    #region 白细胞（+++）
                    case "白细胞（+++）":
                        foreach (var item in resultAnalyse.Where(x => x.ProjectResult.Contains("白细胞") || x.ProjectResult.Contains("+")))
                        {
                            count = Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) + item.Count;
                            Cell.ElementAt(1).Descendants<Text>().First().Text = count.ToString();
                        }

                        //检出人数/体检该项目的人数
                        if (Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text) != 0)
                        {
                            BaiFenBi = (Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) * 100 / Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text)).ToString();
                            Cell.ElementAt(2).Descendants<Text>().First().Text = BaiFenBi;
                        }
                        else
                        {
                            Cell.ElementAt(2).Descendants<Text>().First().Text = 0.ToString();
                        }
                        break;
                    #endregion
                    #region 其他
                    default:
                        foreach (var item in resultAnalyse)//设置第二个单元格
                        {
                            if (item.ProjectResult != null && item.ProjectResult.Contains(Cell.ElementAt(0).InnerText))
                            {
                                count = Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text) + item.Count;
                                Cell.ElementAt(1).Descendants<Text>().First().Text = count.ToString();
                            }
                        }
                        //检出人数/体检该项目的人数
                        if (Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text) != 0)
                        {
                            BaiFenBi = ((Convert.ToInt32(Cell.ElementAt(1).Descendants<Text>().First().Text)) / (Convert.ToInt32(Cell.ElementAt(3).Descendants<Text>().First().Text))).ToString();
                            Cell.ElementAt(2).Descendants<Text>().First().Text = BaiFenBi;
                        }
                        else
                        {
                            Cell.ElementAt(2).Descendants<Text>().First().Text = 0.ToString();
                        }
                        break;
                        #endregion
                }
            }
            #endregion

        }
        #endregion
    }
}
