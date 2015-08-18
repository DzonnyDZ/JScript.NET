using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Debuggers;
using Microsoft.VisualStudio.ProjectSystem.Utilities;
using Microsoft.VisualStudio.ProjectSystem.Utilities.DebuggerProviders;
using Microsoft.VisualStudio.ProjectSystem.VS.Debuggers;

namespace Dzonny.JScriptNet
{
    [ExportDebugger(JSNetDebugger.SchemaName)]
    [AppliesTo(MyUnconfiguredProject.UniqueCapability)]
    public class JSNetDebuggerLaunchProvider : DebugLaunchProviderBase
    {
        [ExportPropertyXamlRuleDefinition("Dzonny.JScriptNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9be6e469bc4921f1", "XamlRuleToCode:JSNetDebugger.xaml", "Project")]
        [AppliesTo(MyUnconfiguredProject.UniqueCapability)]
        private object DebuggerXaml { get { throw new NotImplementedException(); } }

        [ImportingConstructor]
        public JSNetDebuggerLaunchProvider(ConfiguredProject configuredProject)
            : base(configuredProject)
        {
        }

        /// <summary>Gets project properties that the debugger needs to launch.</summary>
        [Import]
        private ProjectProperties DebuggerProperties { get; set; }

        /// <summary>Gets whether the debugger can launch in the current configuration.</summary>
        /// <param name="launchOptions">The launch options that would be passed to a subsequent call to <see cref="M:Microsoft.VisualStudio.ProjectSystem.Utilities.DebuggerProviders.DebugLaunchProviderBase.LaunchAsync(Microsoft.VisualStudio.ProjectSystem.Debuggers.DebugLaunchOptions)"/>.</param>
        /// <remarks>
        ///     This method may be called at any time and the implementation should be fast enough to perform well if called every time the UI is updated (potentially several times per second).
        ///     Implementers SHOULD NOT rely on this method being called directly before a call to <see cref="M:Microsoft.VisualStudio.ProjectSystem.Utilities.DebuggerProviders.DebugLaunchProviderBase.LaunchAsync(Microsoft.VisualStudio.ProjectSystem.Debuggers.DebugLaunchOptions)"/>.
        ///     No state should be saved within this method to be used by the <see cref="M:Microsoft.VisualStudio.ProjectSystem.Utilities.DebuggerProviders.DebugLaunchProviderBase.LaunchAsync(Microsoft.VisualStudio.ProjectSystem.Debuggers.DebugLaunchOptions)"/> method.
        /// </remarks>
        public override async Task<bool> CanLaunchAsync(DebugLaunchOptions launchOptions)
        {
            var properties = await this.DebuggerProperties.GetJSNetDebuggerPropertiesAsync();
            string commandValue = await properties.JSNetDebuggerCommand.GetEvaluatedValueAtEndAsync();
            return !string.IsNullOrEmpty(commandValue);
        }

        /// <summary>See <see cref="T:Microsoft.VisualStudio.ProjectSystem.VS.Debuggers.IDebugQueryTarget"/>.</summary>
        public override async Task<IReadOnlyList<IDebugLaunchSettings>> QueryDebugTargetsAsync(DebugLaunchOptions launchOptions)
        {
            var settings = new DebugLaunchSettings(launchOptions);

            // The properties that are available via DebuggerProperties are determined by the property XAML files in your project.
            var debuggerProperties = await this.DebuggerProperties.GetJSNetDebuggerPropertiesAsync();
            settings.CurrentDirectory = await debuggerProperties.JSNetDebuggerWorkingDirectory.GetEvaluatedValueAtEndAsync();
            settings.Executable = await debuggerProperties.JSNetDebuggerCommand.GetEvaluatedValueAtEndAsync();
            settings.Arguments = await debuggerProperties.JSNetDebuggerCommandArguments.GetEvaluatedValueAtEndAsync();
            settings.LaunchOperation = DebugLaunchOperation.CreateProcess;

            settings.LaunchDebugEngineGuid = DebuggerEngines.ManagedOnlyEngine;

            return new IDebugLaunchSettings[] { settings };
        }
    }
}
