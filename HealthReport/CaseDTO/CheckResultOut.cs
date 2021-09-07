using System;
using System.Collections.Generic;
using System.Text;

namespace HealthReport.CaseDTO
{
    /// <summary>
    /// 全部参检人员体检结果及结论
    /// </summary>
    public class CheckResultOut
    {
        /// <summary>
        /// 体检人姓名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }
        /// <summary>
        /// 危害因素
        /// </summary>
        public string HazardFactors { get; set; }
        /// <summary>
        /// 体检类别
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 首次体检结果
        /// </summary>
        public string CheckResult { get; set; }
        /// <summary>
        /// 复检结果
        /// </summary>
        public string ReviewResult { get; set; }
        /// <summary>
        /// 体检结论
        /// </summary>
        public string Conclusion { get; set; }
        /// <summary>
        /// 体检处理意见
        /// </summary>
        public string Opption { get; set; }
        /// <summary>
        /// 纯音测听结果
        /// </summary>
        public string PureToneAudiometryResults { get; set; }
        
    }

}
