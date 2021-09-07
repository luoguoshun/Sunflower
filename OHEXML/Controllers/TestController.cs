using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OHEXML.Contracts.UserModule;
using OHEXML.Controllers.Base;
using OHEXML.Infrastructure.Attributes;
using OHEXML.Server.UserModule;
using System.Threading.Tasks;
using static OHEXML.Common.EnumLIst.AppTypesEnum;

namespace OHEXML.Controllers
{
    public class TestController : BaseController
    {
        private readonly IUserServer _userServer;
        public TestController(IUserServer userServer)
        {
            _userServer = userServer;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AppAuthorize(AppTypes.Background)]
        public async Task<IActionResult> GetListAsync()
        {
            var data = await _userServer.GetListUserInfoAsync();
            return JsonSuccess("", data);
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AppAuthorize(AppTypes.Background)]
        public async Task<IActionResult> AddUserInfoAsync()
        {
            UserDTO user = new UserDTO()
            {
                UserID = "adminG",
                Name = "Lusa",
                Password = "123"
            };
            (bool, string) data = await _userServer.AddUserInfoAsync(user);

            return JsonSuccess(data.Item2.ToString(), "");

        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AppAuthorize(AppTypes.Doctor)]
        public async Task<IActionResult> RemoveUserInfoAsync()
        {
            (bool, string) data = await _userServer.RemoveUserInfoAsync("adminB");
            return data.Item1 ? JsonSuccess(data.Item2.ToString(), "") : JsonFailt(data.Item2.ToString());

        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AppAuthorize(AppTypes.Client)]
        public async Task<IActionResult> UpdateUserInfoAsync()
        {
            UserDTO Dto = new UserDTO()
            {
                UserID = "admin123456",
                Name = "HAHA",
                Password = "123"
            };
            (bool, string) data = await _userServer.UpdateUserInfoAsync(Dto);
            return data.Item1 ? JsonSuccess(data.Item2.ToString(), "") : JsonFailt(data.Item2.ToString());


        }
    }
}
