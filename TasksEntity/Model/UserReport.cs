using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.Model
{
    public partial class UserReport
    {
        public int ReportID { get; set; }
        public Guid UserID { get; set; }
        public int Status { get; set; }
        public Guid ReportUserID { get; set; }
        public string Reason { get; set; }
        public string SourceType { get; set; }
        public string ReportType { get; set; }
        public string ProcessingResults { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Contact { get; set; }
        public string Photos { get; set; }
        public string SourceID { get; set; }
    }
}
