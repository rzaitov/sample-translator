using System.ComponentModel;

namespace Translator.Parser
{
	public enum PBXSourceTree {
		None = 0,
		
		[Description ("<absolute>")]
		Absolute,
		
		[Description ("<group>")]
		Group,
		
		[Description ("SOURCE_ROOT")]
		SourceRoot,
		
		[Description ("BUILT_PRODUCTS_DIR")]
		BuildProductDir,
		
		[Description ("SDKROOT")]
		SdkRoot
	}
}