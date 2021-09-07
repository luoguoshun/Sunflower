using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OHEXML.Contracts.UserModule;
using OHEXML.Controllers.Base;
using System.Net.Http;
using System.Threading.Tasks;
using static OHEXML.Common.EnumLIst.AppTypesEnum;

namespace OHEXML.Controllers.SystemModule
{
    public class AccountController : BaseController
    {

        #region 构造函数
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        #endregion

        /// <summary>
        /// 获取到文档地址
        /// </summary>
        private string IdentityServerAddress => _configuration.GetSection("IdentityServer").GetValue<string>("Address");

        [HttpPost]
        [ApiExplorerSettings(GroupName = "Background")]
        public async Task<IActionResult> AdminAccount(UserDTO user)
        {
            //设置请求范围(范围在Sunflower客服端允许请求范围内)
            string Scope = $"{AppTypes.Client.ToString().ToLower()} {AppTypes.Doctor.ToString().ToLower()} {AppTypes.Background.ToString().ToLower()}";
            var tokenResponse = await new HttpClient().RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = IdentityServerAddress + "/connect/token",
                ClientId = "Sunflower",
                ClientSecret = "secret",
                Scope = Scope,
                UserName = user.Account,
                Password = user.Password
            });
            if (tokenResponse.IsError)
            {
                if (tokenResponse.ErrorDescription.Equals("invalid_username_or_password"))
                    return JsonFailt("用户名或密码错误，请重新输入");
                else
                    return JsonFailt($"[令牌响应错误]: {tokenResponse.Error}");
            }
            return JsonSuccess("登入成功！", tokenResponse.AccessToken);
        }

        [HttpPost]
        [ApiExplorerSettings(GroupName = "Doctor")]
        public async Task<IActionResult> DoctorAccount(UserDTO user)
        {
            //request assess token - 请求访问令牌
            var tokenResponse = await new HttpClient().RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = IdentityServerAddress + "/connect/token",
                ClientId = "Sunflower",
                ClientSecret = "secret",
                Scope = AppTypes.Doctor.ToString().ToLower(),
                UserName = user.Account,
                Password = user.Password
            });
            if (tokenResponse.IsError)
            {
                if (tokenResponse.ErrorDescription.Equals("invalid_username_or_password"))
                    return JsonFailt("用户名或密码错误，请重新输入");
                else
                    return JsonFailt($"[令牌响应错误]: {tokenResponse.Error}");
            }
            return JsonSuccess("登入成功！", tokenResponse.AccessToken);
        }

    }
}
