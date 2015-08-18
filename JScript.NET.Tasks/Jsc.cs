﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using IO = System.IO;

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

        /// <summary>Gets or sest value indicating if assemblies are automatically referenced based on imported namespaces and fully-qualified names</summary>
        /// <value>On by default (unless <see cref="NoStdLib"/> is true)</value>
        [DefaultValue(true)]
        public bool Autoref { get; set; } = true;
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
        [DefaultValue(false)]
        public bool Debug { get; set; }
        /// <summary>Gets or sets value indicating if some language features are turned off to allow better code generation</summary>
        [DefaultValue(false)]
        public bool Fast { get; set; }
        /// <summary>Gets or sets value indicating if all warnings are treated as errors</summary>
        [DefaultValue(false)]
        public bool WarningsAsErrors { get; set; }

        /// <summary>Gets or sets warning level</summary>
        /// <value>0-4</value>
        [DefaultValue(1)]
        public int WarningLevel { get; set; } = 1;
        /// <summary>Gets or sets defined conditional compilation symbols</summary>
        public string[] Defines { get; set; }
        /// <summary>Gets or sets value indicating if <c>print()</c> function is provided</summary>
        [DefaultValue(false)]
        public bool AllowPrintFunction { get; set; }
        /// <summary>Gets or sets value indicating if standard library (mscorlib.dll) is not imported and <see cref="Autoref"/> are changed to default off</summary>
        [DefaultValue(false)]
        public bool NoStdLib { get; set; }
        /// <summary>Gets or sets value indicating default for members not marked <c>override</c> or <c>hide</c></summary>
        [DefaultValue(false)]
        public bool VersionSafe { get; set; }
        /// <summary>gets or sets files to complie</summary>
        [Required]
        public string[] Files { get; set; }

        /// <summary>Executes a task.</summary>
        /// <returns>true if the task executed successfully; otherwise, false.</returns>
        public override bool Execute()
        {
            //#if DEBUG
            //            using (var currentProcess = Process.GetCurrentProcess())
            //                MessageBox.Show($"Attach to process {IO.Path.GetFileName(currentProcess.StartInfo.FileName)} PID {currentProcess.Id}");
            //#endif
            using (var jsc = new Process())
            {
                jsc.StartInfo.UseShellExecute = false;
                jsc.StartInfo.FileName = JscExe;
                StringBuilder cmd = new StringBuilder();
                if (!string.IsNullOrEmpty(Out))
                {
                    string outDir = IO.Path.GetDirectoryName(Out);
                    if (!string.IsNullOrEmpty(outDir) && !IO.Directory.Exists(outDir))
                        IO.Directory.CreateDirectory(outDir);
                    cmd.Append($"/out:\"{Out}\" ");
                }
                if (!string.IsNullOrEmpty(Target)) cmd.Append($"/t:\"{Target}\" ");
                if (!string.IsNullOrEmpty(Platform)) cmd.Append($"/platform:\"{Platform}\" ");
                cmd.Append($"/autoref{(Autoref ? "+" : "-")} ");
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
                cmd.Append($"/debug{(Debug ? "+" : "-")} ");
                cmd.Append($"/fast{(Fast ? "+" : "-")} ");
                cmd.Append($"/warnaserror{(WarningsAsErrors ? "+" : "-")} ");
                cmd.Append($"/w:{WarningLevel} ");
                if (Defines != null)
                    foreach (var symbol in Defines)
                        cmd.Append($"/d:\"{symbol}\" ");
                cmd.Append($"/print{(AllowPrintFunction ? "+" : "-")} ");
                cmd.Append($"/nostdlib{(NoStdLib ? "+" : "-")} ");
                cmd.Append($"/versionsafe{(VersionSafe ? "+" : "-")} ");

                cmd.Append("/nologo ");
                if (Files != null)
                    foreach (var file in Files)
                        cmd.Append($"\"{file}\" ");
                jsc.StartInfo.Arguments = cmd.ToString();
                Log.LogCommandLine(jsc.StartInfo.FileName + " " + jsc.StartInfo.Arguments);
                Log.LogMessage($"Running {jsc.StartInfo.FileName} {jsc.StartInfo.Arguments}");
                jsc.StartInfo.RedirectStandardError = true;
                jsc.StartInfo.RedirectStandardOutput = true;
                jsc.ErrorDataReceived += (sender, e) => { if (e.Data != null) Log.LogError(e.Data); };
                jsc.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        if (e.Data.IndexOf("error", StringComparison.InvariantCultureIgnoreCase) >= 0)
                            Log.LogError(e.Data);
                        else if (e.Data.IndexOf("warning", StringComparison.InvariantCultureIgnoreCase) >= 0)
                            Log.LogWarning(e.Data);
                        else Log.LogMessage(e.Data);
                    }
                };
                jsc.Start();
                jsc.BeginErrorReadLine();
                jsc.BeginOutputReadLine();
                jsc.WaitForExit();
                if (jsc.ExitCode != 0)
                    Log.LogError($"Process {System.IO.Path.GetFileName(jsc.StartInfo.FileName)} {jsc.StartInfo.Arguments} exited with code {jsc.ExitCode}");
                else
                    Log.LogMessage($"Process {System.IO.Path.GetFileName(jsc.StartInfo.FileName)} {jsc.StartInfo.Arguments} exited with code {jsc.ExitCode}");
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
