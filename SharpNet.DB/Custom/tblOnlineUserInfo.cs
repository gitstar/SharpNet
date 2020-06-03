using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharpNet.DB.Custom
{
    [Table("tblOnlineUserInfo")]
    public class tblOnlineUserInfo
    { 
        [Key]      
        [Required]
        public int UserID { get; set; } //int

        [Key]
        [MaxLength(50)]
        [StringLength(50)]
        [Required]
        public string UserNickName { get; set; } // nvarchar(50)

        public int TableID { get; set; }
        public decimal? Money { get; set; } // money
        public int? Item { get; set; } // int

        public int? UserState { get; set; } // int

        public string Vip { get; set; } // int

        public  DateTime StartDate { get; set; }
     
    }
}
