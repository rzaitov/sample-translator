namespace XcodeProjectParser
{
	public class PBXFrameworksBuildPhase : PBXBuildPhaseBase
	{
		public override IsaType ObjectType {
			get {
				return IsaType.PBXFrameworksBuildPhase;
			}
		}
	}
}
