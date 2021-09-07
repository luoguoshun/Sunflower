using OHEXML.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OHEXML.Entity.Entities
{    
    [Table("MessageType")]
    public class MessageType : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public int TypeId { get; set; }
        /// <summary>
        /// 类型说明
        /// </summary>
        public string Explain { get; set; }
        /// <summary>
        /// 消息信息表集合（集合导航属性）
        /// </summary>

    }
}
