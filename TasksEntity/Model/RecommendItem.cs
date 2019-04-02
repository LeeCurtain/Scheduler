using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.Model
{
    public partial class RecommendItem
    {
        public int Id { get; set; }
        public Guid ItemId { get; set; }
        public int Type { get; set; }
        public int? Vip { get; set; }
        public string Tags { get; set; }
        public decimal Grade{ get;set;}
        //public string Payload { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public int IsRecomm { get; set; }
    }
}
