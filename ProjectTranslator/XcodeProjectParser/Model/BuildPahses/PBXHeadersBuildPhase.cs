namespace XcodeProjectParser
{
	public class PBXHeadersBuildPhase: PBXBuildPhaseBase
	{
		public override IsaType ObjectType {
			get {
				return IsaType.PBXHeadersBuildPhase;
			}
		}
	}
}

