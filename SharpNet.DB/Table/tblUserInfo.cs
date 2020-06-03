using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharpNet.DB.Table
{
    [Table("tbl_users")]
    public class tbl_users
    { 
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Key]
        [Required]
        public string Nickname { get; set; }

        [Key]
        [Required]
        public string Email { get; set; }
        public string Password { get; set; }

      
        public string Name { get; set; }
        public string Mobile { get; set; }

        public string MacAddress { get; set; }

        public string IpAddress { get; set; }

        public sbyte RoleId { get; set; }
        public sbyte IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDtm { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDtm { get; set; }
        public string Pay_code { get; set; }
        public string Token_code { get; set; }
        public DateTime? Last_login { get; set; }
        public int workPlay { get; set; }
    }
}
