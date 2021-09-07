using OHEXML.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OHEXML.Entity.Entities
{
    /// <summary>
    /// 聊天信息记录表
    /// </summary>
    [Table("MessageInfo")]
    public class MessageInfo : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        [StringLength(125)]
        public string SenderId { get; set; }
        [StringLength(125)]
        public string ReceiverId { get; set; }
        [MaxLength(500)]
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 信息类型外键 
        /// </summary>
        ///[Column("MessageTypeId")]
        [Column("MessageTypeId")]
        public int MessageTypeId { get; set; }
        /// <summary>
        /// 日志记录外键
        /// </summary>
        [Column("logId")]
        public int logId { get; set; }
        /// <summary>
        /// 消息日志（引用导航属性） 
        /// </summary>
        public LogInfo log { get; set; }

        #region 备注：属性满足以下要求则配置为外键
        // 如果依赖实体包含一个名称与以下模式之一匹配的属性，则它将配置为外键：
        //<导航属性名称><主键属性名称>
        //<导航属性名称>Id
        //<主体实体名称><主键属性名称>
        //<主体实体名称>Id
        #endregion

    }
}
