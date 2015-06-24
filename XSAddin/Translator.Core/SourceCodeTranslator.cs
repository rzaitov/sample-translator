using System;
using System.IO;

using ClangSharp;

namespace Translator.Core
{
	public class SourceCodeTranslator
	{
		readonly CXIndex index = clang.createIndex (1, 1);
		readonly string[] clangArgs;
		readonly IBindingLocator bindingLocator;

		public SourceCodeTranslator (string[] args, IBindingLocator bindingLocator)
		{
			clangArgs = args;
			this.bindingLocator = bindingLocator;
		}

		public void Translate (string pathToSrc, string namespaceName, TextWriter writer)
		{
			CXCursor tu = GetTranslationUnit (pathToSrc);
			TranslationUnitPorter porter = new TranslationUnitPorter (tu, namespaceName, bindingLocator);
			writer.Write (porter.Generate ());
		}

		public CXCursor GetTranslationUnit (string pathToSrc)
		{
			CXUnsavedFile unsavedFiles;
			CXTranslationUnit translationUnit = clang.parseTranslationUnit(index, pathToSrc, clangArgs, clangArgs.Length, out unsavedFiles, 0, 0);
			if (translationUnit.Pointer == IntPtr.Zero)
				throw new InvalidOperationException ();

			return clang.getTranslationUnitCursor (translationUnit);
		}
	}
}