using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Translator.Parser
{
	public class Target
	{
		public string ID { get; set; }

		public string Name { get; set; }

		public SharpProjectType ProjectType { get; set; }

		public List<PBXFileReference> Files { get; set; }

		public List<string> Dependencies { get; set; }

		public string PCHFilePath { get; set; }

		IList<string> sourceFiles;
		public IList<string> SourceFiles {
			get {
				sourceFiles = Files.Where (c => c.FileType == PBXFileType.SourcecodeCObjc
						|| c.FileType == PBXFileType.SourcecodeCppObjcpp).
					Select (c => c.Path).ToList<string> ();
				return sourceFiles;
			}
		}

		IList<string> headerFiles;
		public IList<string> SourceHeaderFiles {
			get {
				if (headerFiles == null)
					headerFiles = Files.Where (c => c.FileType == PBXFileType.SourcecodeCH
						|| c.FileType == PBXFileType.SourcecodeCppH).
						Select (c => c.Path).ToList<string> ();
				return headerFiles;
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

		IList<string> scnKitAssets;
		public IList<string> ScnKitAssets {
			get {
				if (scnKitAssets == null)
					scnKitAssets = Files.Where (c => c.FileType == PBXFileType.FileScp
						|| c.FileType == PBXFileType.TextXmlDae
						|| c.FileType == PBXFileType.FileBplist).
					Select (c => ((PBXFileReference)c).Path).ToList<string> ();;
				return scnKitAssets;
			}
		}

		IList<string> textureAtlases;
		public IList<string> TextureAtlases {
			get {
				if (textureAtlases == null)
					textureAtlases = Files.Where (c => c.FileType == PBXFileType.FolderAssetAtlas).
						Select (c => ((PBXFileReference)c).Path).ToList<string> ();
				return textureAtlases;
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

