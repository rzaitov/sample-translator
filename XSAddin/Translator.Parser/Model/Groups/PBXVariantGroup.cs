namespace Translator.Parser
{
    public class PBXVariantGroup : PBXGroupBase
    {
        public override IsaType ObjectType {
            get {
                return IsaType.PBXVariantGroup;
            }
        }
    }
}
