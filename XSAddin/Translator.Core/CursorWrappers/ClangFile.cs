using System;
using System.Runtime.InteropServices;

using ClangSharp;

namespace Translator.Core
{
	public class ClangFile : IEquatable<ClangFile>
	{
		IntPtr handle;

		string path;
		public string Path {
			get {
				if (path == null) {
					var nativeStr = GetFileName (handle);
					path = nativeStr.ToString ();
				}

				return path;
			}
		}

		public string FileName {
			get {
				return System.IO.Path.GetFileName (FileName);
			}
		}

		public ClangFile (CXFile file)
			: this (file.Pointer)
		{
		}

		public ClangFile (IntPtr handle)
		{
			this.handle = handle;
		}

		public override int GetHashCode ()
		{
			return handle.GetHashCode ();
		}

		public override bool Equals (object obj)
		{
			Type type = obj.GetType ();
			if (type != typeof (ClangFile))
				return false;

			return Equals ((ClangFile)obj);
		}

		public bool Equals (ClangFile other)
		{
			return IsEqual (handle, other.handle) > 0;
		}

		[DllImport ("libclang.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_File_isEqual")]
		public static extern int IsEqual (IntPtr file1, IntPtr file2);

		[DllImport ("libclang.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getFileName")]
		public static extern CXString GetFileName (IntPtr file);
	}
}
