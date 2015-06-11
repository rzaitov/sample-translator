namespace XcodeProjectParser
{
	public class PBXGroup : PBXGroupBase
	{
		public override IsaType ObjectType {
			get {
				return IsaType.PBXGroup;
			}
		}
	}
}
