using OHEXML.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OHEXML.Entity.Entities
{
    [Table("WordPathInfo")]
    public partial class WordPathInfo : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string UnitName { get; set; }
        public string WordName { get; set; }
        public string BasePath { get; set; }           
        public string SaveTime { get; set; }
        /// <summary>
        /// 引用导航属性
        /// </summary>
        //public LogInfo Log { get; set; }
    }
}
