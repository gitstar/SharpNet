using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharpNet.DB.Custom
{
    [Table("tblTableViewInfo")]
    public class tblTableViewInfo
    {
        [Key]
        [Required]
        public int TableID { get; set; } // int

        [Required]
        public string Creator { get; set; } // int

        public string GameMode { get; set; }

        [Required]
        public int PlayerNum { get; set; } // int

        public int? RoundLimit { get; set; } // int

        public string MainSuit { get; set; } // int

        public string SubSuit { get; set; }  // int 

        public DateTime? CreateDate { get; set; } // smalldatetime

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength(1)]
        [StringLength(1)]
        [Required]
        public string UseType { get; set; } // char(1)
    }
}
