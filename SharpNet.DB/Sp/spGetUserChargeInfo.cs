using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SharpNet.DB.Sp
{
    [Table("spGetUserChargeInfo")]
    public class spGetUserChargeInfo
    {
        public string UserNickName { get; set; }

        public DateTime? ChargeDate { get; set; }

        public int? ItemCount { get; set; }

        public int? ChargeMoney { get; set; }

        public string AgentNum { get; set; }

        public int? AgentRate { get; set; }

        public double? keepMoney { get; set; }
    }
}
