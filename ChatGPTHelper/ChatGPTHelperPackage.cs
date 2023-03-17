global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;

namespace ChatGPTHelper
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.ChatGPTHelperString)]
    public sealed class ChatGPTHelperPackage : ToolkitPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.RegisterCommandsAsync();
        }

        //private void RegisterCommands()
        //{
        //    OleMenuCommandService commandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
        //    if (commandService != null)
        //    {
        //        var commandID = new CommandID(Guid.Parse(BaseCommandPackageGuid), BaseCommandId);
        //        var menuItem = new OleMenuCommand(Execute, commandID);
        //        commandService.AddCommand(menuItem);
        //    }
        //}
    }
}