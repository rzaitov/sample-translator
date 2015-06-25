using System.ComponentModel;

namespace Translator.Parser
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
