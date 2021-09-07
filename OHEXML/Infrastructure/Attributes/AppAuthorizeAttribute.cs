using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Routing;
using static OHEXML.Common.EnumLIst.AppTypesEnum;

namespace OHEXML.Infrastructure.Attributes
{
    public class AppAuthorizeAttribute : AuthorizeAttribute, IApiDescriptionGroupNameProvider, IRouteTemplateProvider
    {
        public AppAuthorizeAttribute(AppTypes appType)
        {
            AppType = appType.ToString();
            Policy = AppType;
        }
        public string AppType { get; }//根据登入接口获取到AppType
        public string GroupName => AppType;//分组名称
        public string Template => $"/api/{AppType}/[controller]/[action]";//自定义路由
        public int? Order => null;
        public string Name => null;        
    }
}
