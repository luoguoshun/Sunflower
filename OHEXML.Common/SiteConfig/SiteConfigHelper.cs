using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace OHEXML.Common.SiteConfig
{
    /// <summary>
    /// 配置信息读取模型
    /// </summary>
    public class SiteConfigHelper
    {
        /// <summary>
        /// 父节点下的子节点
        /// </summary>
        /// <param name="FatherSection"></param>
        /// <param name="childection"></param>
        /// <returns></returns>
        public static string GetSectioValue(string FatherSection, string childection)
        {
            //添加 json 文件路径
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            //创建配置根对象
            var configurationRoot = builder.Build();
            //获取节点数据
            string SectioValue = configurationRoot.GetSection(FatherSection).GetSection(childection).Value;
            return SectioValue;
        }
        /// <summary>
        /// 单一节点
        /// </summary>
        /// <param name="singleSection"></param>
        /// <returns></returns>
        public static string GetSectioValue(string singleSection)
        {
            //添加 json 文件路径
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            //创建配置根对象
            var configurationRoot = builder.Build();
            //获取节点数据
            string SectioValue = configurationRoot.GetSection(singleSection).Value;
            return SectioValue;
        }
    }
}
