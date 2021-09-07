using OHEXML.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OHEXML.Entity.Entities
{
    [Table("LogInfo")]
    public class LogInfo : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string ErroeMessage { get; set; }
        public DateTime ErrorTime { get; set; }
        public int Times { get; set; }
    }
}
