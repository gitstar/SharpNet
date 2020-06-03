using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharpNet.DB.Table
{
    [Table("tbl_Notice")]
    public class tblNotice
    {
        [Key]
        [Required]
        public int? ID { get; set; }

        [Key]
        [MaxLength(50)]
        [StringLength(50)]
        [Required]
        public string Title { get; set; }

        [MaxLength(255)]
        [StringLength(255)]
        public string Contents { get; set; }

        public int? Type { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength(1)]
        [StringLength(1)]
        public string UseType { get; set; }

        [MaxLength(20)]
        [StringLength(20)]
        public string UpdateUser { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
