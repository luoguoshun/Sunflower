using Microsoft.AspNetCore.Mvc;
using OHEXML.Contracts.HealthReportModule;
using OHEXML.Controllers.Base;
using OHEXML.Infrastructure.Attributes;
using OHEXML.Server.CreatePDFModule;
using System.Threading.Tasks;
using static OHEXML.Common.EnumLIst.AppTypesEnum;

namespace OHEXML.InsertReport.Controllers
{
    public class CreatePDFController : BaseController
    {
        public readonly ICreatePDFServer _createPDF;
        public CreatePDFController(ICreatePDFServer createPDF)
        {
            _createPDF = createPDF;
        }

        /// <summary>
        /// 简洁版Word报告
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize(AppTypes.Doctor)]
        [ApiExplorerSettings(GroupName = "Doctor")]
        public async Task<IActionResult> CreateCompletePDF(CreatePdfDTO dto)
        {

            if (string.IsNullOrEmpty(dto.UnitID) || dto.CheckDate == null)
            {
                return JsonFailt("请输入正确的编号或日期！");
            }
            else
            {
                (bool, string, string) FileUrl = await _createPDF.CreatePDFAsync(dto.UnitID, dto.CheckDate.ToString());
                return JsonSuccess(FileUrl.Item2, FileUrl.Item3);

            }
        }
        /// <summary>
        /// 完整版Word报告
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        //[AppAuthorize(AppTypes.Doctor)]
        [ApiExplorerSettings(GroupName = "Doctor")]
        public async Task<IActionResult> CreateCompletePDF(Test dto)
        {
            if (string.IsNullOrEmpty(dto.UnitID) || dto.CheckDate == null)
            {
                return JsonFailt("请输入正确的编号或日期！");
            }
            (bool, string, string) FileUrl = await _createPDF.CreateCompletePDFAsync(dto.UnitID, dto.CheckDate.ToString());
            return JsonSuccess(FileUrl.Item2, FileUrl.Item3);

        }
    }
}

