using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OHEXML.Server.CreatePDFModule
{
    public interface ICreatePDFServer
    {
        /// <summary>
        /// 简洁版Word
        /// </summary>
        /// <returns></returns>
        Task<(bool, string, string)> CreatePDFAsync(string UnitID, string CheckDate);
        /// <summary>
        /// 完整版Word
        /// </summary>
        /// <returns></returns>
        Task<(bool, string, string)> CreateCompletePDFAsync(string UnitID, string CheckDate);
        /// <summary>
        /// Word转PDF
        /// </summary>
        /// <param name="WordPath"></param>
        /// <returns></returns>
        (bool, string) WordToPDF(string UnitName, string WordPath);
    }
}
