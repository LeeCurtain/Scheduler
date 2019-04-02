using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.Neo4jModels
{
    public class NeoEmployeeRelationModel
    {
        public string Since { get; set; }
        public string Title { get; set; }
    }

    public class NeoFriendRelationModel
    {
        public string Since { get; set; }
    }

    public class NeoFollowRelationModel
    {
        public string Since { get; set; }
    }
}
