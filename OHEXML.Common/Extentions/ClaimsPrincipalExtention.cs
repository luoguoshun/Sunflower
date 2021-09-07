using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace OHEXML.Common.Extentions
{
    public static class ClaimsPrincipalExtention
    {
        public static T GetClaimValue<T>(this ClaimsPrincipal claimsPrincipal, string type)
        {
            var claim = claimsPrincipal.FindFirst(type);
            if (claim == null) return default;
            return (T)Convert.ChangeType(claim.Value, typeof(T));
        }
        /// <summary>
        /// 获取具有指定声明类型的所有声明
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="claimsPrincipal"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<T> GetAllClaimValue<T>(this ClaimsPrincipal claimsPrincipal, string type)
        {
            var claims = claimsPrincipal.FindAll(type);
            return claims.Select(claim => (T)Convert.ChangeType(claim.Value, typeof(T))).ToList();
        }
        /// <summary>
        /// 获取用户指定声明
        /// </summary>
        /// <param name="claimsPrincipal"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetClaimValue(this ClaimsPrincipal claimsPrincipal, string type)
        {
           return claimsPrincipal.FindFirst(type).Value ?? null;
        }
    }
}
