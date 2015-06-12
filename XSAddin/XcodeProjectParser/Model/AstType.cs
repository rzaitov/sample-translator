using System;
using System.ComponentModel;
using System.Reflection;

namespace XcodeProjectParser
{
	public enum IsaType
	{
		None = 0,

		[CorrespondingClass (typeof(PBXBuildFile))]
		PBXBuildFile,

		[CorrespondingClass (typeof(PBXCopyFilesBuildPhase))]
		PBXCopyFilesBuildPhase,

		[CorrespondingClass (typeof(PBXFrameworksBuildPhase))]
		PBXFrameworksBuildPhase,

		[CorrespondingClass (typeof(PBXHeadersBuildPhase))]
		PBXHeadersBuildPhase,

		[CorrespondingClass (typeof(PBXResourcesBuildPhase))]
		PBXResourcesBuildPhase,

		[CorrespondingClass (typeof(PBXBuildFile))]
		PBXShellScriptBuildPhase,

		[CorrespondingClass (typeof(PBXSourcesBuildPhase))]
		PBXSourcesBuildPhase,

		[CorrespondingClass (typeof(PBXContainerItemProxy))]
		PBXContainerItemProxy,

		[CorrespondingClass (typeof(PBXFileReference))]
		PBXFileReference,

		[CorrespondingClass (typeof(PBXGroup))]
		PBXGroup,

		[CorrespondingClass (typeof(PBXVariantGroup))]
		PBXVariantGroup,

		[CorrespondingClass (typeof(PBXBuildFile))]
		PBXAggregateTarget,

		[CorrespondingClass (typeof(PBXNativeTarget))]
		PBXNativeTarget,

		[CorrespondingClass (typeof(PBXProject))]
		PBXProject,

		[CorrespondingClass (typeof(PBXTargetDependency))]
		PBXTargetDependency,

		[CorrespondingClass (typeof(XCBuildConfiguration))]
		XCBuildConfiguration,

		[CorrespondingClass (typeof(PBXBuildFile))]
		XCConfigurationList
	}

	[AttributeUsage (AttributeTargets.Field)]
	public class CorrespondingClass : Attribute
	{
		public Type UnderlyingType { get; private set; }

		public CorrespondingClass (Type type)
		{
			UnderlyingType = type;
		}
	}

	public static class Extensions
	{
		public static string NormalizePath (this string path)
		{
			return path.Replace (@"/", @"\");
		}

		public static IPBXElement CreatePBXObject (this string str)
		{
			var type = typeof(IsaType);
			var memberInfo = type.GetMember (str);

			if (memberInfo.Length == 0)
				return null;

			var attributes = memberInfo [0].GetCustomAttributes (typeof(CorrespondingClass), false);
			var underlyingType = ((CorrespondingClass)attributes [0]).UnderlyingType;
			return (IPBXElement)Activator.CreateInstance (underlyingType);
		}

		public static T GetEnumMemberByDescription<T> (this string str)
		{
			var type = typeof(T);
			var membersInfo = type.GetMembers ();

			foreach (var member in membersInfo) {
				var attributes = member.GetCustomAttributes (typeof(DescriptionAttribute), false);
                
				if (attributes.Length == 0)
					continue;

				var description = ((DescriptionAttribute)attributes [0]).Description;

				if (description == str)
					return (T)Enum.Parse (typeof(T), member.Name);
			}

			return default(T);
		}

		public static void SetPropertyByDescription<T> (IPBXElement element, string propertyDescription, T value)
		{
			var membersInfo = element.GetType ().GetMembers ();

			foreach (var member in membersInfo) {
				var attributes = member.GetCustomAttributes (typeof(DescriptionAttribute), false);
                
				if (attributes.Length == 0)
					continue;

				var description = ((DescriptionAttribute)attributes [0]).Description;
				if (description == propertyDescription) {
					var methodInfo = ((PropertyInfo)member).GetSetMethod ();
					var parameters = methodInfo.GetParameters ();

					if (parameters.Length != 0) {
						var parameterType = parameters [0].ParameterType;
						var parameterValue = value;
						methodInfo.Invoke (element, new object[] { parameterValue });
						break;
					}
				}
			}
		}

		public static T ConvertString<T> (this string str)
		{
			try {
				return (T)Convert.ChangeType (str, typeof(T));
			} catch {
				return default(T);
			}
		}

		public static string ConvertPathToSharpName (this string path)
		{
			return System.IO.Path.GetFileNameWithoutExtension (path) + ".cs";
		}
	}
}
