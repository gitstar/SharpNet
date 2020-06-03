using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharpNet.DB.Table
{
    [Table("tblLoginHistory")]
    public class tblLoginHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int? ID { get; set; }

        [Key]
        [Required]
        public int? UserID { get; set; }

        [Key]
        [Required]
        public DateTime? JoinDate { get; set; }

        [MaxLength(30)]
        [StringLength(30)]
        public string JoinIP { get; set; }

        [MaxLength(4)]
        [StringLength(4)]
        public string JoinState { get; set; }
    }
}
