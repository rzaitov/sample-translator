namespace Translator.Core
{
	public class XCodeConfiguration
	{
		public string XCodePath { get; private set; }

		public PlatformConfiguration IPhoneSimulator { get; private set; }
		public PlatformConfiguration IPhone { get; private set; }

		public XCodeConfiguration (string xcodePath)
		{
			XCodePath = xcodePath;
			IPhoneSimulator = new PlatformConfiguration (XCodePath, PlatformsName.IPhoneSimulator);
			IPhone = new PlatformConfiguration (XCodePath, PlatformsName.IPhone);
		}
	}
}