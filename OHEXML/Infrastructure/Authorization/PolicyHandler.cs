using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OHEXML.Infrastructure.PolicyHelper
{
    /// <summary>
    /// 自定义权限控制类(通过继承 AuthorizationHandler 来实现我们的授权逻辑)
    /// </summary>
    public class PolicyHandler : AuthorizationHandler<PolicyRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PolicyRequirement requirement)
        {
            IEnumerable<Claim> claims = context.User.Claims.Where(x => x.Type== "scope");
            foreach (var type in requirement.AppTypes)
            {
                if (claims.Any(c => c.Value == type.ToString().ToLower()))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }
            return Task.CompletedTask;
        }
    }

}
