using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.Neo4jModels
{
    public class NeoFollowModel
    {
        public string UserID { get; set; }
        public DateTime Since { get; set; }
    }
}
