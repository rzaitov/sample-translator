using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using MonoDevelop.Core.ProgressMonitoring;

using MonoDevelop.Ide;
using MonoDevelop.Projects;

using ProjectTranslatorUI;
using XcodeProjectParser;

namespace ProjectTranslator
{
	public class SolutionGenerator
	{
		const string metadataFileName = "Metadata.xml";
		const string readmeFileName = "README.md";
		const string screenshotsFolderName = "Screenshots";
		const string resourcesFolderName = "Resources";

		ConversionPreferences preferences;
		XcodeProject xcodeProjectModel;

		string rootFolder;
		string projectName;

		public SolutionGenerator (XcodeProject xcodeProjectModel, ConversionPreferences preferences)
		{
			rootFolder = preferences.CSharpProjectFolderPath;
			this.xcodeProjectModel = xcodeProjectModel;
			this.preferences = preferences;

			projectName = Path.GetFileNameWithoutExtension (xcodeProjectModel.FilePath);
		}

		public void Run ()
		{
			CreateFileStructure ();
			GenerateMetadata ();
			GenerateReadme ();
			GenerateProjects ();
			GenerateSolution ();
		}

		void GenerateSolution ()
		{
			var solution = new Solution {
				Name = projectName
			};

			var progressMonitor = new SimpleProgressMonitor ();
			var solutionPath = Path.Combine (rootFolder, projectName, solution.Name);

			foreach (var target in xcodeProjectModel.Targets) {
				var projectPath = Path.Combine (rootFolder, projectName, target.Name, target.Name + ".csproj");
				var project = Project.LoadProject (projectPath, progressMonitor);
				solution.RootFolder.AddItem (project);
			}

			solution.AddConfiguration ("Debug|iPhoneSimulator", true);
			solution.AddConfiguration ("Release|iPhoneSimulator", true);
			solution.AddConfiguration ("Debug|iPhone", true);
			solution.AddConfiguration ("Release|iPhone", true);
			solution.AddConfiguration ("Ad-Hoc|iPhone", true);
			solution.AddConfiguration ("AppStore|iPhone", true);

			solution.Save (solutionPath, progressMonitor);
			IdeApp.Workspace.OpenWorkspaceItem (solutionPath + ".sln");
		}

		void GenerateProjects ()
		{
			foreach (var target in xcodeProjectModel.Targets) {

				var projectGenerationSettings = new ProjectGenerationSettings {
					OveriwriteLaunchImages = preferences.OveriwriteLaunchImages,
					OverwriteAppIcons = preferences.OverwriteAppIcons,
					SolutionName = projectName,
					PathToXcodeProject = xcodeProjectModel.FilePath,
					CsharpSolutionParentFolder = preferences.CSharpProjectFolderPath,
					TargetProject = target
				};

				var projectGenerator = new ProjectGenerator (projectGenerationSettings);
				projectGenerator.Run ();
			}
		}

		void CreateFileStructure ()
		{
			Directory.CreateDirectory (Path.Combine (rootFolder, projectName));
			Directory.CreateDirectory (Path.Combine (rootFolder, projectName, screenshotsFolderName));
		}

		void GenerateReadme ()
		{
			string templateContent = System.IO.File.ReadAllText (Path.Combine (resourcesFolderName, readmeFileName));
			File.WriteAllText (Path.Combine (rootFolder, projectName, readmeFileName), templateContent);
		}

		void GenerateMetadata ()
		{
			string templateContent = System.IO.File.ReadAllText (Path.Combine (resourcesFolderName, metadataFileName));
			var metadata = XDocument.Parse (templateContent);
			var idElement = metadata.Descendants ().
				Where (p => p.Name.LocalName == "ID").First ();
			idElement.Value = Guid.NewGuid ().ToString ();
			metadata.Save (Path.Combine (rootFolder, projectName, metadataFileName));
		}
	}
}

