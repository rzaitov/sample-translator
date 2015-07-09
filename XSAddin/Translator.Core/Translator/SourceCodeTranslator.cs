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

		public void Translate (string pathToSrc, TranslatorOptions options, TextWriter writer)
		{
			CXCursor tu = GetTranslationUnit (pathToSrc);
			var porter = new TranslationUnitPorter (tu, options, bindingLocator);
			writer.Write (porter.Generate ());
		}

		public CXCursor GetTranslationUnit (string pathToSrc)
		{
			CXUnsavedFile unsavedFiles;
			uint options = (uint)CXTranslationUnit_Flags.CXTranslationUnit_DetailedPreprocessingRecord;
			CXTranslationUnit translationUnit = clang.parseTranslationUnit(index, pathToSrc, clangArgs, clangArgs.Length, out unsavedFiles, 0, options);
			if (translationUnit.Pointer == IntPtr.Zero)
				throw new InvalidOperationException ();

			return clang.getTranslationUnitCursor (translationUnit);
		}
	}
}