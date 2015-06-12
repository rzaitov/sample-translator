namespace XcodeProjectParser
{
	public class PBXSourcesBuildPhase : PBXResourcesBuildPhase
	{
		public override IsaType ObjectType {
			get {
				return IsaType.PBXSourcesBuildPhase;
			}
		}
	}
}
