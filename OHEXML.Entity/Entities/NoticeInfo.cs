using OHEXML.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OHEXML.Entity.Entities
{
    [Table("NoticeInfo")]
    public class NoticeInfo : BaseEntity
    {
        [Key]
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string NotificationType { get; set; }
        public DateTime CreateTime { get; set; }

        //public string ModuleUrl { get; set; }
    }
}
