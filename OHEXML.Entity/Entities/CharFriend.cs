using OHEXML.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OHEXML.Entity.Entities
{
    /// <summary>
    /// 好友表
    /// </summary>
     [Table("CharFriend")]
   public class CharFriend : BaseEntity
    {
        [Key]
        public string UserId { get; set; }
        [Key]
        public string FriendID { get; set; }
    }
}
