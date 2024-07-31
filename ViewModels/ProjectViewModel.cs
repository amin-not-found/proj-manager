using System;
using System.Collections.Generic;
using ProjManager.Models;

namespace ProjManager.ViewModels;

public class ProjectViewModel(Project project) : ViewModelBase
{
    public readonly Project project = project;

    public string Name => project.Name;
    public DateTimeOffset Created => project.Accessed;
    public DateTimeOffset Accessed => project.Accessed;
    public HashSet<string> Tags
    {
        get { return project.Tags; }
    }
    public string TagsText { get { return string.Join(", ", Tags); } }
}
