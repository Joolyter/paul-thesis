# JoolyterIDE

*Joolyter IDE* was developed for .NET Framework 4.7.2 in Visual Studio 2019.

## Tree of JoolyterIDE

├───docs

│   ├───html

│   │   └───search

│   ├───latex

│   └───src_copy

├───Joolyter.KSP

│   ├───bin

│   ├───obj

│   └───Properties

├───Joolyter.Unity

│   ├───Android

│   ├───bin

│   ├───Functions

│   │   ├───Android

│   │   ├───Handlers

│   │   └───SimpleFileBrowser

│   ├───Interfaces

│   ├───obj

│   ├───Properties

├───Unity

│   └───Joolyter

│       ├───AssetBundles

│       ├───Assets

│       │   ├───Fonts

│       │   ├───PartTools

│       │   ├───Plugins

│       │   │   ├───Joolyter

│       │   │   │   ├───Plugin

│       │   │   │   └───Ressources

│       │   │   │       └───Images

│       │   │   ├───KSPAssets

│       │   │   └───SimpleFileBrowser

│       │   │       ├───Prefabs

│       │   │       ├───Resources

│       │   │       ├───Skins

│       │   │       └───Sprites

│       │   │           └───FileIcons

│       │   ├───Scenes

│       │   ├───SquadCore

│       │   ├───TextMesh Pro

│       │   └───XML

│       ├───Library

│       ├───Logs

│       ├───Packages

│       ├───ProjectSettings

│       └───TestBuild

└───Joolyter.sln

## Opening Project in VS

After setting up Visual Studio as specified in the thesis the C# project can be launched by opening *Joolyter.sln*. You may need to reset the dependencies. All linked DLLs can be found in Kerbal Space Programs installation folder in *./KSP_x64_Data/Managed/*.

## Opening Unity Project

After Unity's installation the project can be opened using Unity Hub. After clicking on *Open* select *./Unity/Joolyter/* and proceed.

## JoolyterIDE Documentation

The documentation of JoolyterIDE was created with **doxygen**. It is available under [Doxygen: Downloads](https://doxygen.nl/download.html).

The configuration file for **doxygen** is included in the documentation folder.

A latex version of the documentation is saved in the folder *latex*. In the *html* the documentation in the respective format can be found.

To exclude C# files inside the Unity Project folder a copy of the source code that is to be documented is placed in *./docs/src_copy*.