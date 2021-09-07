using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Logging;

namespace OHEXML.Common.OpenXMLHelper
{
    public class SpreadsheetHelper
    {
        private ILogger<SpreadsheetHelper> _logger;
        public SpreadsheetHelper(ILogger<SpreadsheetHelper> logger)
        {
            _logger = logger;
        }
        public SpreadsheetHelper()
        {
        }
        /// <summary>
        /// 从文本流中打开
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <returns></returns>
        public (List<T>, string) GetListFromExcelByStream<T>(Stream file) where T : new()
        {
            using SpreadsheetDocument document = SpreadsheetDocument.Open(file, false);
            return GetList<T>(document);
        }
        /// <summary>
        /// 根据文件路径打开
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public (List<T>, string) GetListFromExcelByPath<T>(string filePath) where T : new()
        {
            using SpreadsheetDocument document = SpreadsheetDocument.CreateFromTemplate(filePath);
            return GetList<T>(document);
        }
        public (List<T>, string) GetList<T>(SpreadsheetDocument document) where T : new()
        {
            try
            {
                List<T> data = new List<T>();
                Sheet sheet = document.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().FirstOrDefault();
                if (sheet == null)
                {
                    return (null, "找不到文件");
                }
                WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheet.Id.Value);
                Worksheet worksheet = worksheetPart.Worksheet;
                //获取所有行
                List<Row> rows = worksheet.Descendants<SheetData>().First().Descendants<Row>().ToList();
                for (int i = 1; i < rows.Count; i++)
                {
                    int cellIndex = 0;
                    T baseC = new T();
                    PropertyInfo[] properties = baseC.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (Cell cell in rows[i].Take(properties.Length))
                    {
                        if (cellIndex+1 > properties.Length) break;
                        string text = string.Empty;
                        //单元格不为空值
                        if (cell.CellValue != null)
                        {
                            // 判断单元格的类型是否为共享字符串(对于数字和日期类型， DataType 属性的值为 null)
                            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                            {
                                SharedStringTablePart shareStringTablePart = document.WorkbookPart.SharedStringTablePart;
                                if (shareStringTablePart == null)
                                {
                                    return (null, "找不到共享字符串");
                                }
                                int shareStringId = Convert.ToInt32(cell.CellValue.Text);
                                SharedStringItem item = shareStringTablePart.SharedStringTable
                                                        .Elements<SharedStringItem>()
                                                        .ElementAt(shareStringId);
                                text = item.InnerText;
                            }
                            else
                            {
                                text = cell.CellValue.Text;
                            }
                        }
                        else
                        {
                            Type propertyType = properties[cellIndex].PropertyType;
                            switch (propertyType.Name)
                            {
                                case "String":
                                    text = "暂无信息";
                                    break;
                                case "Int32":
                                    text = "0";
                                    break;
                                case "DateTime":
                                    text = "2001/01/01 00:00:00";
                                    break;
                            }
                        }
                        object value = Convert.ChangeType(text, properties[cellIndex].PropertyType);
                        properties[cellIndex].SetValue(baseC, value, null);
                        cellIndex++;
                    }
                    data.Add(baseC);
                }
                return (data, "ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return (null, ex.Message);
            }
        }
    }
}
