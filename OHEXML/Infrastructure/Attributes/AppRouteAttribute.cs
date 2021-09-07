using Microsoft.AspNetCore.Mvc;
using static OHEXML.Common.EnumLIst.AppTypesEnum;

namespace OHEXML.Infrastructure.Attributes
{
    public class AppRouteAttribute : RouteAttribute
    {
        public AppRouteAttribute(AppTypes appType) : base($"/api/{appType}/[controller]/[action]")
        {
            AppType = appType.ToString();
        }
        public string AppType { get; }
    }
}
