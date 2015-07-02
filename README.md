[PT] Project Translator
======================
The main goal of this tool is to translate ObjC projects to C#. Tool will analyses the provided Xcode project and its source files then it will generate corresponding Xamarin Studio project with C# files. Generated C# class contain properties and methods like in original ObjC class with method bodies as inline comments. After tool completes project translation Xamarin Studio will open the generated solution automatically.

Design
======
**Project Translator** implemented as Xamarin Studio addin. It uses **libclang** to build AST for provided sources. Then it traverses AST to generate C# code with **Roslyn**. Also it uses **Mono.Cecil** to inspect installed **Xamarin.iOS** assembly.

Features
========
* **PT** handles iOS project types gracefully. It will generate C# project with correct ProjectType Guid for regular application for iOS, Apple Watch and App Extensions.
* **PT** will copy resources from original project and set correct Build Action for them. Supported resource types: Storyboards, Image assets, Images, Metal files, Open GL, SceneKit assets.
* **PT** knows about Xamarin.iOS, so it inferres correct signature for bounded APIs.

Limitations
===========
* You must have XCode 6 installed on your Mac (tool doesn't yet support XCode 7)
* Works with iOS project only (Mac projects coming soon)

How to use PT
=============
First you need to install `Addin Maker` then you can build and run tool from sources.

To install `Addin Maker`:
* Open XS
* Xamarin Studio > Add-in Manager... > Gallery tab > Addin Development node > Install button
 
Run `Translator.Addin` project. New instance of Xamarin Studio will launch.

Select in main menu:
* Tools > Project Translator

Use following steps to convert Xcode project:

* Select Xcode project *.xcodeproj file you want to convert
* Select output C# project path
* Decide whether or not Xcode project assets should be overwritten with Xamarin application icons and launch images
* Press **Analyze Xcode project** button to figure out the structure of Xcode project and create its intermediate representation
* Press **Generate C# project** button to write project files to file system
* Xamarin Studio will open generated solution for you

If you translating project which has external dependencies (e.g. Pods) then you have to manually provide additional **Header Search Paths**. Put path to directory with headers into **Header Search Paths** field. If you have multiple folders then use semicolon (**;**) as separator

Road map
========
* Translate Mac project
* Handle method's bodies
* Handle Swift (awaiting announced Swift opensourcing)
* Create addin distribution package