using System;
using System.Collections.Generic;
using System.Text;

namespace HealthReport.CaseDTO
{
   public class UserHazardousCountOut 
    {

        /// <summary>
        /// 危害因素名称
        /// </summary>
        public string HazardousName { get; set; }
        /// <summary>
        /// 检查结果
        /// </summary>
        public string CheckResult { get; set; }
        /// <summary>
        /// 接触人数统计
        /// </summary>
        public int ContactCount { get; set; }
        /// <summary>
        /// 应该检查人数
        /// </summary>
        public int NeedCount { get; set; }
        /// <summary>
        /// 实际检查人数
        /// </summary>
        public int ActualCount { get; set; }
        /// <summary>
        /// 疑似职业病人数统计
        /// </summary>
        public int YisiCount { get; set; }
        /// <summary>
        /// 职业禁忌症人数统计
        /// </summary>
        public int JinjiCount { get; set; }
        /// <summary>
        /// 总人数
        /// </summary>
        public int Count { get; set; }
    }
}
