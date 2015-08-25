# JScript.NET
Currently we have , thanks to node.js, possibility to build server-side code in Java Script. Thanks to Metro we have possibility to write mabapps in javaScript.
But did you know that since .NET Framework has been created we always have had possibility to write server-side code in JavaScript, and also to write console applications in JavaScript and even dektop GUI applications (WinForms)?
Microsoft developed a JavaScript-based language called JScript.NET.
The language has a lot of common with JavaScript but it also has extra features borrowed from newer versions of ECMA script (like classes, namespaces, static functions etc.).
The language is compiled to .NET Framework's CIL.
It gives you possibility to use large set .NET libraries form JavaScript, and it also gives you possibility to write DLLs in JavaScript and later use them from Visual Basic, C# or even PHP.
Unfortunatelly the language never got much attention.
Despite huge effort it must have cost Microsoft to develop the language it was never advertised and never got proper tooling support.
It is possible to choose JavaScript as language for ASP.NET WebForms. JSC compiler is being silently shipped with .NET Framework.
But it has not been  possible to create a JavaScript project in Visual Studio and to use Visual Studio to develop applications and libraries in JavaScript.
Dispite there is still better and better support for JavaScript in Visual Studio the support is limited to web and Metro.


I beleive that the possibility to write EXE applications and DLLs in JavaScript should not be available only to crazy people willing to compile from command line and type in Notepad(++).
This this project.

This project adds JScript.NET project to Visual Studio, to allow you to build JavaScript .NET applications.
So far the project is in very erly stage, but it already allows you to wrtite, run and debug console application in Visual Studio.

## Capabilities
* Write JavaScript EXE and DLL applications in Visual Studio
* Compile them
* Debug them
* Reference DLLs written in JavaScript form other .NET languages (Visual Basic, C#, C++/CLI, PHP, F#)
* Reference DLLs written in other .NET languages from JavaScript

## Known limitations
* Many changes currently cannot be done via Visual Studio, you must unload the project and edit project file manualy
** E.g. to change project type from console application to DLL look for <OutputType> element cnd change its value to library (for GUI napplication change it to winexe).
* Not all JSC options are supported
* Impossible to reference JScript.NET project from C#. As a workaround use DLL reference instead.
* VS editor is reporting lot of false warnings and errors.
* Impossible to add new files to project via right-click | Add | New item
** Copy-paste the default item instead, or create the item outside of Visual Studio and then include it in a project.

Download from VS Gallery https://visualstudiogallery.msdn.microsoft.com/b45da343-89fd-4359-84ac-8530fbc14890 or install directly from VS

This is sister project of ILProj https://github.com/DzonnyDZ/ILProj