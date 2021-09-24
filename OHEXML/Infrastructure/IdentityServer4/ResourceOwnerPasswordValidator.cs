using IdentityModel;
using IdentityServer4.Validation;
using OHEXML.Common.EnumLIst;
using OHEXML.Repository.UserModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static IdentityModel.OidcConstants;

namespace OHEXML.Infrastructure.IdentityServer4
{
    /// <summary>
    /// 继承IResourceOwnerPassword验证器来实现自己的验证规则
    /// </summary>
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        #region 构造函数
        private readonly IUserRepository _UserRepository;
        private readonly IAdminRepository _adminRepository;
        public ResourceOwnerPasswordValidator(IUserRepository UserRepository, IAdminRepository adminRepository)
        {
            _UserRepository = UserRepository;
            _adminRepository = adminRepository;
        }
        #endregion
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            //判断授权范围是否包含三端
            if (Enum.TryParse<AppTypesEnum.AppTypes>(context.Request.Scopes.FirstOrDefault(), true, out var appType))
            {
                //根据Scopes判断登入接口
                User user = null;
                switch (appType)
                {
                    case AppTypesEnum.AppTypes.Doctor:
                        user = await new ClientUserHandler(_UserRepository).GetUserAsync(context.UserName, context.Password);
                        break;
                    case AppTypesEnum.AppTypes.Background:
                        user = await new AdminUserHandler(_adminRepository).GetUserAsync(context.UserName, context.Password);
                        break;
                }
                if (user != null)
                {
                    var claims = new List<Claim>
                        {
                            new Claim(JwtClaimTypes.Id, user.Id),
                            new Claim(JwtClaimTypes.Name, user.Name),
                        };
                    //claims.AddRange(user.Roles.Select(r => new Claim(JwtClaimTypes.Role, r)));
                    context.Result = new GrantValidationResult(context.UserName, AuthenticationMethods.Password, claims);
                }
            }
        }
    }

    /// <summary>
    /// 存储在用户信息实体
    /// </summary>
    internal class User
    {
        public string Id { get; set; }

        public string Name { get; set; }

        //public IEnumerable<string> Roles { get; set; }
    }

    /// <summary>
    /// 获取到用户信息
    /// </summary>
    internal class ClientUserHandler
    {
        private readonly IUserRepository _UserRepository;

        public ClientUserHandler(IUserRepository UserRepository)
        {
            _UserRepository = UserRepository;
        }

        public async Task<User> GetUserAsync(string account, string password)
        {
            var doctor = await _UserRepository.GetEntityAsync(a => a.UserID == account && a.PassWord == password);
            if (doctor is null)
            {
                return null;
            }
            return new User
            {
                Id = doctor.UserID,
                Name = doctor.Name,
            };
        }
    }
    /// <summary>
    /// 获取到管理员信息
    /// </summary>
    internal class AdminUserHandler
    {
        private readonly IAdminRepository _adminRepository;
        public AdminUserHandler(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<User> GetUserAsync(string account, string password)
        {
            var admin = await _adminRepository.GetEntityAsync(a => a.AdminNo == account && a.PassWord == password);
            if (admin is null)
            {
                return null;
            }
            return new User
            {
                Id = admin.AdminNo,
                Name = admin.Name,
            };
        }
    }
}
