/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/
extern alias shell15;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.VS;
using shell15::Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.ProjectSystem.Utilities;
using Microsoft.VisualStudio.ProjectSystem.Designers;

namespace Dzonny.JScriptNet
{

    [Export]
    [AppliesTo(MyUnconfiguredProject.UniqueCapability)]
    [ProjectTypeRegistration(JScriptNetVsPackage.ProjectTypeGuid, "JScript.NET", "#2", ProjectExtension, Language, resourcePackageGuid: JScriptNetVsPackage.PackageGuid, PossibleProjectExtensions = ProjectExtension, ProjectTemplatesDir = @"..\..\Templates\Projects\MyCustomProject")]
    [ProvideProjectItem(JScriptNetVsPackage.ProjectTypeGuid, "My Items", @"..\..\Templates\ProjectItems\MyCustomProject", 500)]
    internal class MyUnconfiguredProject
    {
        /// <summary>
        /// The file extension used by your project type.
        /// This does not include the leading period.
        /// </summary>
        internal const string ProjectExtension = "jsnproj";

        /// <summary>
        /// A project capability that is present in your project type and none others.
        /// This is a convenient constant that may be used by your extensions so they
        /// only apply to instances of your project type.
        /// </summary>
        /// <remarks>
        /// This value should be kept in sync with the capability as actually defined in your .targets.
        /// </remarks>
        internal const string UniqueCapability = "JScript.NET";

        internal const string Language = "JScript.NET";

        [ImportingConstructor]
        public MyUnconfiguredProject(UnconfiguredProject unconfiguredProject)
        {
            this.ProjectHierarchies = new OrderPrecedenceImportCollection<IVsHierarchy>(projectCapabilityCheckProvider: unconfiguredProject);
        }

        [Import]
        internal UnconfiguredProject UnconfiguredProject { get; }

        [Import]
        internal IActiveConfiguredProjectSubscriptionService SubscriptionService { get; }

        [Import]
        internal IThreadHandling ThreadHandling { get; }

        [Import]
        internal ActiveConfiguredProject<ConfiguredProject> ActiveConfiguredProject { get; }

        [Import]
        internal ActiveConfiguredProject<MyConfiguredProject> MyActiveConfiguredProject { get; }

        [ImportMany(ExportContractNames.VsTypes.IVsProject, typeof(IVsProject))]
        internal OrderPrecedenceImportCollection<IVsHierarchy> ProjectHierarchies { get; }

        internal IVsHierarchy ProjectHierarchy
        {
            get { return this.ProjectHierarchies.Single().Value; }
        }
    }
}
