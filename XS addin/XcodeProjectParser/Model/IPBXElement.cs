namespace XcodeProjectParser
{
	public interface IPBXElement
	{
		string ID { get; set; }

		IsaType ObjectType { get; }
	}
}
