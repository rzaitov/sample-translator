using System.Collections.Generic;
using System.ComponentModel;

namespace XcodeProjectParser
{
	public class PBXNativeTarget : IPBXElement
	{
		public IsaType ObjectType {
			get {
				return IsaType.PBXNativeTarget;
			}
		}

		public string ID { get; set; }

		[Description ("buildPhases")]
		public IList<string> BuildPhases { get; set; }

		[Description ("buildRules")]
		public IList<string> BuildRules { get; set; }

		[Description ("dependencies")]
		public IList<string> Dependencies { get; set; }

		[Description ("name")]
		public string Name { get; set; }

		[Description ("productName")]
		public string ProductName { get; set; }

		[Description ("productReference")]
		public string ProductReference { get; set; }

		[Description ("productType")]
		public string ProductType { get; set; }

		public PBXNativeTarget ()
		{
			BuildPhases = new List<string> ();
			BuildRules = new List<string> ();
			Dependencies = new List<string> ();
		}
	}
}
