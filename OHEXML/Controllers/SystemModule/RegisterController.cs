
using OHEXML.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using OHEXML.Contracts.UserModule;
using OHEXML.Server.UserModule;
using System.Threading.Tasks;

namespace OHEXML.Controllers
{
    public class RegisterController : BaseController
    {
        #region 构造函数
        private readonly IUserServer _userServer;
        public RegisterController(IUserServer userServer)
        {
            _userServer = userServer;
        }
        #endregion

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiExplorerSettings(GroupName = "Client")]
        public async Task<IActionResult> UserRegister([FromBody] UserDTO user)
        {
          var Result= await _userServer.AddUserInfoAsync(user);
          return Result.Item1 ? JsonSuccess(Result.Item2, null) : JsonFailt(Result.Item2);
        }
    }
}
