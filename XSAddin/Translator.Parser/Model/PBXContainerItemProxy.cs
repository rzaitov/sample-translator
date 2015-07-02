using System;
using System.ComponentModel;
using System.Reflection;

namespace Translator.Parser
{
	public class PBXContainerItemProxy : IPBXElement
	{
		public IsaType ObjectType {
			get {
				return IsaType.PBXBuildFile;
			}
		}

		public string ID { get; set; }

		[Description ("containerPortal")]
		public string ContainerPortal { get; set; }

		[Description ("proxyType")]
		public int ProxyType { get; set; }

		[Description ("remoteGlobalIDString")]
		public string RemoteGlobalIDString { get; set; }

		[Description ("remoteInfo")]
		public string RemoteInfo { get; set; }
	}
}
