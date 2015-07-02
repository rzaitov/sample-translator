using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Translator.Parser
{
	public class PBXProject : IPBXElement
	{
		public IsaType ObjectType {
			get {
				return IsaType.PBXProject;
			}
		}

		public string ID { get; set; }

		[Description ("buildConfigurationList")]
		public string BuildConfigurationList { get; set; }

		[Description ("compatibilityVersion")]
		public string CompatibilityVersion { get; set; }

		[Description ("developmentRegion")]
		public string DevelopmentRegion { get; set; }

		[Description ("hasScannedForEncodings")]
		public int HasScannedForEncodings { get; set; }

		[Description ("knownRegions")]
		public IList<string> KnownRegions { get; set; }

		[Description ("mainGroup")]
		public string MainGroup { get; set; }

		[Description ("projectDirPath")]
		public string ProjectDirPath { get; set; }

		[Description ("projectRoot")]
		public string ProjectRoot { get; set; }

		[Description ("targets")]
		public IList<string> Targets { get; set; }

		public PBXProject ()
		{
			Targets = new List<string> ();
			KnownRegions = new List<string> ();
		}
	}
}
