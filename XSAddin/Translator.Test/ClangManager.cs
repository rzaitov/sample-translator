using System;
using System.IO;

using Translator.Core;
using ClangSharp;

namespace Translator.Test
{
	public class ClangManager : IDisposable
	{
		CXIndex? index;
		readonly ClangArgBuilder argBuilder;

		string tmpDir;
		public string TmpFolder {
			get {
				if (tmpDir == null) {
					tmpDir = Path.Combine (Path.GetTempPath (), Path.GetRandomFileName ());
					Directory.CreateDirectory (tmpDir);
				}

				return tmpDir;
			}
		}

		public XCodeConfiguration XCode { get; private set; }

		public ClangManager ()
		{
			var xcodeLocator = new XCodeLocator ();
			var criteria = new XCodeCriteria {
				Max = new VersionInfo {
					Major = 7
				}
			};
			string xcodePath = xcodeLocator.FindXCode (criteria);
			XCode = new XCodeConfiguration (xcodePath);

			argBuilder = new ClangArgBuilder {
				PathToFrameworks = XCode.IPhoneSimulator.Frameworks,
				SimMinVersion = XCode.IPhoneSimulator.SdkVersion,
				SysRoot = XCode.IPhoneSimulator.SdkPath,
			};
		}

		public string StoreFile (string content, string fileName)
		{
			string filePath = Path.Combine (TmpFolder, fileName);
			File.WriteAllText (filePath, content);

			return filePath;
		}

		public string[] FetchClangArguments ()
		{
			return argBuilder.Build ();
		}

		public CXTranslationUnit BuildTranslationUnit (string path)
		{
			if(!index.HasValue)
				index = clang.createIndex (1, 1);

			CXUnsavedFile unsavedFiles;
			const uint options = (uint)CXTranslationUnit_Flags.CXTranslationUnit_DetailedPreprocessingRecord;

			string[] args = FetchClangArguments ();
			return clang.parseTranslationUnit (index.Value, path, args, args.Length, out unsavedFiles, 0, options);
		}

		#region IDisposable implementation

		public void Dispose ()
		{
			if (tmpDir != null)
				Directory.Delete (tmpDir);

			if (index.HasValue) {
				clang.disposeIndex (index.Value);
				index = null;
			}
		}

		#endregion
	}
}

