namespace XcodeProjectParser
{
	public class PBXResourcesBuildPhase : PBXBuildPhaseBase
	{
		public override IsaType ObjectType {
			get {
				return IsaType.PBXResourcesBuildPhase;
			}
		}
	}
}
