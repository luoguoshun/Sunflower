using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OHEXML.Entity.Entities
{
    [Table("RoleInfo")]
    public class RoleInfo
    {
        [Key]
        public int RoleID { get; set; }
        public int AppType { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        [MaxLength(125)]
        public string RoleName { get; set; }
        /// <summary>
        /// 角色说明
        /// </summary>
        [MaxLength(125)]
        public string Explain { get; set; }

        /// <summary>
        /// 管理员集合（集合导航属性）
        /// </summary>
        public List<AdminInfo> admins { get; set; }
        /// <summary>
        /// 用户集合（集合导航属性）
        /// </summary>
        public List<UserInfo> users { get; set; }
    }
}
