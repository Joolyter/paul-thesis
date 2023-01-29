# Joolyter Help Sheet [as per Jan 29, 2023]
This is a incomplete collection of helpful links, tips, and informations that have proven themselves helpful in the course of the usage of Joolyter. This sheet is inteded to be expanded as new information is found.
All documentations created by myself can be accessed on https://joolyter.github.io/paul-thesis/.

## 1. KSP Mission Building
Kerbal Space Program's Mission Builder is part of the DLC *Making History*. If it cannot be found check if the extension is installed.
The demo missions that are included can also be used as templates. Therefore, open the Mission Builder and load the mission by clicking the *Load* button on the top right toolbar and select the mission that is to be loaded.

### Tutorials on Mission Creation
[Kerbal Space Program Blog, Kerbal Space Program: Making History Expansion -... (tumblr.com)](https://kerbaldevteam.tumblr.com/post/171138611984/kerbal-space-program-making-history-expansion)

[KSP Making History Expansion - How to Create with the Mission Builder - YouTube](https://www.youtube.com/watch?v=xyR0iNr-Uug)

The game has guided tutorials as playable missions. They can be found under *Start Game  ->  Play Missions  ->  Stock Missions  ->  Tutorials*.

## 2. Parser Development
Please see the documentation at https://joolyter.github.io/paul-thesis/CoreApplication/src/docs/_build/html/index.html.

For each mission a parser has to be developed. Please read the corresponding section of the thesis to make yourself familiar with the concept.
Initializing the class **Setup** sets up streams for basic telemetry. For further information see the documentation and source code. 
To get to know kRPC it is highly advised to complete the tutorials providied on [Tutorials and Examples — kRPC 0.4.8 documentation](https://krpc.github.io/krpc/tutorials.html).
The [documentation](https://krpc.github.io/krpc/python.html) has also proven itself very helpful.
Further and personal assistance can be found on [kRPC: Remote Procedure Call Server  - Kerbal Space Program Forums](https://forum.kerbalspaceprogram.com/index.php?/topic/130742-15x-to-122-krpc-control-the-game-using-c-c-java-lua-python-ruby-haskell-c-arduino-v048-28th-october-2018/) but kRPC's [Discord server](https://discord.gg/c8c36UM) is much more active.

## 3. Joolyter IDE
Please see the documentation at https://joolyter.github.io/paul-thesis/IDE/docs/html/index.html.

The Joolyter IDE and Joolyter Demo are completely independent. The IDE has been developed to make debugging and execution of solution scripts more convenient on small screen devices. It is written in C# and built in Unity 2019.18.f1.
The developement was a lot more complex and time consuming as anticipated. Thus, if changes are to be made and no knowledge in Unity and Kerbal Space Program exists, I highly recommend to complete DMagic's tutorial on [Unity UI Creation Tutorial - Kerbal Space Program Forums](https://forum.kerbalspaceprogram.com/index.php?/topic/151354-unity-ui-creation-tutorial/) **after** getting comfortable using C# and loading plugins through [Linx' basic video tutorial](https://www.youtube.com/watch?v=i0I7MhOM7mg).
As DMagic uses different mods as basis for his tutorial links to his [GitHub profile](https://github.com/DMagic1) and [threads on the Kerbal Space Program Forum](https://forum.kerbalspaceprogram.com/index.php?/profile/57416-dmagic/content/&type=forums_topic&change_section=1) are added.

**Simple File Browser** is developed by Süleyman Yasir Kula. [Its GitHub repository](https://github.com/yasirkula/UnitySimpleFileBrowser) covers an extensive README file. Furthermore, he offers support on the [Unity Forum](https://forum.unity.com/threads/simple-file-browser-open-source.441908/) and [Discord](https://discord.gg/UJJt549AaV).

Generally the Unity [manual](https://docs.unity3d.com/2019.4/Documentation/Manual/index.html) and [documentation](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/index.html) as well as the [forum](https://forum.unity.com/) offer great support. Tutorials on YouTube have often proven to be a good starting point as well.

## 4. Contact
As I became more and more attatched to the project through the countless hours of fiddling around, I will always do my best to offer support where I can.
