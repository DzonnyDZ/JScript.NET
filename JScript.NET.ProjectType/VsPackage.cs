/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Dzonny.JScriptNet
{                  
    /// <summary>This class implements the package exposed by this assembly.</summary>
    /// <remarks>
    /// This package is required if you want to define adds custom commands (ctmenu)
    /// or localized resources for the strings that appear in the New Project and Open Project dialogs.
    /// Creating project extensions or project types does not actually require a VSPackage.
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [Description("JScript.NET project system")]
    [Guid(PackageGuid)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution )]
    public sealed class VsPackage : Package
    {

        /// <summary>Type initializer - initializes the class <see cref="VsPackage"/></summary>
        static VsPackage()
        {
            deploymentException = EnsureCustomProjectSystem();
        }

        /// <summary>In case deployment of custom project system failed, contains the exception</summary>
        private static readonly Exception deploymentException;

        /// <summary>Called when the VSPackage is loaded by Visual Studio.</summary>
        protected override void Initialize()
        {
            if (deploymentException != null)
                OnInit();
        }

        /// <summary>In case theer was error deployng local custom project system, reports the issue to user assynchronously</summary>
        /// <returns>Task to await the async operation</returns>
        private async Task OnInit()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            IVsOutputWindow outputWindow = GetService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            if (outputWindow != null)
            {
                Guid guidGeneral = VSConstants.GUID_OutWindowGeneralPane;
                IVsOutputWindowPane windowPane;
                if (ErrorHandler.Failed(outputWindow.GetPane(ref guidGeneral, out windowPane)) && (ErrorHandler.Succeeded(outputWindow.CreatePane(ref guidGeneral, null, 1, 1))))
                    outputWindow.GetPane(ref guidGeneral, out windowPane);

                string desc = ((DescriptionAttribute)GetType().GetCustomAttributes(typeof(DescriptionAttribute), true).First()).Description;

                if (windowPane != null)
                    windowPane.OutputString($"Error when deploying custom project system {desc}:\r\n{deploymentException.GetType().Name}\r\n{deploymentException.Message}\r\n{deploymentException.StackTrace}");
            }
        }

        /// <summary>Ensures that up-to-date version for custom project system is installed</summary>
        private static Exception EnsureCustomProjectSystem()
        {
            try
            {
                if (CustomProjectSystemInstaller.NeedsDeployment())
                    CustomProjectSystemInstaller.Deploy();
            }
            catch (Exception ex)
            {
                return ex;
            }
            return null;
        }

        /// <summary>The GUID for this package.</summary>
        public const string PackageGuid = "b584f11e-5e77-40e8-bbfd-f70b550504ba";

        /// <summary>The GUID for this project type.  It is unique with the project file extension and appears under the VS registry hive's Projects key.</summary>
        public const string ProjectTypeGuid = "e452ebf3-3bbb-4a96-b835-ae6ecaeab859";

        /// <summary>The file extension of this project type.  No preceding period.</summary>
        public const string ProjectExtension = "jsnproj";

        /// <summary>The default namespace this project compiles with, so that manifest resource names can be calculated for embedded resources.</summary>
        internal const string DefaultNamespace = "Dzonny.JScriptNet";
    }
}  