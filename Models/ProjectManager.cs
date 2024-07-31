using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
namespace ProjManager.Models
{
    class LoadingException : Exception
    {
        public LoadingException(string message) : base(message) { }
        public LoadingException(string message, Exception inner) : base(message, inner) { }
    }
    public class ProjectManager
    {
        const string ProjectFile = ".project.json";
        static readonly string RootPath = @"S:\Coding"; // TODO : config away
        const string Editor = @"C:\Users\Amin\scoop\apps\vscode\current\Code.exe"; // TODO : config away
        const string EditorArgs = "{0}"; // TODO : config away
        public List<Project> Projects { get; private set; }
        public HashSet<string> Tags { get; private set; }
        private ProjectManager(List<Project> projects)
        {
            Projects = projects;
            Tags = [];
            foreach (var project in projects)
            {
                Tags.UnionWith(project.Tags);
            }
        }
        public static async Task<ProjectManager> Load(IStorageProvider storageProvider)
        {
            // TODO : LoadConfig()
            return await LoadProjects(storageProvider, RootPath);
        }
        public static async Task<ProjectManager> LoadProjects(IStorageProvider storageProvider, string path)
        {
            IStorageFolder? rootDir;

            rootDir = await storageProvider.TryGetFolderFromPathAsync(path);
            if (rootDir is null) throw new Exception("Root({}) doesn't exist or isn't a directory.");

            var projects = new List<Project>();

            await foreach (var item in rootDir.GetItemsAsync())
            {
                // Open project config file
                var projectFilepath = Path.Combine(item.Path.AbsolutePath, ProjectManager.ProjectFile);
                var projectFile = await storageProvider.TryGetFileFromPathAsync(projectFilepath);
                if (projectFile is null)
                {
                    Debug.Print($"Tried opening {projectFilepath} but failed. (Ignore if actually doesn't exist and isn't a project)");
                    continue;
                }
                // Get file creation time from file properties
                var _created = (await projectFile.GetBasicPropertiesAsync()).DateCreated;
                if (_created is null)
                {
                    Trace.WriteLine($"Couldn't get creation time for {projectFilepath}; Setting its time to 0.");
                }
                var created = _created ?? new DateTimeOffset(0, new TimeSpan(0));
                // Deserialize file content into ProjectInfo
                var fileStream = await projectFile.OpenReadAsync();
                ProjectInfo? projectInfo;
                try
                {
                    projectInfo = JsonSerializer.Deserialize<ProjectInfo>(fileStream);
                }
                catch (Exception e)
                {
                    Trace.WriteLine($"Couldn't parse JSON for file({projectFile.Path}):\n{e}");
                    continue;
                }
                if (projectInfo is null)
                {
                    Trace.WriteLine($"Couldn't parse JSON for file({projectFile.Path}).");
                    continue;
                }
                // Add project to list of projects
                projects.Add(new Project(item.Name, created, projectInfo));
            }
            return new ProjectManager(projects);
        }

        private static string? GetProjectPath(string name)
        {
            var path = Path.Join(RootPath, name);
            if (!Path.Exists(path)) return null;
            return path;
        }

        public void OpenInEditor(Project project, IStorageProvider storageProvider)
        {
            var projectPath = GetProjectPath(project.Name);
            if (projectPath is null || !Path.Exists(projectPath))
            {
                Trace.WriteLine($"Couldn't get project path for {project.Name} in {RootPath}.");
                return;
            }
            var args = string.Format(EditorArgs, projectPath);
            UpdateProject(null, project, storageProvider);
            Process.Start(Editor, args);
        }
        public void OpenInExplorer(Project project, ILauncher launcher, IStorageProvider storageProvider)
        {
            var projectPath = GetProjectPath(project.Name);
            if (projectPath is null || !Path.Exists(projectPath))
            {
                Trace.WriteLine($"Couldn't get project path for {project.Name} in {RootPath}.");
                return;
            }
            UpdateProject(null, project, storageProvider);
            launcher.LaunchUriAsync(new Uri(projectPath));
        }
        // TODO : better error handling
        public async void UpdateProject(Project? oldProject, Project newProject, IStorageProvider storageProvider)
        {
            var projectPath = Path.Combine(RootPath, newProject.Name);
            var projectDir = await storageProvider.TryGetFolderFromPathAsync(projectPath);

            // Create project directory if it doesn't exist
            if (projectDir is null)
            {
                var dir = await storageProvider.TryGetFolderFromPathAsync(RootPath);
                if (dir is null)
                {
                    Trace.WriteLine($"Couldn't open root folder : {RootPath}\n");
                    return;
                }
                projectDir = (await dir.CreateFolderAsync(newProject.Name))!;
            }

            // Change directory if project name's changed
            if (oldProject != null && newProject.Name != oldProject.Name)
            {

                var oldPath = Path.Combine(RootPath, oldProject.Name);
                var oldDir = await storageProvider.TryGetFolderFromPathAsync(oldPath);
                if (oldDir is null)
                {
                    Trace.WriteLine($"Can't open directory for project: {oldPath}");
                    return;
                }
                await foreach (var child in oldDir.GetItemsAsync())
                {
                    await child.MoveAsync(projectDir);
                }
                await oldDir.DeleteAsync();
            }

            var projectFilePath = Path.Combine(projectPath, ProjectManager.ProjectFile);
            var projectFile = await projectDir.CreateFileAsync(projectFilePath);

            if (projectFile is null)
            {
                Trace.WriteLine($"Couldn't create file: {projectFilePath}");
                return;
            }
            var stream = await projectFile.OpenWriteAsync();
            newProject.Accessed = DateTimeOffset.Now;
            JsonSerializer.Serialize(stream, newProject.Info);
            stream.Close();

            // replace the project in `projects` field with new one
            if (oldProject is not null)
            {
                Projects.RemoveAt(Projects.FindIndex((p) => p.Name == oldProject.Name));
            }
            Projects.Add(newProject);
        }
    }
}