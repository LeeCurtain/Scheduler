using SchedulerModel.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.BM_Models
{
    public class TaskTG
    {
        public List<TriggerModel> triggers { get; set; }
        public List<GroupModel> groups { get; set; }
    }
    public class TaskTTG
    {
        public TaskModel Task { get; set; }
        public List<TriggerModel> triggers { get; set; }
        public List<GroupModel> groups { get; set; }
    }
}
