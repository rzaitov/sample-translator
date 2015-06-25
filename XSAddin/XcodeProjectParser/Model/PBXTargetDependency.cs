using System;
using System.ComponentModel;

namespace Translator.Parser
{
	public class PBXTargetDependency : IPBXElement
	{
		public IsaType ObjectType {
			get {
				return IsaType.PBXTargetDependency;
			}
		}

		public string ID { get; set; }

		[Description ("target")]
		public string Target { get; set; }

		[Description ("targetProxy")]
		public string TargetProxy { get; set; }
	}
}

