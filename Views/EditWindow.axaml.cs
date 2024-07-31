using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ProjManager.ViewModels;

namespace ProjManager.Views;

public partial class EditWindow : Window
{
    // TODO : button to close window next to `Save`
    public EditWindow(EditWindowViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        SelectedTags.AttachedToVisualTree += (s, e) => SelectedTags.SelectAll();

    }
    void SaveProject(object? sender, RoutedEventArgs args)
    {

        var tags = new HashSet<string>();
        foreach (var tag in SelectedTags.SelectedItems)
        {
            tags.Add((tag as string)!);
        }
        (DataContext as EditWindowViewModel)!.Save(tags, StorageProvider);
        Close();
    }
    void SelectTag(object? sender, KeyEventArgs args)
    {
        if (args.Key != Key.Enter) return;

        var tag = EnteredTag.Text;
        EnteredTag.Text = "";
        if (tag is null || tag == "") return;
        var tags = (DataContext as EditWindowViewModel)!.Tags;
        if (!tags.Contains(tag))
        {
            tags.Add(tag);
            SelectedTags.SelectedItems.Add(tag);
            return;
        }
        if (SelectedTags.SelectedItems.Contains(tag))
        {
            SelectedTags.SelectedItems.Remove(tag);
            return;
        }
        SelectedTags.SelectedItems.Add(tag);
    }
}