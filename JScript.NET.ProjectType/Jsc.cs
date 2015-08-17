using System.Diagnostics;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Dzonny.JScriptNet
{
    /// <summary>Wraps JScript.NET compiler as MSBuild task</summary>
   public class Jsc : Task
    {
        /// <summary>Gets or sets name of binary output file</summary>
        /// <value>If not specified output file name is inferred from first file name</value>
        public string Out { get; set; }
        /// <summary>Gets or sets path to JScript.NET compiler</summary>
        /// <value>By default points to relative path of jsc.exe depending on jsce.exe being in PATH</value>
        public string JscExe { get; set; } = "jsc.exe";
        /// <summary>Gets or sets type of targe binary</summary>
        /// <value>
        /// <list type="table">
        /// <listheader><term>Value</term><description>Description</description></listheader>
        /// <item>exe<term></term><description>Console application (default)</description></item>
        /// <item>winexe<term></term><description>Desktop (GUI) application</description></item>
        /// <item><term>library</term><description>DLL library</description></item>
        /// </list>
        /// </value>
        public string Target { get; set; }
        /// <summary>Gets or sets assembly platform</summary>
        /// <value>Possible values are: x86, Itanium, x64, any cpu (default)</value>
        public string Platform { get; set; }
        /// <summary>gets or sest value indicating if assemblies are automatically referenced based on imported namespaces and fully-qualified names</summary>
        /// <value>On by default (unless <see cref="NoStdLib"/> is true)</value>
        public bool? Autoref { get; set; }
        /// <summary>Gets or sets additional directories to search in for references</summary>
        public string[] Libraries { get; set; }
        /// <summary>Gets or sets assembly files to reference metadata from</summary>
        public string[] References { get; set; }
        /// <summary>Gets or sets Win32 resource files</summary>
        public string[] Win32Resources { get; set; }
        /// <summary>Gets or sets embeded resources</summary>
        public ResourceInfo[] Resources { get; set; }
        /// <summary>Gets or sets linked resources</summary>
        public ResourceInfo[] LinkedResources { get; set; }
        /// <summary>Gets or sets value indicating if debugging information are emitted</summary>
        public bool? Debug { get; set; }
        /// <summary>Gets or sets value indicating if some language features are turned off to allow better code generation</summary>
        public bool? Fast { get; set; }
        /// <summary>Gets or sets value indicating if all warnings are treated as errors</summary>
        public bool? WarningsAsErrors { get; set; }
        /// <summary>Gets or sets warning level</summary>
        /// <value>0-4</value>
        public int? WarningLevel { get; set; }
        /// <summary>Gets or sets defined conditional compilation symbols</summary>
        public string[] Defines { get; set; }
        /// <summary>Gets or sets value indicating if <c>print()</c> function is provided</summary>
        public bool? AllowPrintFunction { get; set; }
        /// <summary>Gets or sets value indicating if standard library (mscorlib.dll) is not imported and <see cref="Autoref"/> are changed to default off</summary>
        public bool? NoStdLib { get; set; }
        /// <summary>Gets or sets value indicating default for members not marked <c>override</c> or <c>hide</c></summary>
        public bool? VersionSafe { get; set; }
        /// <summary>gets or sets files to complie</summary>
        [Required]
        public string[] Files { get; set; }

        /// <summary>Executes a task.</summary>
        /// <returns>true if the task executed successfully; otherwise, false.</returns>
        public override bool Execute()
        {
            using (var jsc = new Process())
            {
                jsc.StartInfo.UseShellExecute = false;
                jsc.StartInfo.FileName = JscExe;
                StringBuilder cmd = new StringBuilder();
                if (!string.IsNullOrEmpty(Out)) cmd.Append($"/out:\"{Out}\" ");
                if (!string.IsNullOrEmpty(Target)) cmd.Append($"/t:\"{Target}\" ");
                if (!string.IsNullOrEmpty(Platform)) cmd.Append($"/platform:\"{Platform}\" ");
                if (Autoref.HasValue) cmd.Append($"/autoref{(Autoref.Value ? "+" : "-")} ");
                if (Libraries != null)
                    foreach (var lib in Libraries)
                        cmd.Append($"/lib:\"{lib}\" ");
                if (References != null)
                {
                    cmd.Append("/r:");
                    int i = 0;
                    foreach (var @ref in References)
                    {
                        if (i++ > 0) cmd.Append(";");
                        cmd.Append($"\"{@ref}\"");
                    }
                    cmd.Append(" ");
                }
                if (Win32Resources != null)
                    foreach (var res in Win32Resources)
                        cmd.Append($"/win32res:\"{res}\" ");
                if (Resources != null)
                {
                    foreach (var res in Resources)
                    {
                        cmd.Append($"/res:\"{res.FileName}\"");
                        if (!string.IsNullOrEmpty(res.Name) || res.Public.HasValue) cmd.Append($",\"{res.Name}\"");
                        if (res.Public.HasValue) cmd.Append("," + (res.Public.Value ? "public" : "private"));
                        cmd.Append(" ");
                    }
                }
                if (LinkedResources != null)
                {
                    foreach (var res in LinkedResources)
                    {
                        cmd.Append($"/linkres:\"{res.FileName}\"");
                        if (!string.IsNullOrEmpty(res.Name) || res.Public.HasValue) cmd.Append($",\"{res.Name}\"");
                        if (res.Public.HasValue) cmd.Append("," + (res.Public.Value ? "public" : "private"));
                        cmd.Append(" ");
                    }
                }
                if (Debug.HasValue) cmd.Append($"/debug{(Debug.Value ? "+" : "-")} ");
                if (Fast.HasValue) cmd.Append($"/fast{(Fast.Value ? "+" : "-")} ");
                if (WarningsAsErrors.HasValue) cmd.Append($"/warnaserror{(WarningsAsErrors.Value ? "+" : "-")} ");
                if (WarningLevel.HasValue) cmd.Append($"/w:{WarningLevel} ");
                if (Defines != null)
                    foreach (var symbol in Defines)
                        cmd.Append($"/d:\"{symbol}\" ");
                if (AllowPrintFunction.HasValue) cmd.Append($"/print{(AllowPrintFunction.Value ? "+" : "-")} ");
                if (NoStdLib.HasValue) cmd.Append($"/nostdlib{(NoStdLib.Value ? "+" : "-")} ");
                if (VersionSafe.HasValue) cmd.Append($"/versionsafe{(VersionSafe.Value ? "+" : "-")} ");

                cmd.Append("/nologo ");
                if(Files!=null )
                    foreach (var file in Files)
                        cmd.Append(file + " ");
                jsc.StartInfo.Arguments = cmd.ToString();
                Log.LogCommandLine(jsc + " " + jsc.StartInfo.Arguments);
                jsc.Start();
                jsc.WaitForExit();
                if (jsc.ExitCode != 0)
                    Log.LogError($"Process {System.IO.Path.GetFileName(JscExe)} exited with code {jsc.ExitCode}");
                else
                    Log.LogMessage($"Process {System.IO.Path.GetFileName(JscExe)} exited with code {jsc.ExitCode}");
                return jsc.ExitCode == 0;
            }
        }

        public struct ResourceInfo
        {
            public string FileName { get; set; }
            public string Name { get; set; }
            public bool? Public { get; set; }
        }
    }
}
