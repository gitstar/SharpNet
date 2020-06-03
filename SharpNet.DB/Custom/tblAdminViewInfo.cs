using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharpNet.DB.Custom
{
    [Table("tblAdminViewInfo")]
    public class tblAdminViewInfo
    {
        [Key]
        [MaxLength(20)]
        [StringLength(20)]
        [Required]
        public string AdminID { get; set; } // nvarchar(20)

        [Key]
        [MaxLength(50)]
        [StringLength(50)]
        [Required]
        public string AdminName { get; set; } // nvarchar(50)

        [Key]
        [MaxLength(50)]
        [StringLength(50)]
        [Required]
        public string AdminNickName { get; set; } // nvarchar(50)

        [Required]
        [MaxLength(20)]
        [StringLength(20)]
        public string Pswd { get; set; }

        [MaxLength(20)]
        [StringLength(20)]
        [Required]
        public string AdminEmail { get; set; } // varchar(20)

        [MaxLength(20)]
        [StringLength(20)]
        [Required]
        public string AdminPhone { get; set; } // varchar(20)

        [Required]
        [MaxLength(10)]
        [StringLength(10)]
        public string AdminLevel { get; set; } // int

        [Required]
        public int AdminState { get; set; } // int

        [MaxLength(1)]
        [StringLength(1)]
        public string UseType { get; set; } // char(1)

        public DateTime? UpdateDate { get; set; } // smalldatetime

        [MaxLength(20)]
        [StringLength(20)]
        public string UpdateUser { get; set; } // nvarchar(20)
    }
}
