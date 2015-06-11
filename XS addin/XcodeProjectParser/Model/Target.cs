using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XcodeProjectParser
{
	public class Target
	{
		public string ID { get; set; }

		public string Name { get; set; }

		public string ProjectTypeGuid { get; set; }

		public List<PBXFileReference> Files { get; set; }

		public List<string> Dependencies { get; set; }

		IList<string> sourceFiles;
		public IList<string> SourceFiles {
			get {
				if (sourceFiles == null)
					sourceFiles = Files.Where (c => c.FileType == PBXFileType.SourcecodeCObjc
							|| c.FileType == PBXFileType.SourcecodeCppObjcpp).
						Select (c => c.Path).ToList<string> ();
				return sourceFiles;
			}
		}

		IList<string> imageFiles;
		public IList<string> ImageFiles {
			get {
				if (imageFiles == null)
					imageFiles = Files.Where (c => c.FileType == PBXFileType.ImagePng
							|| c.FileType == PBXFileType.ImageJpeg).
						Select (c => ((PBXFileReference)c).Path).ToList<string> ();
				return imageFiles;
			}
		}

		IList<string> interfaceFiles;
		public IList<string> InterfaceFiles {
			get {
				if (interfaceFiles == null)
					interfaceFiles = Files.Where (c => c.FileType == PBXFileType.FileXib
							|| c.FileType == PBXFileType.FileStoryboard).
						Select (c => ((PBXFileReference)c).Path).ToList<string> ();
				return interfaceFiles;
			}
		}

		IList<string> imageAssets;
		public IList<string> ImageAssets {
			get {
				if (imageAssets == null)
					imageAssets = Files.Where (c => c.FileType == PBXFileType.FolderAssetCatalog).
						Select (c => c.Path).ToList<string> ();
				return imageAssets;
			}
		}

		IList<string> shaderFiles;
		public IList<string> ShaderFiles {
			get {
				if (shaderFiles == null)
					shaderFiles = Files.Where (c => c.FileType == PBXFileType.SourcecodeGlsl).
						Select (c => c.Path).ToList<string> ();
				return shaderFiles;
			}
		}

		IList<string> metalFiles;
		public IList<string> MetalFiles {
			get {
				if (metalFiles == null)
					metalFiles = Files.Where (c => c.FileType == PBXFileType.SourcecodeMetal).
						Select (c => c.Path).ToList<string> ();
				return metalFiles;
			}
		}

		IList<string> frameworks;
		public IList<string> Frameworks {
			get {
				if (frameworks == null)
					frameworks = Files.Where (c => c.FileType == PBXFileType.WrapperFramework).
						Select (c => Path.GetFileNameWithoutExtension (c.Name)).ToList<string> ();
				return frameworks;
			}
		}

		IList<string> plistFiles;
		public IList<string> PlistFiles {
			get {
				if (plistFiles == null)
					plistFiles = Files.Where (c => c.FileType == PBXFileType.TextPlistXml).
						Select (c => c.Path).ToList<string> ();
				return plistFiles;
			}
		}

		public Target ()
		{
			Files = new List<PBXFileReference> ();
			Dependencies = new List<string> ();
		}
	}
}

