using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using XcodeProjectParser;

namespace ProjectTranslator
{
	public class ProjectGenerator
	{
		const string resourcesFolderName = "Resources";
		const string screenshotsFolderName = "Screenshots";
		const string projectFileTemplateName = "ProjectFileTemplate.xml";
		const string entitlmentsFileName = "Entitlements.plist";
		const string xmlNamespace = "{http://schemas.microsoft.com/developer/msbuild/2003}";

		Target currentTarget;
		ProjectGenerationSettings settings;
		string rootFolder;
		string xcodeProjPath;
		string projectName;
		string solutionName;

		string ProjectPath {
			get {
				return Path.Combine (rootFolder, solutionName, projectName);
			}
		}

		public ProjectGenerator (ProjectGenerationSettings generationSettings)
		{
			settings = generationSettings;
			rootFolder = generationSettings.CsharpSolutionParentFolder;
			currentTarget = generationSettings.TargetProject;
			projectName = generationSettings.TargetProject.Name;
			xcodeProjPath = generationSettings.PathToXcodeProject;
			solutionName = generationSettings.SolutionName;
		}

		public void Run ()
		{
			CreateFileStructure ();
			GenerateProject ();
		}

		void CreateFileStructure ()
		{
			Directory.CreateDirectory (Path.Combine (ProjectPath, resourcesFolderName));
			Directory.CreateDirectory (Path.Combine (ProjectPath));
		}

		void GenerateProject ()
		{
			string templateContent = System.IO.File.ReadAllText (Path.Combine (resourcesFolderName, projectFileTemplateName));
			var projectXml = XDocument.Parse (templateContent);

			AddSourceFiles (projectXml);
			AddImageAssets (projectXml);
			AddImages (projectXml);
			AddInterfaceDefintions (projectXml);

			AddProjectName (projectXml);
			AddAssemblyName (projectXml);
			AddProjectGuid (projectXml);
			AddProjectTypeGuid (projectXml);

			AddMetalFiles (projectXml);
			AddShaderFiles (projectXml);
			AddEntitlementsAndInfo (projectXml);
			AddSceneKitAssets (projectXml);

			using (var stream = File.Create (Path.Combine (ProjectPath, projectName + ".csproj")))
				projectXml.Save (stream);
		}

		void AddSceneKitAssets (XDocument projectXml)
		{
			// TODO
		}

		void AddImageAtlases (XDocument projectXml)
		{
			// TODO
		}

		void AddImages (XDocument projectXml)
		{
			var imagesItemGroup = new XElement (xmlNamespace + "ItemGroup");

			foreach (var imagePath in currentTarget.ImageFiles) {
				string sourcePath = Path.Combine (Directory.GetParent (xcodeProjPath).FullName, imagePath);
				string destinationPath = Path.Combine (ProjectPath, resourcesFolderName, Path.GetFileName (imagePath));
				File.Copy (sourcePath, destinationPath, true);

				var index = destinationPath.IndexOf (resourcesFolderName);
				var resoursePath = destinationPath.Substring (index);

				var imageElement = new XElement (xmlNamespace + "BundleResource", new XAttribute ("Include", resoursePath.NormalizePath ()));
				imagesItemGroup.Add (imageElement);
			}

			projectXml.Root.Add (imagesItemGroup);
		}

		void AddEntitlementsAndInfo (XDocument projectXml)
		{
			var plistFilesItemGroup = new XElement (xmlNamespace + "ItemGroup");

			foreach (var infoPlist in currentTarget.PlistFiles) {
				string sourcePath = Path.Combine (Directory.GetParent (xcodeProjPath).FullName, infoPlist);

				// TODO report about this change
				var infoPlistFileName = Path.GetFileName (infoPlist);
				if (infoPlistFileName.Contains (projectName))
					infoPlistFileName = "Info.plist";
						
				string destinationPath = Path.Combine (ProjectPath, infoPlistFileName);
				File.Copy (sourcePath, destinationPath, true);

				var plistElement = new XElement (xmlNamespace + "None", new XAttribute ("Include", infoPlistFileName));
				plistFilesItemGroup.Add (plistElement);
			}

			string entitlements = System.IO.File.ReadAllText (Path.Combine (resourcesFolderName, entitlmentsFileName));
			var entitlementsPath = Path.Combine (ProjectPath, entitlmentsFileName);
			File.WriteAllText (entitlementsPath, entitlements);

			var entitlementsElement = new XElement (xmlNamespace + "None", new XAttribute ("Include", Path.GetFileName (entitlementsPath)));
			plistFilesItemGroup.Add (entitlementsElement);

			projectXml.Root.Add (plistFilesItemGroup);
		}

		void AddShaderFiles (XDocument projectXml)
		{
			if (currentTarget.ShaderFiles.Count == 0)
				return;

			var shaderFilesItemGroup = new XElement (xmlNamespace + "ItemGroup");

			foreach (var sourceFile in currentTarget.ShaderFiles) {
				string sourcePath = Path.Combine (Directory.GetParent (xcodeProjPath).FullName, sourceFile);
				string destinationPath = Path.Combine (ProjectPath, resourcesFolderName, Path.GetFileName (sourceFile));
				File.Copy (sourcePath, destinationPath, true);

				var index = destinationPath.IndexOf (resourcesFolderName);
				var resoursePath = destinationPath.Substring (index);

				var shaderFileElement = new XElement (xmlNamespace + "Content", new XAttribute ("Include", resoursePath.NormalizePath ()));
				shaderFileElement.Add (new XElement (xmlNamespace + "CopyToOutputDirectory", "Always"));
				shaderFilesItemGroup.Add (shaderFileElement);
			}

			projectXml.Root.Add (shaderFilesItemGroup);
		}

		void AddMetalFiles (XDocument projectXml)
		{
			if (currentTarget.MetalFiles.Count == 0)
				return;

			var metalFilesItemGroup = new XElement (xmlNamespace + "ItemGroup");

			foreach (var sourceFile in currentTarget.MetalFiles) {
				string sourcePath = Path.Combine (Directory.GetParent (xcodeProjPath).FullName, sourceFile);
				string destinationPath = Path.Combine (ProjectPath, resourcesFolderName, Path.GetFileName (sourceFile));
				File.Copy (sourcePath, destinationPath, true);

				var index = destinationPath.IndexOf (resourcesFolderName);
				var resoursePath = destinationPath.Substring (index);

				var metalFileElement = new XElement (xmlNamespace + "Metal", new XAttribute ("Include", resoursePath.NormalizePath ()));
				metalFilesItemGroup.Add (metalFileElement);
			}

			projectXml.Root.Add (metalFilesItemGroup);
		}

		void AddSourceFiles (XDocument projectXml)
		{
			var sourceFilesItemGroup = new XElement (xmlNamespace + "ItemGroup");

			List<string> cSharpFiles = new List<string> ();

			foreach (var sourceFile in currentTarget.SourceFiles) {
				var sourceElement = new XElement (xmlNamespace + "Compile", new XAttribute ("Include", sourceFile.ConvertPathToSharpName ()));
				sourceFilesItemGroup.Add (sourceElement);

				string sourceFilePath = Path.Combine (ProjectPath, Path.GetFileName (sourceFile));
				if (!Directory.Exists (Path.GetDirectoryName (sourceFilePath)))
					Directory.CreateDirectory (Path.GetDirectoryName (sourceFilePath));
				
				var sourceFiles = Path.Combine (Directory.GetParent (xcodeProjPath).FullName, sourceFile);
				cSharpFiles.Add (sourceFiles);
			}

			projectXml.Root.Add (sourceFilesItemGroup);

			var translationConfig = new CodeTranslationConfiguration {
				ProjectNamespace = projectName,
				ProjectPath = ProjectPath,
				FilePaths = cSharpFiles,
				Frameworks = currentTarget.Frameworks
			};

			CodeTranslator.Default.Translate (translationConfig);
		}

		void AddImageAssets (XDocument projectXml)
		{
			var resourceFilesItemGroup = new XElement (xmlNamespace + "ItemGroup");

			CopyXamarinAssets (ref resourceFilesItemGroup);
			CopyAssetsFromXcodeProject (ref resourceFilesItemGroup);

			projectXml.Root.Add (resourceFilesItemGroup);
		}

		void CopyXamarinAssets (ref XElement resourceFilesItemGroup)
		{
			var files = new List<string> ();

			if (settings.OverwriteAppIcons)
				files.AddRange (Directory.GetFiles ("Resources/AppIcon.appiconset"));

			if (settings.OveriwriteLaunchImages)
				files.AddRange (Directory.GetFiles ("Resources/LaunchImage.launchimage"));

			var imageAssetsFolder = Path.Combine (ProjectPath, resourcesFolderName, "Images.xcassets");
			if (!Directory.Exists (imageAssetsFolder))
				Directory.CreateDirectory (imageAssetsFolder);

			var iconsAssetsFolder = Path.Combine (imageAssetsFolder, "AppIcon.appiconset");
			if (!Directory.Exists (iconsAssetsFolder) && settings.OverwriteAppIcons)
				Directory.CreateDirectory (iconsAssetsFolder);

			var launchAssetsFolder = Path.Combine (imageAssetsFolder, "LaunchImage.launchimage");
			if (!Directory.Exists (launchAssetsFolder) && settings.OveriwriteLaunchImages)
				Directory.CreateDirectory (launchAssetsFolder);

			foreach (var file in files) {
				var sourcePath = file.Replace (resourcesFolderName, "Images.xcassets");
				string destinationPath = Path.Combine (ProjectPath, resourcesFolderName, sourcePath);
				File.Copy (file, destinationPath, true);

				var index = destinationPath.IndexOf (resourcesFolderName);
				var resoursePath = destinationPath.Substring (index);
				var resourceElement = new XElement (xmlNamespace + "ImageAsset", new XAttribute ("Include", resoursePath.NormalizePath ()));
				resourceFilesItemGroup.Add (resourceElement);
			}
		}

		void CopyAssetsFromXcodeProject (ref XElement resourceFilesItemGroup)
		{
			if (currentTarget.ImageAssets.Count == 0)
				return;

			foreach (var sourceFile in currentTarget.ImageAssets) {
				string sourcePath = Path.Combine (Directory.GetParent (xcodeProjPath).FullName, sourceFile);
				string destinationPath = Path.Combine (ProjectPath, resourcesFolderName, Path.GetFileName (sourceFile));

				foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
					Directory.CreateDirectory (dirPath.Replace (sourcePath, destinationPath));

				foreach (string newPath in Directory.GetFiles (sourcePath, "*.*", SearchOption.AllDirectories)) {
					if (Path.GetFileName (newPath).Contains (".DS_Store")
						|| (newPath.Contains ("AppIcon.appiconset") && settings.OverwriteAppIcons)
						|| (newPath.Contains ("LaunchImage.launchimage") && settings.OveriwriteLaunchImages))
						continue;

					File.Copy (newPath, newPath.Replace (sourcePath, destinationPath), true);

					var resoursePath = newPath.Replace (sourcePath, destinationPath).Replace (xcodeProjPath, string.Empty);
					var index = resoursePath.IndexOf (resourcesFolderName);
					resoursePath = resoursePath.Substring (index);
					var resourceElement = new XElement (xmlNamespace + "ImageAsset", new XAttribute ("Include", resoursePath.NormalizePath ()));
					resourceFilesItemGroup.Add (resourceElement);
				}
			}
		}

		void AddInterfaceDefintions (XDocument projectXml)
		{
			if (currentTarget.InterfaceFiles.Count == 0)
				return;
			
			var interfaceDefinitionItemGroup = new XElement (xmlNamespace + "ItemGroup");

			foreach (var sourceFile in currentTarget.InterfaceFiles) {
				string sourcePath = Path.Combine (Directory.GetParent (xcodeProjPath).FullName, sourceFile);
				string destinationPath = Path.Combine (ProjectPath, Path.GetFileName (sourceFile));
				File.Copy (sourcePath, destinationPath, true);

				var sourceElement = new XElement (xmlNamespace + "InterfaceDefinition", new XAttribute ("Include", Path.GetFileName (sourceFile)));
				interfaceDefinitionItemGroup.Add (sourceElement);
			}

			projectXml.Root.Add (interfaceDefinitionItemGroup);
		}

		void AddProjectName (XDocument projectXml)
		{
			var rootNamespace = projectXml.Descendants ().
				Where (p => p.Name.LocalName == "RootNamespace").First ();
			rootNamespace.Value = projectName;
		}

		void AddAssemblyName (XDocument projectXml)
		{
			var assemblyName = projectXml.Descendants ().
				Where (p => p.Name.LocalName == "AssemblyName").First ();
			assemblyName.Value = projectName;
		}

		void AddProjectGuid (XDocument projectXml)
		{
			var projectGuid = projectXml.Descendants ().
				Where (p => p.Name.LocalName == "ProjectTypeGuids").First ();
			projectGuid.Value = settings.TargetProject.ProjectTypeGuid;
		}

		void AddProjectTypeGuid (XDocument projectXml)
		{
			var projectTypeGuids = projectXml.Descendants ().
				Where (p => p.Name.LocalName == "ProjectGuid").First ();
			projectTypeGuids.Value = string.Format ("{{{0}}}", Guid.NewGuid ().ToString ());
		}
	}
}

