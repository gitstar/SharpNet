using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharpNet.DB.Table
{
    [Table("tblviewuserinfo")]
    public class tblViewuserinfo
    {
        [Key]
        [Required]
        public int userId { get; set; }
        public sbyte roleId { get; set; }
        public int createdBy { get; set; }
        public DateTime endDate { get; set; }
    }
}
