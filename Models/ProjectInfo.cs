using System;
using System.Collections.Generic;

namespace ProjManager.Models
{
    public class ProjectInfo
    {
        public required HashSet<string> tags { get; set; }
        public required DateTimeOffset accessed { get; set; }
    }
}