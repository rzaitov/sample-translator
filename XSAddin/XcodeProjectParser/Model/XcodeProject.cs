using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translator.Parser
{
	public class XcodeProject
	{
		TreeNode<Tuple<string, IPBXElement>> projectFileTree;

		public double ArchiveVersion { get; set; }

		public double ObjectsVersion { get; set; }

		public IList<IPBXElement> Objects { get; set; }

		public List<Target> Targets { get; set; }

		public string RootObject { get; set; }

		public string FilePath { get; set; }

		List<PBXFileReference> headerFiles;
		List<PBXFileReference> HeaderFiles {
			get {
				headerFiles = Objects.Where (obj => obj.ObjectType == IsaType.PBXFileReference
					&& (((PBXFileReference)obj).FileType == PBXFileType.SourcecodeCH || ((PBXFileReference)obj).FileType == PBXFileType.SourcecodeCppH))
					.Cast<PBXFileReference> ().ToList ();
				return headerFiles;
			}
		}

		string pchFilePath;
		string PCHFilePath {
			get {
				pchFilePath = pchFilePath ?? HeaderFiles.Where (header => Path.GetExtension (header.Path) == ".pch").Select (header => header.Path).FirstOrDefault ();
				return pchFilePath;
			}
		}

		PBXProject projectElement;
		public PBXProject ProjectElement {
			get {
				projectElement = projectElement ?? (PBXProject)Objects.Where (c => c.ID == RootObject).FirstOrDefault ();
				return projectElement;
			}
		}

		public XcodeProject ()
		{
			Objects = new List<IPBXElement> ();
			Targets = new List<Target> ();
		}

		void AnalyzeProjectDependecies ()
		{
			var targets = ProjectElement.Targets;

			// TODO;
			foreach (var targetID in targets) {
				var target = CreateBuildTarget (targetID);

				if (target.ProjectType == SharpProjectType.None)
					continue;
				Targets.Add (target);
			}
		}

		Target CreateBuildTarget (string nativeTargetID)
		{
			var nativeTarget = (PBXNativeTarget)GetElementById (nativeTargetID);

			var result = new Target {
				Name = nativeTarget.Name.Replace ("\"", string.Empty),
				ID = nativeTarget.ID,
				PCHFilePath = PCHFilePath,
				ProjectType = nativeTarget.ProductType.Replace ("\"", string.Empty).GetEnumMemberByDescription<SharpProjectType> ()
			};

			foreach (var dependencyID in nativeTarget.Dependencies) {
				var targetDependency = (PBXTargetDependency)GetElementById (dependencyID);
				var containerItemProxy = (PBXContainerItemProxy)GetElementById (targetDependency.TargetProxy);
				result.Dependencies.Add (containerItemProxy.RemoteGlobalIDString);
			}

			foreach (var buildPhaseID in nativeTarget.BuildPhases) {
				var buildPhase = (PBXBuildPhaseBase)GetElementById (buildPhaseID);
					
				if (buildPhase.Files == null || buildPhase.Files.Count == 0)
					continue;
				
				foreach (var buildFileID in buildPhase.Files) {
					var buildFile = (PBXBuildFile)GetElementById (buildFileID);
					var buildReference = GetElementById (buildFile.FileRef);

					if (buildReference.GetType () == typeof(PBXFileReference)) {
						result.Files.Add ((PBXFileReference)buildReference);
					} else if (buildReference.GetType () == typeof(PBXVariantGroup)) {
						var variantGroup = (PBXVariantGroup)buildReference;
						foreach (var children in variantGroup.Children)
							result.Files.Add ((PBXFileReference)GetElementById (children));
					}
				}
			}

			foreach (var headerPath in HeaderFiles.Select (header => header.Path))
				result.SourceHeaderFiles.Add (headerPath);

			return result;
		}

		public void AnalyzeProjectsFileStructure ()
		{
			var mainGroupId = ProjectElement.MainGroup;
			var rootElement = (PBXGroupBase)GetElementById (mainGroupId);
			projectFileTree = new TreeNode<Tuple<string, IPBXElement>> (new Tuple<string, IPBXElement> (mainGroupId, rootElement));
			IterateProjectTree (rootElement, projectFileTree);

			foreach (TreeNode<Tuple<string, IPBXElement>> node in projectFileTree) {
				if (node.Data.Item2.GetType () == typeof(PBXFileReference)) {
					var fullPath = ConstructPath (node, ((PBXFileReference)node.Data.Item2).Path);
					((PBXFileReference)node.Data.Item2).Path = fullPath.Replace ("\"", string.Empty);
					Console.WriteLine (((PBXFileReference)node.Data.Item2).Path);
				}
			}

			AnalyzeProjectDependecies ();
		}

		IPBXElement GetElementById (string id)
		{
			return Objects.Where (c => c.ID == id).FirstOrDefault ();
		}

		void IterateProjectTree (PBXGroupBase parentGroup, TreeNode<Tuple<string, IPBXElement>> parentNode)
		{
			if (parentGroup.Children.Count == 0)
				return;

			foreach (var childElementId in parentGroup.Children) {
				var childElement = GetElementById (childElementId);
				var childNode = parentNode.AddChild (new Tuple<string, IPBXElement> (childElementId, childElement));

				if (childElement.GetType ().BaseType == typeof(PBXGroupBase))
					IterateProjectTree ((PBXGroupBase)childElement, childNode);
			}
		}

		string ConstructPath (TreeNode<Tuple<string, IPBXElement>> node, string path)
		{
			if (node.Parent != null) {
				var parentNode = node.Parent;
				if (((PBXGroupBase)parentNode.Data.Item2).Path != null)
					path = System.IO.Path.Combine (((PBXGroupBase)parentNode.Data.Item2).Path, path);
				return ConstructPath (parentNode, path);
			}

			return path;
		}
	}
}
