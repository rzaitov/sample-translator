using System.ComponentModel;

namespace XcodeProjectParser
{
	public class PBXFileReference : IPBXElement
	{
		public IsaType ObjectType {
			get {
				return IsaType.PBXFileReference;
			}
		}

		public string ID { get; set; }

		[Description ("lastKnownFileType")]
		public PBXFileType FileType { get; set; }

		[Description ("fileEncoding")]
		public PBXFileEncoding FileEncoding { get; set; }

		string path;

		[Description ("path")]
		public string Path { 
			get {
				return path;
			}
			set {
				path = value.Replace ("\"", string.Empty);
			}
		}

		[Description ("name")]
		public string Name { get; set; }

		[Description ("sourceTree")]
		public PBXSourceTree SourceTree { get; set; }
	}
}
