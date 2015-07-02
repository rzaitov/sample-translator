[ST] Source Translator
======================
The main goal of this tool is to help translate ObjC projects to C#. Tool will analyse the provided xcode's project source files and will generate corresponding C# files. The generated classes contain properties and methods from original ObjC classes with bodies as inline comments.

The tool is implemented as Xamarin Studio addin. After tool completes project translation Xamarin Studio will open the generated solution.

Design
======
We implemented our **ST** tool as Xamarin Studion addin. It uses **libclang** to build AST for provided sources. Then it traverses AST to generate C# code with **Roslyn**. Also it uses **Mono.Cecil** to inspect installed **Xamarin.iOS** assembly.

Features
========
* **ST** handles iOS project types gracefully. It will generate C# project with correct ProjectType Guid for regular iOS application, WatchKit and Extension applications.
* **ST** will copy all Resources from original project (Storyboards, Image assets, Launch screen)
* **ST** knows about Xamarin.iOS, so it inferres correct signature for bounded APIs.

Limitations
===========
* You have to have XCode6 on your Mac (doesn't work with XCode7)
* Wokrs with iOS project only (will add Mac projects soon)

How to use ST
=============
Here is no binary, so you have to build and run from sources. Because this is an Xamarin Studio addin, you need to install `Addin Maker` first.

* Open XS
* Xamarin Studio > Add-in Manager... > Gallery tab > Addin Development node > Install button
 
Run `Translator.Addin` project. New instance of Xamarin Studio will launch.

* Tools > Convert XCode project.

Use following steps to convert Xcode project:

* Select Xcode project *.xcodeproj file
* Select output C# project path
* Decide whether or not Xcode project assets should be overwritten with Xamarin application icons and launch images
* Press **Analyze Xcode project** button to figure out the structure of Xcode project and create its intermediate representation
* Press **Generate C# project** button to write project files to file system
* Xamarin Studio will open generated solution for you

If you porting project which has external dependensies (e.g. Pods) then you have to manually provide additional **Header Search Paths**. Put path to directory with headers into **Header Search Paths** field. If you have multiple folders then use semicolon (**;**) as separator

Road map
========
* Handle Mac project
* Handle method's bodies
* Handle Swift (awaiting announced Swift opensourcing)
