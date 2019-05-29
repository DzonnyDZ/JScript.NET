extern alias shell15;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Dzonny.VSLangProj;
using Microsoft.VisualStudio.Shell.Interop;
using shell15::Microsoft.VisualStudio.Shell;

namespace Dzonny.JScriptNet
{
    /// <summary>This class implements the package exposed by this assembly.</summary>
    /// <remarks>
    /// This package is required if you want to define adds custom commands (ctmenu)
    /// or localized resources for the strings that appear in the New Project and Open Project dialogs.
    /// Creating project extensions or project types does not actually require a VSPackage.
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Description("JScript.NET project system")]
    [Guid(PackageGuid)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class JScriptNetVsPackage : AsyncProjectSystemPackage
    {
        /// <summary>CTor - creates a new instance of the <see cref="JScriptNetVsPackage"/> class</summary>
        public JScriptNetVsPackage() : base("JScript.NET") { }

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