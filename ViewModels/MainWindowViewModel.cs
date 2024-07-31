using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Platform.Storage;
using ProjManager.Models;

namespace ProjManager.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    // TODO : Show a progress bar while loading
    public ObservableCollection<ProjectViewModel> SearchResults { get; } = [];
    public ProjectManager? manager;
    // TODO : Set using config
    public string SortCriteria = "Sort By Last Access";
    public string SortOrder = "Descending";
    public string SearchText { get; set; } = "";
    public async void Load(IStorageProvider storageProvider)
    {
        manager = await ProjectManager.Load(storageProvider);
        Search("");
    }
    public void SaveProject(Project newProject, Project oldProject, IStorageProvider storageProvider)
    {
        manager?.UpdateProject(oldProject, newProject, storageProvider);
        Search(SearchText);
    }
    public void Search(string s)
    {
        SearchResults.Clear();
        if (manager is null)
        {
            Trace.WriteLine("Manager is null. This shouldn't be happening. Returning from MainWindowViewModel.search empty handedly.");
            return;
        };
        // TODO : better search functionality
        foreach (var project in manager.Projects)
        {
            var nameContains = project.Name.Contains(s, StringComparison.InvariantCultureIgnoreCase);
            var tagsContain = false;
            foreach (var tag in project.Tags)
            {
                tagsContain = tag.Contains(s, StringComparison.InvariantCultureIgnoreCase);
                if (tagsContain) break;
            }
            if (nameContains || tagsContain)
            {
                SearchResults.Add(new ProjectViewModel(project));
            }
        }
        SortResults();
    }
    public void SortResults()
    {
        IEnumerable<ProjectViewModel> sortedResults = (SortCriteria, SortOrder) switch
        {
            ("Sort By Last Access", "Ascending") => SearchResults.OrderBy(x => x.Accessed),
            ("Sort By Creation", "Ascending") => SearchResults.OrderBy(x => x.Created),
            ("Sort By Name", "Ascending") => SearchResults.OrderBy(x => x.Name),
            ("Sort By Last Access", "Descending") => SearchResults.OrderByDescending(x => x.Accessed),
            ("Sort By Creation", "Descending") => SearchResults.OrderByDescending(x => x.Created),
            ("Sort By Name", "Descending") => SearchResults.OrderByDescending(x => x.Name),
            _ => SearchResults,
        };
        sortedResults = sortedResults.ToArray();
        SearchResults.Clear();
        foreach (var result in sortedResults)
        {
            SearchResults.Add(result);
        }
    }

}
