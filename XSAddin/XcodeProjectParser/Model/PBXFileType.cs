using System.ComponentModel;

namespace XcodeProjectParser
{
	public enum PBXFileType {
		None = 0,
		
		[Description ("archive.ar")]
		archiveAr,
		
		[Description ("archive.asdictionary")]
		ArchiveAsdictionary,
		
		[Description ("archive.binhex")]
		ArchiveBinhex,
		
		[Description ("archive.ear")]
		ArchiveEar,
		
		[Description ("archive.gzip")]
		ArchiveGzip,
		
		[Description ("archive.jar")]
		ArchiveJar,
		
		[Description ("archive.macbinary")]
		ArchiveMacbinary,
		
		[Description ("archive.ppob")]
		ArchivePpob,
		
		[Description ("archive.rsrc")]
		ArchiveRsrc,
		
		[Description ("archive.stuffit")]
		ArchiveStuffit,
		
		[Description ("archive.tar")]
		ArchiveTar,
		
		[Description ("archive.war")]
		ArchiveWar,
		
		[Description ("archive.zip")]
		ArchiveZip,
		
		[Description ("audio.aiff")]
		AudioAiff,
		
		[Description ("audio.au")]
		AudioAu,
		
		[Description ("audio.midi")]
		AudioMidi,
		
		[Description ("audio.mp3")]
		AudioMp3,
		
		[Description ("audio.wav")]
		AudioWav,
		
		[Description ("compiled.mach-o")]
		CompiledMachO,
		
		[Description ("compiled.mach-o.bundle")]
		CompiledMachOBundle,
		
		[Description ("compiled.mach-o.corefile")]
		CompiledMachOCorefile,
		
		[Description ("compiled.mach-o.dylib")]
		CompiledMachODylib,
		
		[Description ("compiled.mach-o.fvmlib")]
		CompiledMachOFvmlib,
		
		[Description ("compiled.mach-o.objfile")]
		CompiledMachOObjfile,
		
		[Description ("compiled.mach-o.preload")]
		CompiledMachOPreload,
		
		[Description ("file.bplist")]
		FileBplist,

		[Description ("file.scp")]
		FileScp,
		
		[Description ("file.xib")]
		FileXib,

		[Description ("file.storyboard")]
		FileStoryboard,
		
		[Description ("image.bmp")]
		ImageBmp,
		
		[Description ("image.gif")]
		ImageGif,
		
		[Description ("image.icns")]
		ImageIcns,
		
		[Description ("image.ico")]
		ImageIco,
		
		[Description ("image.jpeg")]
		ImageJpeg,
		
		[Description ("image.pdf")]
		ImagePdf,
		
		[Description ("image.pict")]
		ImagePict,
		
		[Description ("image.png")]
		ImagePng,
		
		[Description ("image.tiff")]
		ImageTiff,
		
		[Description ("pattern.proxy")]
		PatternProxy,
		
		[Description ("sourcecode.ada")]
		SourcecodeAda,
		
		[Description ("sourcecode.applescript")]
		SourcecodeApplescript,
		
		[Description ("sourcecode.asm")]
		SourcecodeAsm,
		
		[Description ("sourcecode.asm.asm")]
		SourcecodeAsmAsm,
		
		[Description ("sourcecode.asm.llvm")]
		SourcecodeAsmLlvm,
		
		[Description ("sourcecode.c")]
		SourcecodeC,
		
		[Description ("sourcecode.c.c.preprocessed")]
		SourcecodeCCPreprocessed,
		
		[Description ("sourcecode.c.h")]
		SourcecodeCH,
		
		[Description ("sourcecode.c.objc")]
		SourcecodeCObjc,
		
		[Description ("sourcecode.c.objc.preprocessed")]
		SourcecodeCObjcPreprocessed,
		
		[Description ("sourcecode.cpp.cpp")]
		SourcecodeCppCpp,
		
		[Description ("sourcecode.cpp.cpp.preprocessed")]
		SourcecodeCppCppPreprocessed,
		
		[Description ("sourcecode.cpp.h")]
		SourcecodeCppH,
		
		[Description ("sourcecode.cpp.objcpp")]
		SourcecodeCppObjcpp,

		[Description ("sourcecode.metal")]
		SourcecodeMetal,
		
		[Description ("sourcecode.cpp.objcpp.preprocessed")]
		SourcecodeCppObjcppPreprocessed,
		
		[Description ("sourcecode.dtrace")]
		SourcecodeDtrace,
		
		[Description ("sourcecode.exports")]
		SourcecodeExports,
		
		[Description ("sourcecode.fortran")]
		SourcecodeFortran,
		
		[Description ("sourcecode.fortran.f77")]
		SourcecodeFortranF77,
		
		[Description ("sourcecode.fortran.f90")]
		SourcecodeFortranF90,
		
		[Description ("sourcecode.glsl")]
		SourcecodeGlsl,
		
		[Description ("sourcecode.jam")]
		SourcecodeJam,
		
		[Description ("sourcecode.java")]
		SourcecodeJava,
		
		[Description ("sourcecode.javascript")]
		SourcecodeJavascript,
		
		[Description ("sourcecode.lex")]
		SourcecodeLex,
		
		[Description ("sourcecode.make")]
		SourcecodeMake,
		
		[Description ("sourcecode.mig")]
		SourcecodeMig,
		
		[Description ("sourcecode.nasm")]
		SourcecodeNasm,
		
		[Description ("sourcecode.opencl")]
		SourcecodeOpencl,
		
		[Description ("sourcecode.pascal")]
		SourcecodePascal,
		
		[Description ("sourcecode.rez")]
		SourcecodeRez,
		
		[Description ("sourcecode.yacc")]
		SourcecodeYacc,
		
		[Description ("text")]
		Text,
		
		[Description ("text.css")]
		TextCss,
		
		[Description ("text.html.documentation")]
		TextHtmlDocumentation,
		
		[Description ("text.man")]
		TextMan,
		
		[Description ("text.pbxproject")]
		TextPbxproject,
		
		[Description ("text.plist")]
		TextPlist,
		
		[Description ("text.plist.info")]
		TextPlistInfo,
		
		[Description ("folder.assetcatalog")]
		FolderAssetCatalog,

		[Description ("folder.skatlas")]
		FolderAssetAtlas,
		
		[Description ("text.plist.scriptTerminology")]
		TextPlistScriptTerminology,
		
		[Description ("text.plist.strings")]
		TextPlistStrings,
		
		[Description ("text.plist.xclangspec")]
		TextPlistXclangspec,
		
		[Description ("text.plist.xcsynspec")]
		TextPlistXcsynspec,
		
		[Description ("text.plist.xctxtmacro")]
		TextPlistXctxtmacro,
		
		[Description ("text.plist.xml")]
		TextPlistXml,
		
		[Description ("text.rtf")]
		TextRtf,
		
		[Description ("text.script")]
		TextScript,
		
		[Description ("text.script.csh")]
		TextScriptCsh,
		
		[Description ("text.script.perl")]
		TextScriptPerl,
		
		[Description ("text.script.php")]
		TextScriptPhp,
		
		[Description ("text.script.python")]
		TextScriptPython,
		
		[Description ("textScript.ruby")]
		TextScriptRuby,
		
		[Description ("text.scriptSh")]
		TextScriptSh,
		
		[Description ("text.script.worksheet")]
		TextScriptWorksheet,
		
		[Description ("text.xcconfig")]
		TextXcconfig,
		
		[Description ("text.xml")]
		TextXml,

		[Description ("text.xml.dae")]
		TextXmlDae,
		
		[Description ("video.avi")]
		VideoAvi,
		
		[Description ("video.mpeg")]
		VideoMpeg,
		
		[Description ("video.quartz-composer")]
		VideoQuartzComposer,
		
		[Description ("video.quicktime")]
		VideoQuicktime,
		
		[Description ("wrapper.application")]
		WrapperApplication,
		
		[Description ("wrapper.cfbundle")]
		WrapperCFBundle,
		
		[Description ("wrapper.framework")]
		WrapperFramework,
		
		[Description ("wrapper.pb-project")]
		WrapperPBProject
	}
}