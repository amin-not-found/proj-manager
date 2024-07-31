using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Platform.Storage;
using ProjManager.Models;

namespace ProjManager.ViewModels;

public class EditWindowViewModel : ViewModelBase
{
    readonly Project OriginalProject;
    // old project, new project and storage provider
    public readonly Action<Project, Project, IStorageProvider> SaveAction;
    public Project Project { get; }
    public HashSet<string> PossibleTags { get; set; }
    public ObservableCollection<string> Tags { get; }
    public EditWindowViewModel(Project project, HashSet<string> possibleTags, Action<Project, Project, IStorageProvider> saveAction)
    {
        Project = project;
        // make a deep copy of project:
        OriginalProject = project.Copy();
        Tags = new(Project.Tags);
        PossibleTags = possibleTags;
        this.SaveAction = saveAction;
    }
    public void Save(HashSet<string> newTags, IStorageProvider storageProvider)
    {
        Project.Info.tags = newTags;
        SaveAction(Project, OriginalProject, storageProvider);
    }
}
