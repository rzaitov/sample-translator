using System.ComponentModel;

namespace XcodeProjectParser
{
	public class XCBuildConfiguration : IPBXElement
	{
		public IsaType ObjectType {
			get {
				return IsaType.XCBuildConfiguration;
			}
		}

		public string ID { get; set; }

		[Description ("baseConfigurationReference")]
		public string BaseConfigurationReference { get; set; }

		[Description ("name")]
		public string Name { get; set; }

		[Description ("CLANG_ENABLE_OBJC_ARC")]
		public string ClangEnableObjcArc { get; set; }

		[Description ("CODE_SIGN_IDENTITY")]
		public string CodeSignIdentity { get; set; }

		[Description ("GCC_WARN_ABOUT_MISSING_FIELD_INITIALIZERS")]
		public string GccWarnAboutMissingFieldInitializers { get; set; }

		[Description ("GCC_WARN_ABOUT_RETURN_TYPE")]
		public string GccWarnAboutReturnType { get; set; }

		[Description ("GCC_WARN_INITIALIZER_NOT_FULLY_BRACKETED")]
		public string GccWarnInitializerNotFullyBrackated { get; set; }

		[Description ("GCC_WARN_SHADOW")]
		public string GccWarnShadow { get; set; }

		[Description ("GCC_WARN_SIGN_COMPARE")]
		public string GccWarnSignCompare { get; set; }

		[Description ("GCC_WARN_UNDECLARED_SELECTOR")]
		public string GccWarnUndeclaredSelector { get; set; }

		[Description ("GCC_WARN_UNUSED_FUNCTION")]
		public string GccWarUnusedFunction { get; set; }

		[Description ("GCC_WARN_UNUSED_LABEL")]
		public string GccWarUnusedLabel { get; set; }

		[Description ("GCC_WARN_UNUSED_VARIABLE")]
		public string GccWarUnusedVariable { get; set; }

		[Description ("IPHONEOS_DEPLOYMENT_TARGET")]
		public string iPhoneOSDeployemntTarget { get; set; }

		[Description ("PROVISIONING_PROFILE")]
		public string ProvivisioningProfile { get; set; }

		[Description ("RUN_CLANG_STATIC_ANALYZER")]
		public string RunClangStaticAnalyzer { get; set; }
	}
}
