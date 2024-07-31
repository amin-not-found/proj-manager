using System;
using System.Collections.Generic;

namespace ProjManager.Models
{
    public class Project(string name, DateTimeOffset created, ProjectInfo projectInfo)
    {
        public string Name { get; set; } = name;
        public readonly DateTimeOffset created = created;
        public ProjectInfo Info { get; set; } = projectInfo;

        public DateTimeOffset Accessed
        {
            get { return Info.accessed; }
            set { Info.accessed = value; }
        }

        public HashSet<string> Tags
        {
            get { return Info.tags; }
        }
        public Project Copy()
        {
            var info = new ProjectInfo
            {
                tags = new HashSet<string>(Tags),
                accessed = Accessed
            };
            return new Project(Name, created, info);
        }
        public void UpdateAccess()
        {
            Accessed = DateTimeOffset.Now;
        }
    }
}