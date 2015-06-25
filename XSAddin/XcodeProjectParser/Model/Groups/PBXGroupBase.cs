using System.Collections.Generic;
using System.ComponentModel;

namespace Translator.Parser
{
	public class PBXGroupBase : IPBXElement
	{
		public virtual IsaType ObjectType {
			get {
				return IsaType.None;
			} 
		}

		public string ID { get; set; }

		[Description ("name")]
		public string Name { get; set; }

		[Description ("children")]
		public IList<string> Children { get; set; }

		[Description ("sourceTree")]
		public PBXSourceTree SourceTree { get; set; }

		[Description ("path")]
		public string Path { get; set; }

		public PBXGroupBase ()
		{
			Children = new List<string> ();
		}
	}
}
