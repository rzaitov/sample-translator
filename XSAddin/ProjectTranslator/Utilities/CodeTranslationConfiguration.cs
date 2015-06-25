using System.Collections.Generic;

namespace Translator.Addin
{
	public class CodeTranslationConfiguration
	{
		string projectNamespace;
		public string ProjectNamespace {
			get {
				return projectNamespace;
			}
			set {
				projectNamespace = value.Replace (" ", string.Empty);
			}
		}

		public string ProjectPath { get; set; }

		public IList<string> FilePaths { get; set; }

		public IList<string> Frameworks { get; set; }

		public CodeTranslationConfiguration ()
		{
			FilePaths = new List<string> ();
			Frameworks = new List<string> ();
		}

		public string FilesToString ()
		{
			string files = string.Empty;
			foreach (string filePath in FilePaths)
				files += string.Format ("\"{0}\" ", filePath);

			return files;
		}

		public string FramewroksToString ()
		{
			string frameworks = string.Empty;
			foreach (string framework in Frameworks)
				frameworks += string.Format (" -framework {0}", framework);

			return frameworks;
		}
	}
}

