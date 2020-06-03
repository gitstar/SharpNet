using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharpNet.DB.Table
{
    [Table("tbl_payments")]
    public class tblPaymentInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Amount { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime EndDate { get; set; }

        public string ip { get; set; }
    }
}
