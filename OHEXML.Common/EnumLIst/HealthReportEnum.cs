using System.Collections.Generic;

namespace OHEXML.Common.EnumLIst
{
    public static class  HealthReportEnum
    {
        /// <summary>
        /// 体检项目
        /// </summary>
        public enum Project
        {
            一般检查 = 1,
            内科 = 2,
            外科 = 3,
            眼科 = 4,
            耳鼻喉科 = 5,
            口腔科 = 6,
            检验科 = 7,
            金域检验 = 8,
            B超 = 9,
            彩超 = 10,
            心电图 = 11,
            放射科 = 12,
            CT = 13,
            MR = 14,
            肺功能室检查 = 15,
            呼气试验 = 16,
            妇科 = 17,
            五官科 = 18,
            其他 = 19,
            毒化实验室 = 20,
            皮肤科 = 21,
            DR = 22,
            神经肌电图 = 23,
        };
        /// <summary>
        /// 体检套餐
        /// </summary>
        public enum ChecksetMeal
        {
            团队套餐 = 0,
            私人套餐 = 1,
            豪华套餐 = 2
        }
        /// <summary>
        /// 体检类别
        /// </summary>
        public enum CheckType
        {
            上岗前 = 0,
            在岗期间 = 1,
            离岗 = 2,
            申请职业病 = 3,
            其他 = 4,
            孕离 = 5,
            应急 = 6,

        }
        /// <summary>
        /// 用户性别
        /// </summary>
        public enum UserSex
        {

            男 = 1,
            女 = 2,
            未知 = 3

        }
        /// <summary>
        /// 常见结果枚举
        /// </summary>
        public enum Result
        {
            目前未见异常 = 1,
            所检项目大致正常 = 2,
            其他疾病或异常 = 3,
            复查 = 4,
            疑似职业病 = 5,
            职业禁忌证 = 6

        }
    }
}
