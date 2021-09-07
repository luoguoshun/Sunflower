using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OHEXML.Common.Extentions;
using OHEXML.Contracts.UserModule;
using OHEXML.Controllers.Base;
using OHEXML.Server.UserModule;
using System.Threading.Tasks;

namespace OHEXML.Controllers.UserModule
{
    public class UserController : BaseController
    {
        #region 构造函数
        private readonly IUserServer _userServer;
        public UserController(IUserServer userServer)
        {
            _userServer = userServer;
        }
        #endregion

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="DTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiExplorerSettings(GroupName = "Background")]
        public async Task<IActionResult> UserReset(ResetPasswordDTO DTO)
        {
            if (DTO.OldPassword == null || DTO.NewPassword == null || DTO.RePassword == null)
            {
                return JsonFailt("请输入完整信息！");
            }
            string clientNo = HttpContext.User.GetClaimValue("Id");
            (bool, string) Result = await _userServer.UserResetPassWord(clientNo, DTO.OldPassword, DTO.NewPassword, DTO.RePassword);
            return Result.Item1 ? JsonSuccess(Result.Item2, null) : JsonFailt(Result.Item2);
        }
    }
}
