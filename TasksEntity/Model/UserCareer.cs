using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.Model
{
    public partial class UserCareer
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int? CareerType { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public string CompanyTel { get; set; }
        public string CardUrl { get; set; }
        public int Main { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Remark { get; set; }
    }
}
