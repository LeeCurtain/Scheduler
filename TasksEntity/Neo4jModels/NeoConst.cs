using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.Neo4jModels
{
    public class NeoNodeConst
    {
        public const string Person = "Person";
        public const string Company = "Company";
        public const string Region = "Region";
        public const string Industry = "Industry";
    }

    public class NeoRelationConst
    {
        public const string Block = "Block";
        public const string Friend = "Friend";
        public const string Follow = "Follow";
        public const string Employee = "Employee";
        public const string Belong = "Belong";
        public const string Field = "Field";
    }
}
