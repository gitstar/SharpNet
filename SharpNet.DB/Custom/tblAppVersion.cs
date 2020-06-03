using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharpNet.DB.Custom
{
    [Table("tblAppVersion")]
    public class tblAppVersion
    {
        [Key]
        [Required]
        public string Class { get; set; }

        [Key]
        [Required]
        public string Code { get; set; }

        [Key]      
        [Required]
        public string GameName { get; set; } // nvarchar(50)

        public string Version { get; set; }

        public string AndroidSize { get; set; }
        public string IphoneSize { get; set; }

        public string ServerUrl { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string UpdaterID { get; set; }
    }
}
