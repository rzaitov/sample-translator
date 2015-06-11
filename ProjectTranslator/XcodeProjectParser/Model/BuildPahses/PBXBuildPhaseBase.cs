using System.Collections.Generic;
using System.ComponentModel;

namespace XcodeProjectParser
{
	public class PBXBuildPhaseBase : IPBXElement
	{
		public virtual IsaType ObjectType {
			get {
				return IsaType.None;
			}
		}

		public string ID { get; set; }

		[Description ("runOnlyForDeploymentPostprocessing")]
		public int RunOnlyForDeploymentPostprocessing { get; set; }

		[Description ("files")]
		public IList<string> Files { get; set; }

		[Description ("buildActionMask")]
		public int BuildActionMask { get; set; }

		public PBXBuildPhaseBase ()
		{
			Files = new List<string> ();
		}
	}
}
