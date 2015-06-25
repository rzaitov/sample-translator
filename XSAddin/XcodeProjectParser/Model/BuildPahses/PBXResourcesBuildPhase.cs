namespace Translator.Parser
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
