using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ProjManager.Views;

public partial class ProjectView : UserControl
{
    public ProjectView()
    {
        InitializeComponent();
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    public static readonly RoutedEvent<RoutedEventArgs> OpenedInEditor =
        RoutedEvent.Register<ProjectView, RoutedEventArgs>("OpenedInEditor",
             RoutingStrategies.Bubble);
    public static readonly RoutedEvent<RoutedEventArgs> OpenedInExplorer =
        RoutedEvent.Register<ProjectView, RoutedEventArgs>("OpenedInExplorer",
             RoutingStrategies.Bubble);
    public static readonly RoutedEvent<RoutedEventArgs> EditsProject =
    RoutedEvent.Register<ProjectView, RoutedEventArgs>("EditsProject",
         RoutingStrategies.Bubble);

    protected virtual void OpenInEditor(object source, RoutedEventArgs args)
    {
        var event_args = new RoutedEventArgs(OpenedInEditor);
        RaiseEvent(event_args);
    }
    protected virtual void OpenInExplorer(object source, RoutedEventArgs args)
    {
        var event_args = new RoutedEventArgs(OpenedInExplorer);
        RaiseEvent(event_args);
    }
    protected virtual void EditProject(object source, RoutedEventArgs args)
    {
        var event_args = new RoutedEventArgs(EditsProject);
        RaiseEvent(event_args);
    }

}