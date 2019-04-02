using SchedulerCommon.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.BM_Models
{
   public class TaskList
    {
        public List<TaskInfoEntity> taskInfoEntity { get; set; }
        public int TaskTotal { get; set; }
        public int PageNum { get; set; }
        public int CurrentPage { get; set; }
    }
}
