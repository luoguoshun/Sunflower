using Microsoft.Office.Interop.Word;
using System;

namespace HealthReportHelper.HealthReportHelper
{
    public class WordHelper
    {

        /// <summary>
        /// Word==》pdf
        /// </summary>
        /// <param name="WordPath">可WOrd文件</param>
        /// <param name="PDFPath">pdf的文件保存路径</param>
        public static string WordToPDF(string WordPath, string PDFPath)
        {
            Application application = new Application { Visible = false };
            try
            {
                var doc = application.Documents.Open(WordPath, ReadOnly: true);
                doc.ExportAsFixedFormat(PDFPath, WdExportFormat.wdExportFormatPDF);
                return "OK";
            }
            catch(Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                application.Quit();
            }
        }

    }
}
