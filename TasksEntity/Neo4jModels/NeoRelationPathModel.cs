using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.Neo4jModels
{
    public class NeoRelationPathModel
    {
        public int PathCount { get; set; }
        public List<NeoPathModel> Nodes { get; set; }
    }

    public class NeoPathModel
    {
        public List<string> NodeList { get; set; }
    }

    public class NeoRelationPathCountModel
    {
        public int PathCount { get; set; }
    }
}
