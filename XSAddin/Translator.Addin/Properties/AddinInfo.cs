using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin (
	"ProjectTranslator",
	Namespace = "Translator.Addin",
	Category = "Project Import and Export",
	Url = "http://xamarin.com/",
	Version = "1.0"
)]

[assembly:AddinName ("ProjectTranslator")]
[assembly:AddinCategory ("Project Import and Export")]
[assembly:AddinDescription ("Convert Xcode project to Xamarin.iOS project")]
[assembly:AddinAuthor ("Oleg Demchenko & Rustam Zaitov")]

[assembly:ImportAddinAssembly("Microsoft.CodeAnalysis.CSharp.Workspaces.dll")]

