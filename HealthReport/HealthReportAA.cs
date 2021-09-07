using System;
using System.Collections.Generic;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using System.Linq;
using HealthReportHelper.HealthReportHelper;
using HealthReport.CaseDTO;
using OHEXML.Common.OftenString;

using A = DocumentFormat.OpenXml.Drawing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using static OHEXML.Contracts.HealthReportModule.HealthReportDTO;

namespace HealthReport
{
    public class HealthReportAA : IDisposable
    {
        #region 全局变量 哈
        //当前程序集的相对路径(在构造函数初始化)
        private readonly string basePath;
        //设置页面word的DocProperties(文档属性)元素（uint则是不带符号的，表示范围是：2^32即0到4294967295）
        public uint docId;
        //分页页数统计
        public int PageCount;
        //获取空模板的主文档(空模板的地址(在构造函数初始化))
        private readonly WordprocessingDocument _document;
        protected MainDocumentPart NullMainPart => _document.MainDocumentPart;
        //获取空模板的身体部分（不包括页眉、页脚、图片(属于引用部件)）
        protected Body NullBody => NullMainPart.Document.Body;
        public DateTime TimeNow { get; set; }
        public string UnitName { get; set; }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public HealthReportAA(string UnitName, DateTime TimeNow)
        {
            // 使用path获取当前应用程序集的执行目录的上级的上级目录==》basePath = Path.GetFullPath("../..")
            //当前程序集的相对路径
            basePath = Environment.CurrentDirectory + @"\wwwroot\Templates";
            _document = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\Base.docx");
            this.TimeNow = TimeNow;
            this.UnitName = UnitName;

        }
        public HealthReportAA TestAddImageToBody(string ImgPath)
        {
            using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\1-Main.docx");

            MainDocumentPart mainPart = template.MainDocumentPart;
            Body Templatebody = template.MainDocumentPart.Document.Body;
            ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);//创建图片部件

            using (FileStream stream = new FileStream(ImgPath, FileMode.Open))
            {
                imagePart.FeedData(stream);//将数据馈送到部分流。零件的流将首先被截断。
            }
            AddImageToBody(Templatebody, mainPart.GetIdOfPart(imagePart));
            return AddPage(template);

        }

        /// <summary>
        /// 添加首页
        /// </summary>
        /// <param name="companyOutOut"></param>
        /// <param name="CheckDat"></param>
        /// <returns></returns>
        public HealthReportAA AddmainPage(CompanyOutPut companyOutOut, string CheckDat)
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
                        CheckDat.Replace(".","/"),
                        TimeNow.ToString("yyyy/MM/dd"),
                        "020-34063261",
                        "020-34063261",
                        "020-84456797",
                    };
                list.LastOrDefault();
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
        /// <param name="companyOutOut">体检单位信息表</param>
        /// <param name="AllCheckInfo">用户信息登记表</param>
        /// <returns></returns>
        public HealthReportAA AddExplainPage(CompanyOutPut companyOutOut, List<UserCheckInfoOutPut> AllCheckInfo)
        {
            using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\2-Explain.docx");
            var Templatebody = template.MainDocumentPart.Document.Body;
            //找到下标为20的段落的第二个运行文本
            var P20_2 = Templatebody.Descendants<Paragraph>().ElementAt(20).Descendants<Run>();

            List<string> list = new List<string>
                {
                     companyOutOut.CompanyName,
                     AllCheckInfo.Count().ToString(),//登记体检信息总人数
                };
            int i = 0;
            foreach (var item in P20_2)
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
        public HealthReportAA AddSummary(CompanyOutPut companyUnitOutOut, List<UserCheckInfoOutPut> AllCheckInfo, List<CheckResultOut> ResultSummary, string StrHazardousName)
        {
            using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\3-SummaryReport.docx");
            var Templatebody = template.MainDocumentPart.Document.Body;
            //段落集合
            var ParagraphList = Templatebody.Descendants<Paragraph>().ToList();
            #region 定义日期
            string[] Datestr = null;
            string[] datestr2 = null;
            if (AllCheckInfo.FirstOrDefault().CheckDate != null) //开始日期
            {
                Datestr = AllCheckInfo.FirstOrDefault().CheckDate.Split(".");
            }
            //第十四个段落 签字日期
            string[] datestr3 = (TimeNow.ToString("yyyy.MM.dd").Split("."));
            #endregion

            #region 统计病例
            int yisi = ResultSummary.Where(x => x.CheckResult == StriingDTO.Result.Result5).Count();
            int jinji = ResultSummary.Where(x => x.CheckResult == StriingDTO.Result.Result6).Count();
            #endregion

            #region 插入数据
            //所有数据集合
            List<string> list = new List<string>
                {
                     companyUnitOutOut.CompanyName,
                     companyUnitOutOut.CompanyName,
                     Datestr[0] ?? "--",
                     Datestr[1] ?? "--",
                     Datestr[2] ?? "--",
                     Datestr[1] ?? "--",
                     Datestr[2] ?? "--",
                     AllCheckInfo.Count().ToString(),
                     datestr2==null?"--":"--",
                     datestr2==null?"--":"--",
                     datestr2==null?"--":"--",
                     companyUnitOutOut.CompanyName,
                     StrHazardousName,
                     AllCheckInfo.Where(x=>x.TypeName.Contains("在岗期间")).Count().ToString(),//在岗体检应检人数
                     ResultSummary.Where(x=>x.TypeName.Contains("在岗期间")).Count().ToString(),//实际检查在岗人数
                     AllCheckInfo.Count().ToString(),
                     jinji==0?$"本次检查未发现有职业禁忌症；":$"本次检查发现有{yisi}例职业禁忌症，具体见附件4;",
                     yisi==0?$"本次检查未发现有疑似职业病；":$"本次检查发现有{jinji}例疑似职业病，具体见附件4;",
                     companyUnitOutOut.CompanyName,
                     datestr3[0],
                     datestr3[1],
                     datestr3[2],
                };
            int k = 0;
            //循环每个含有*占位符
            foreach (var item in ParagraphList)
            {
                if (item.InnerText.Contains("*"))
                {
                    var RunList = item.Descendants<Run>().ToList();
                    for (int j = 0; j < RunList.Count; j++)
                    {
                        if (RunList[j].InnerText.Contains("*"))
                        {
                            RunList[j].Descendants<Text>().First().Text = list[k];
                            k++;
                        }
                    }
                }
            }
            #endregion
            return AddPage(template);
        }
        /// <summary>
        /// 添加健康卡
        /// </summary>
        /// <param name="companyOutPut"></param>
        /// <returns></returns>
        public HealthReportAA AddHealthCard(CompanyOutPut companyOutPut, List<UserHazardousCountOut> HazardousCount)
        {
            using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\4-HealthCard.docx");
            var Templatebody = template.MainDocumentPart.Document.Body;

            FirstTable();
            SecondTable();
            ThirdTable();
            return AddPage(template);

            void FirstTable()
            {
                var table = Templatebody.Descendants<Table>().ElementAt(0);
                var RowList = table.Descendants<TableRow>().ToList();
                List<string> list = new List<string>
                    {
                    "广东省职业病防治院",
                    "45585827-3",
                    companyOutPut.CompanyName.ToString(),
                    companyOutPut.CompanyCode ?? "请填写",
                    };
                RowList[0].Descendants<TableCell>().ElementAt(1).Descendants<Text>().First().Text = list[0];
                RowList[0].Descendants<TableCell>().ElementAt(3).Descendants<Text>().First().Text = list[1];
                RowList[1].Descendants<TableCell>().ElementAt(1).Descendants<Text>().First().Text = list[2];
                RowList[1].Descendants<TableCell>().ElementAt(3).Descendants<Text>().First().Text = list[3];
            }

            void SecondTable()
            {
                var table = Templatebody.Descendants<Table>().ElementAt(1);
                var RowList = table.Descendants<TableRow>().ToList();

                List<string> list = new List<string>
                 {
                    companyOutPut.Address ?? "请填写",
                    companyOutPut.CompanyCode ?? "请填写",
                    companyOutPut.UserName ?? "请填写",
                    companyOutPut.Telephone ?? "请填写",
                    companyOutPut.EconomicType ?? "请填写",
                    companyOutPut.Industry ?? "请填写",
                 };
                for (int i = 0; i < RowList.Count() - 4; i++)
                {
                    RowList[i].Descendants<TableCell>().ElementAt(1).Descendants<Text>().First().Text = list[i];
                }
                //设置表格最后一行(判断公司人数规模进行选择模板)
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
                #region 设置段落文本
                var plist = Templatebody.Descendants<Paragraph>().ToList();
                var p26 = plist.ElementAt(26).Descendants<Run>();

                List<string> list = new List<string>
                    {
                        companyOutPut.SumNumber ==0 ? "0" : companyOutPut.SumNumber.ToString(),
                        companyOutPut.WomenWrkers ==0 ? "0" :companyOutPut.WomenWrkers.ToString(),
                        companyOutPut.ProWorkers == 0 ? "0" : companyOutPut.ProWorkers.ToString(),
                        companyOutPut.WomenProWorkers == 0 ? "0" : companyOutPut.WomenProWorkers.ToString(),
                        companyOutPut.HarmfulWork == 0 ? "0" : companyOutPut.HarmfulWork.ToString(),
                        companyOutPut.HarmfulWomenProWorkers == 0 ? "0" : companyOutPut.HarmfulWomenProWorkers.ToString()
                    };
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
                #endregion

                #region 处理数据 设置表格
                var table = Templatebody.Descendants<Table>().ElementAt(2);
                var templateRow = table.Descendants<TableRow>().ElementAt(1);
                templateRow.Remove();

                foreach (var item in HazardousCount)
                {
                    var CloneRow = templateRow.CloneNode(true);
                    var CellList = templateRow.Descendants<TableCell>().ToList();
                    List<string> list2 = new List<string>
                        {
                            item.HazardousName.ToString(),
                            item.ContactCount.ToString(),
                            item.NeedCount.ToString(),
                            item.ActualCount.ToString(),
                            item.YisiCount.ToString(),
                            item.JinjiCount.ToString(),
                            item.JinjiCount.ToString(),//调离人数==职业禁忌症人数
                        };
                    for (int i = 0; i < CellList.Count; i++)
                    {
                        (CloneRow.Descendants<TableCell>().ToList())[i].Descendants<Text>().First().Text = list2[i];
                    }
                    table.AppendChild(CloneRow);
                }
                #endregion
            }
        }
        /// <summary>
        /// 添加体检结果页面1
        /// </summary>
        /// <param name="FirstCheckOuts">初检</param>
        /// <returns></returns>
        public HealthReportAA AddCheckResult1(List<CheckResultOut> ResultSummary)
        {
            using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\5-CheckResult1.docx");
            var Templatebody = template.MainDocumentPart.Document.Body;
            FirstTable();
            SecondAddThirdTable();
            return AddPage(template);
            void FirstTable()
            {
                //获取已检查名单的总数
                Templatebody.Descendants<Paragraph>().ElementAt(1).Descendants<Run>().ElementAt(1).Descendants<Text>().First().Text = ResultSummary.Count().ToString();

                var table = Templatebody.Descendants<Table>().ElementAt(0);
                var RowList = table.Descendants<TableRow>().ToList();

                //获取各个检查结果的总数
                List<int> ResultNumber = new List<int>();
                int normal = 0, review = 0, probably = 0, taboo = 0, other = 0;
                foreach (var item in ResultSummary)
                {
                    if (item.CheckResult == "复查")
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
                    RowList[p].Descendants<TableCell>().ElementAt(2).Descendants<Text>().First().Text = (((double)ResultNumber[p - 1] / (double)ResultSummary.Count()) * 100).ToString("0.00") + "%";

                }
            }
            void SecondAddThirdTable()
            {
                //【1】把模板保存下
                var table2 = Templatebody.Descendants<Table>().ElementAt(1);
                var Rowtemplate2 = table2.Descendants<TableRow>().ElementAt(1);

                //获取第三个表格
                var table3 = Templatebody.Descendants<Table>().ElementAt(2);
                var Rowtemplate3 = table3.Descendants<TableRow>().ElementAt(1);

                Rowtemplate2.Remove();
                Rowtemplate3.Remove();
                foreach (var item in ResultSummary)
                {
                    if (item.CheckResult == "疑似职业病")
                    {
                        //复制保存的模板
                        var row = Rowtemplate2.CloneNode(true);
                        var CellCount = row.Descendants<TableCell>().ToList();
                        //动态添加数据
                        List<string> list = new List<string>
                            {
                                item.UserName.ToString(),
                                item.Sex.ToString(),
                                item.Age.ToString(),
                                item.HazardFactors.ToString(),
                                item.CheckResult,
                                item.ReviewResult,
                                item.Conclusion.ToString(),
                                item.Opption.ToString()
                            };
                        for (int i = 0; i < CellCount.Count(); i++)
                        {
                            CellCount[i].Descendants<Text>().First().Text = list[i];
                        }
                        table2.Append(row);
                    }
                    else if (item.CheckResult == "职业禁忌证")
                    {
                        //复制保存的模板
                        var row = Rowtemplate3.CloneNode(true);
                        var CellCount = row.Descendants<TableCell>().ToList();
                        //动态添加数据
                        List<string> list = new List<string>
                            {
                                item.UserName.ToString(),
                                item.Sex.ToString(),
                                item.Age.ToString(),
                                item.HazardFactors.ToString(),
                                item.Conclusion.ToString(),
                                item.CheckResult,
                                item.Opption.ToString()
                            };
                        for (int i = 0; i < CellCount.Count(); i++)
                        {
                            CellCount[i].Descendants<Text>().First().Text = list[i];
                        }
                        table3.Append(row);
                    }
                };
            }
        }
        /// <summary>
        /// 添加复查人员结果页面
        /// </summary>
        /// <param name="ReviewCheckOuts">复检</param>
        /// <returns></returns>
        public HealthReportAA AddCheckResult2(List<CheckResultOut> ResultSummary)
        {
            using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\6-CheckResult2.docx");
            var Templatebody = template.MainDocumentPart.Document.Body;
            FirstTable();
            return AddPage(template);
            void FirstTable()
            {
                //设置表格1头部文本
                var plist = Templatebody.Descendants<Paragraph>().ToList();
                var p1 = plist.ElementAt(0).Descendants<Run>().ElementAt(1).Descendants<Text>().First().Text = ResultSummary.Where(x => x.ReviewResult != "暂无数据").Count().ToString();

                //【2】把模板保存下
                var table = Templatebody.Descendants<Table>().ElementAt(0);
                var Rowtemplate = table.Descendants<TableRow>().ElementAt(1);
                Rowtemplate.Remove();
                //【3】赋值
                foreach (var item in ResultSummary.Where(x => x.ReviewResult != "暂无数据"))
                {
                    //复制保存的模板
                    var row = Rowtemplate.CloneNode(true);
                    var CellCount = row.Descendants<TableCell>().ToList();
                    //动态添加数据
                    List<string> list = new List<string>
                        {
                            item.UserName.ToString(),
                            item.Sex.ToString(),
                            item.Age.ToString(),
                            item.HazardFactors.ToString(),
                            item.PureToneAudiometryResults.ToString(),
                            item.ReviewResult.ToString(),
                            item.Conclusion.ToString(),
                            item.Opption.ToString()
                        };
                    for (int i = 0; i < CellCount.Count(); i++)
                    {
                        CellCount[i].Descendants<Text>().First().Text = list[i];
                    }
                    table.Append(row);
                };

            }
        }
        /// <summary>
        /// 添加所有检查结果页面
        /// </summary>
        /// <param name="ResultSummary"></param>
        /// <returns></returns>
        public HealthReportAA AddCheckResult3(List<CheckResultOut> ResultSummary)
        {
            using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\7-CheckResult3.docx");
            var Templatebody = template.MainDocumentPart.Document.Body;
            //【1】把用户的信息放入表格
            var table = Templatebody.Descendants<Table>().ElementAt(0);
            var Rowtemplate = table.Descendants<TableRow>().ElementAt(1);
            Rowtemplate.Remove();
            //【2】处理数据

            var Firstp = Templatebody.Descendants<Paragraph>().Where(x => x.InnerText.Contains("*")).FirstOrDefault();
            Firstp.Descendants<Run>().Where(x => x.InnerText.Contains("*")).FirstOrDefault().Descendants<Text>().First().Text = ResultSummary.Count().ToString();
            //【3】每一次循环给 List<string> list 进行赋值
            int sort = 0;
            foreach (var item in ResultSummary)
            {
                //每添加完一行则序号加一   
                sort++;
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
                }
                //复制保存的模板
                var row = Rowtemplate.CloneNode(true);
                var TableCellList = row.Descendants<TableCell>().ToList();

                for (int i = 0; i < TableCellList.Count(); i++)
                {
                    TableCellList[i].Descendants<Text>().First().Text = list[i];
                }
                table.Append(row);
            }
            return AddPage(template);
        }
        /// <summary>
        /// 添加资质证书
        /// </summary>
        /// <returns></returns>
        public HealthReportAA AddRredentials()
        {
            using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\9-Rredentials.docx");
            return AddPage(template);
        }
        /// <summary>
        /// 添加补充解释页面
        /// </summary>
        /// <returns></returns>
        public HealthReportAA AddExplanation1()
        {
            using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\10-Explanation1.docx");
            return AddPage(template);
        }
        /// <summary>
        /// 添加名词解释页面
        /// </summary>
        /// <returns></returns>
        public HealthReportAA AddExplanation2()
        {using var template = WordprocessingDocument.CreateFromTemplate($@"{basePath}\HealthReportTemplates\11-Explanation2.docx");
            return AddPage(template);
        }
        /// <summary>
        /// 分页方法
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public HealthReportAA AddPage(WordprocessingDocument template)
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
                                    {TimeNow:yyyy}年职业健康检查总结报告                                              共{PageCount}页";
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
                var HeaderParts = NullMainPart.HeaderParts.ToList();
                //获取最后一个页面的头部
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
        /// 生成PDF
        /// </summary>
        /// <param name="path">PDF文件保存的位置</param>
        public void BuildPDF(string PDFPath)
        {
            string UnitName = this.UnitName;
            var WordPath = Path.Combine($@"{basePath}\HealthReportCaches\", $"{UnitName}{DateTime.Now:yyyyymmdd}.docx");
            //把可编辑的空模板（已完成）保存到savePath
            //_document是全局变量，在以上一系列操作中，已经把编辑好的==》可编辑的模板添加到==》空白模板上
            var newDocument = _document.SaveAs(WordPath);
            _document.Close();
            _document.Dispose();
            newDocument.Close();
            newDocument.Dispose();
            WordHelper.WordToPDF(WordPath, PDFPath);
        }
        /// <summary>
        /// 保存word
        /// </summary>
        /// <param name="SavePath"></param>
        public void SaveWord(string SaveWordPath)
        {
            //_document是全局变量，在以上一系列操作中，已经把编辑好的==》可编辑的模板添加到==》空白模板上
            var newDocument = _document.SaveAs(SaveWordPath);
            _document.Close();
            _document.Dispose();
            newDocument.Close();
            newDocument.Dispose();
        }

        #region 辅助方法
        public void Dispose() => _document.Dispose();
        /// <summary>
        /// 创建头实例并添加其子实例
        /// </summary>
        /// <param name="HeaderText">头部文本</param>
        /// <returns></returns>
        private  Header GeneratePageHeaderPart(string HeaderText)
        {
            //将位置设为中心
            PositionalTab pTab = new PositionalTab()
            {
                Alignment = AbsolutePositionTabAlignmentValues.Left,
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
        /// 添加图片
        /// </summary>
        /// <param name="wordDoc"></param>
        /// <param name="relationshipId"></param>
        private void AddImageToBody(Body body, string relationshipId)
        {
            // 定义图像的引用。
            var element =
                new Drawing(
                    new Inline(
            #region Extent(长度)
                        new Extent() { Cx = 990000L, Cy = 792000L },
            #endregion

            #region EffectExtent(距离边缘范围)
                        new EffectExtent()
                        {
                            LeftEdge = 1L,
                            TopEdge = 2L,
                            RightEdge = 3L,
                            BottomEdge = 4L
                        },
            #endregion

            #region DocProperties(文档属性)
                       new DocProperties() { Id = (UInt32Value)1U, Name = "Picture 1" },
            #endregion

            #region NonVisualGraphicFrameDrawingProperties(非可视图形框架绘图属性)
                       new NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks() { NoChangeAspect = true }),
            #endregion

            #region Graphic(图形)
                       new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties() { Id = (UInt32Value)0U, Name = "New Bitmap Image.jpg" },
                                         new PIC.NonVisualPictureDrawingProperties()
                                         ),

                                     new PIC.BlipFill(
                                             new A.Blip(
                                                 new A.BlipExtensionList(
                                                     new A.BlipExtension() { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" }))
                                             {
                                                 Embed = relationshipId,
                                                 CompressionState =
                                                 A.BlipCompressionValues.Print
                                             }, new A.Stretch(new A.FillRectangle())
                                         ),

                                      new PIC.ShapeProperties(
                                           new A.Transform2D(new A.Offset() { X = 0L, Y = 0L }, new A.Extents() { Cx = 990000L, Cy = 792000L }),
                                           new A.PresetGeometry(new A.AdjustValueList())
                                           {
                                               Preset = A.ShapeTypeValues.Rectangle
                                           })
                                       )
                             )
                             { Uri = "https://schemas.openxmlformats.org/drawingml/2006/picture" })/*Graphic()*/
            #endregion 
                    ) /*new Inline()*/
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     }); /*new Drawing()*/

            // 将图像部件添加到运行文本中末尾            
           body.AppendChild(new Paragraph(new Run(element)));
        }
        #endregion

    }
}
