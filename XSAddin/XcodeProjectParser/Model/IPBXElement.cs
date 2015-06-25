namespace Translator.Parser
{
	public interface IPBXElement
	{
		string ID { get; set; }

		IsaType ObjectType { get; }
	}
}
