using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ProjManager.ViewModels;

namespace ProjManager.Views;

public partial class MainWindow : Window

{
    public MainWindow()
    {
        InitializeComponent();
        AddHandler(ProjectView.OpenedInEditor, OpenInEditor);
        AddHandler(ProjectView.OpenedInExplorer, OpenInExplorer);
        AddHandler(ProjectView.EditsProject, EditProject);
        Loaded += (s, e) => ViewModel.Load(StorageProvider);
        searchBox.AttachedToVisualTree += (s, e) => searchBox.Focus();
    }
    MainWindowViewModel ViewModel
    {
        get
        {
            return (DataContext as MainWindowViewModel)!;
        }
    }
    public void OnLoad(object? sender, RoutedEventArgs args)
    {
        ViewModel.Load(StorageProvider);
    }
    public void OnSearchInteraction(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property.ToString() != "Text") return;
        var text = args.NewValue as string;
        ViewModel.Search(text ?? "");
    }
    public void OpenInEditor(object? sender, RoutedEventArgs args)
    {
        var project = ((args.Source as ProjectView)?.DataContext as ProjectViewModel)?.project;
        if (project is null) return;
        ViewModel.manager?.OpenInEditor(project, StorageProvider);
    }
    public void OpenInExplorer(object? sender, RoutedEventArgs args)
    {
        var project = ((args.Source as ProjectView)?.DataContext as ProjectViewModel)?.project;
        if (project is null) return;
        ViewModel.manager?.OpenInExplorer(project, Launcher, StorageProvider);
    }
    public async void EditProject(object? sender, RoutedEventArgs args)
    {
        var project = ((args.Source as ProjectView)?.DataContext as ProjectViewModel)?.project;
        if (project is null) return;
        var saveAction = ViewModel.SaveProject;
        var possibleTags = ViewModel.manager!.Tags;
        var editVM = new EditWindowViewModel(project, possibleTags, saveAction);
        var editWindow = new EditWindow(editVM);
        await editWindow.ShowDialog(this);
    }
    public void ChangeSortCriteria(object? sender, SelectionChangedEventArgs args)
    {
        if (DataContext is not MainWindowViewModel vm) return;
        var choice = (args.AddedItems[0] as ComboBoxItem)?.Content as string;
        vm.SortCriteria = choice ?? "Sort By Last Access";
        vm.SortResults();
    }
    public void ChangeSortOrder(object? sender, SelectionChangedEventArgs args)
    {
        if (DataContext is not MainWindowViewModel vm) return;
        var choice = (args.AddedItems[0] as ComboBoxItem)?.Content as string;
        vm.SortOrder = choice ?? "Descending";
        vm.SortResults();
    }
}