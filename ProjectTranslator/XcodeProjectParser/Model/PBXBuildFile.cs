using System.ComponentModel;

namespace XcodeProjectParser
{
	public class PBXBuildFile : IPBXElement
	{
		public IsaType ObjectType {
			get {
				return IsaType.PBXBuildFile;
			}
		}

		public string ID { get; set; }

		[Description ("fileRef")]
		public string FileRef { get; set; }
	}
}
