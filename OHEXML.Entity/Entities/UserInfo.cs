using OHEXML.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OHEXML.Entity.Entities
{
    /// <summary>
    /// 用户信息表
    /// </summary>
    [Table("UserInfo")]
   public class UserInfo : BaseEntity
    {
        [Key]
        [MaxLength(125)]
        public string UserID { get; set; }
        public string Name { get; set; }
        public string PassWord { get; set; }
        /// <summary>
        /// 角色集合
        /// </summary>
        public List<RoleInfo> Roles { get; set; }
    }
}
