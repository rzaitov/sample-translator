namespace Translator.Parser
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
