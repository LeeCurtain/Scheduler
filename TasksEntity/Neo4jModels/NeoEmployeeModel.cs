using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.Neo4jModels
{
    public class NeoEmployeePersonModel
    {
        public NeoPersonModel User { get; set; }
        public NeoEmployeeRelationModel Employee { get; set; }
    }

    public class NeoEmployeeCompanyModel
    {
        public NeoCompanyModel Company { get; set; }
        public NeoEmployeeRelationModel Employee { get; set; }
    }
}
