using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using hass_workstation_service.Communication.InterProcesCommunication.Models;
using hass_workstation_service.Communication.NamedPipe;
using JKang.IpcServiceFramework.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using UserInterface.Util;
using UserInterface.ViewModels;

namespace UserInterface.Views
{
    public class AddCommandDialog : Window
    {
        private readonly IIpcClient<ServiceContractInterfaces> client;
        public ComboBox comboBox { get; set; }
        public ComboBox detectionModecomboBox { get; set; }
        public AddCommandDialog()
        {
            this.InitializeComponent();
            DataContext = new AddCommandViewModel();
            this.comboBox = this.FindControl<ComboBox>("ComboBox");
            this.comboBox.Items = Enum.GetValues(typeof(AvailableCommands)).Cast<AvailableCommands>().OrderBy(v => v.ToString());
            this.comboBox.SelectedIndex = 0;

            // register IPC clients
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddNamedPipeIpcClient<ServiceContractInterfaces>("addCommand", pipeName: "pipeinternal")
                .BuildServiceProvider();

            // resolve IPC client factory
            IIpcClientFactory<ServiceContractInterfaces> clientFactory = serviceProvider
                .GetRequiredService<IIpcClientFactory<ServiceContractInterfaces>>();

            // create client
            this.client = clientFactory.CreateClient("addCommand");
        }

        public async void Save(object sender, RoutedEventArgs args)
        {
            var item = ((AddCommandViewModel)this.DataContext);
            dynamic model = new { item.Name, item.Command, item.Key};
            string json = JsonSerializer.Serialize(model);
            await this.client.InvokeAsync(x => x.AddCommand(item.SelectedType, json));
            Close();
        }

        public void ComboBoxClosed(object sender, SelectionChangedEventArgs args)
        {
            var item = ((AddCommandViewModel)this.DataContext);
            switch (this.comboBox.SelectedItem)
            {
                case AvailableCommands.CustomCommand:
                    item.Description = "This command lets you execute any command you want. It will run in a Windows Command Prompt silently. ";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Commands.md#customcommand";
                    item.ShowCommandInput = true;
                    item.ShowKeyInput = false;
                    break;
                case AvailableCommands.ShutdownCommand:
                    item.Description = "This command shuts down the PC immediately. ";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Commands.md#shutdowncommand";
                    item.ShowCommandInput = false;
                    item.ShowKeyInput = false;
                    break;
                case AvailableCommands.RestartCommand:
                    item.Description = "This command restarts the PC immediately. ";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Commands.md#restartcommand";
                    item.ShowCommandInput = false;
                    item.ShowKeyInput = false;
                    break;
                case AvailableCommands.LogOffCommand:
                    item.Description = "This command logs the current user off immediately. ";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Commands.md#logoffcommand";
                    item.ShowCommandInput = false;
                    item.ShowKeyInput = false;
                    break;
                case AvailableCommands.KeyCommand:
                    item.Description = "This command can be used to emulate a keystroke. It requires a key code which you can find by clicking the info button below.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Commands.md#keycommand";
                    item.ShowCommandInput = false;
                    item.ShowKeyInput = true;
                    break;
                case AvailableCommands.PlayPauseCommand:
                    item.Description = "This command plays or pauses currently playing media.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Commands.md#media-commands";
                    item.ShowCommandInput = false;
                    item.ShowKeyInput = false;
                    break;
                case AvailableCommands.NextCommand:
                    item.Description = "This command skips to the next media.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Commands.md#media-commands";
                    item.ShowCommandInput = false;
                    item.ShowKeyInput = false;
                    break;
                case AvailableCommands.PreviousCommand:
                    item.Description = "This command plays previous media.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Commands.md#media-commands";
                    item.ShowCommandInput = false;
                    item.ShowKeyInput = false;
                    break;
                case AvailableCommands.VolumeDownCommand:
                    item.Description = "Lowers the system volume.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Commands.md#media-commands";
                    item.ShowCommandInput = false;
                    item.ShowKeyInput = false;
                    break;
                case AvailableCommands.VolumeUpCommand:
                    item.Description = "Raises the system volume.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Commands.md#media-commands";
                    item.ShowCommandInput = false;
                    item.ShowKeyInput = false;
                    break;
                case AvailableCommands.MuteCommand:
                    item.Description = "Toggles muting the system volume.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Commands.md#media-commands";
                    item.ShowCommandInput = false;
                    item.ShowKeyInput = false;
                    break;
                default:
                    item.Description = null;
                    item.MoreInfoLink = null;
                    item.ShowCommandInput = false;
                    item.ShowKeyInput = false;
                    break;
            }
        }
        public void OpenInfo(object sender, RoutedEventArgs args)   
        {
            var item = ((AddCommandViewModel)this.DataContext);
            BrowserUtil.OpenBrowser(item.MoreInfoLink);
        }

        public void Test(object sender, RoutedEventArgs args)
        {
            var item = ((AddCommandViewModel)this.DataContext);

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/k {"echo You won't see this window normally. &&" + item.Command}";
            process.StartInfo = startInfo;
            process.Start();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
