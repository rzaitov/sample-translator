using System.Collections.Generic;
using System.ComponentModel;

namespace XcodeProjectParser
{
	public class XCConfigurationList : IPBXElement
	{
		public IsaType ObjectType {
			get {
				return IsaType.XCBuildConfiguration;
			}
		}

		public string ID { get; set; }

		[Description ("defaultConfigurationName")]
		public string DefaultConfigurationName { get; set; }

		[Description ("defaultConfigurationIsVisible")]
		public int DefaultConfigurationIsVisible { get; set; }

		[Description ("buildConfigurations")]
		public IList<string> BuildConfigurations { get; set; }

		public XCConfigurationList ()
		{
			BuildConfigurations = new List<string> ();
		}
	}
}
