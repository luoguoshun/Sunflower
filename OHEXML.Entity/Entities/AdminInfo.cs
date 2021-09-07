using OHEXML.Common.EnumLIst;
using OHEXML.Entity.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OHEXML.Entity.Entities
{
    /// <summary>
    /// 管理员信息表
    /// </summary>
    [Table("AdminInfo")]
    public class AdminInfo : BaseEntity
    {
        [Key]
        public string AdminNo { get; set; }
        public string Name { get; set; }
        public SexEnum Sex;
        public string PassWord { get; set; }
        /// <summary>
        /// 角色集合
        /// </summary>
        public List<RoleInfo> Roles { get; set; }
    }
    
}
