using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using hass_workstation_service.Communication.InterProcesCommunication.Models;
using hass_workstation_service.Communication.NamedPipe;
using JKang.IpcServiceFramework.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text.Json;
using UserInterface.Util;
using UserInterface.ViewModels;

namespace UserInterface.Views
{
    public class AddCommandDialog : Window
    {
        private readonly IIpcClient<IServiceContractInterfaces> _client;
        public ComboBox ComboBox { get; set; }
        public ComboBox DetectionModecomboBox { get; set; }
        public Guid CommandId { get; }

        public AddCommandDialog(Guid commandId) : this()
        {
            CommandId = commandId;
            GetCommandInfo(CommandId);
            Title = "Edit command";
        }

        public AddCommandDialog()
        {
            InitializeComponent();
            DataContext = new AddCommandViewModel();
            ComboBox = this.FindControl<ComboBox>("ComboBox");
            ComboBox.Items = Enum.GetValues(typeof(AvailableCommands)).Cast<AvailableCommands>().OrderBy(v => v.ToString());
            ComboBox.SelectedIndex = 0;

            // register IPC clients
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddNamedPipeIpcClient<IServiceContractInterfaces>("addCommand", pipeName: "pipeinternal")
                .BuildServiceProvider();

            // resolve IPC client factory
            IIpcClientFactory<IServiceContractInterfaces> clientFactory = serviceProvider
                .GetRequiredService<IIpcClientFactory<IServiceContractInterfaces>>();

            // create client
            _client = clientFactory.CreateClient("addCommand");
            Title = "Add command";

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void GetCommandInfo(Guid commandId)
        {
            var command = await _client.InvokeAsync(x => x.GetConfiguredCommand(commandId));

            ComboBox.SelectedItem = command.Type;
            FillDefaultValues();
            ComboBox.IsEnabled = false;
            var item = (AddCommandViewModel)DataContext;
            item.SelectedType = command.Type;
            item.Name = command.Name;
            item.Command = command.Command;
            item.Key = command.Key;
            
        }

        public async void Save(object sender, RoutedEventArgs args)
        {
            var item = (AddCommandViewModel)DataContext;
            dynamic model = new { item.Name, item.Command, item.Key };
            string json = JsonSerializer.Serialize(model);
            if (CommandId == Guid.Empty)
                await _client.InvokeAsync(x => x.AddCommand(item.SelectedType, json));
            else
                await _client.InvokeAsync(x => x.UpdateCommandById(CommandId, json));

            Close();
        }

        public void ComboBoxClosed(object sender, SelectionChangedEventArgs args)
        {
            FillDefaultValues();
        }

        private void FillDefaultValues()
        {
            var item = (AddCommandViewModel)DataContext;
            switch (ComboBox.SelectedItem)
            {
                case AvailableCommands.CustomCommand:
                    item.Description = "This command lets you execute any command you want. It will run in a Windows Command Prompt silently. ";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service#customcommand";
                    item.ShowCommandInput = true;
                    item.ShowKeyInput = false;
                    break;
                case AvailableCommands.ShutdownCommand:
                    item.Description = "This command shuts down the PC immediately. ";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service#shutdowncommand";
                    item.ShowCommandInput = false;
                    item.ShowKeyInput = false;
                    break;
                case AvailableCommands.RestartCommand:
                    item.Description = "This command restarts the PC immediately. ";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service#restartcommand";
                    item.ShowCommandInput = false;
                    item.ShowKeyInput = false;
                    break;
                case AvailableCommands.LogOffCommand:
                    item.Description = "This command logs the current user off immediately. ";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service#logoffcommand";
                    item.ShowCommandInput = false;
                    item.ShowKeyInput = false;
                    break;
                case AvailableCommands.KeyCommand:
                    item.Description = "This command can be used to emulate a keystroke. It requires a key code which you can find by clicking the info button below.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service#keycommand";
                    item.ShowCommandInput = false;
                    item.ShowKeyInput = true;
                    break;
                case AvailableCommands.PlayPauseCommand:
                    item.Description = "This command plays or pauses currently playing media.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service#media-commands";
                    item.ShowCommandInput = false;
                    item.ShowKeyInput = false;
                    break;
                case AvailableCommands.NextCommand:
                    item.Description = "This command skips to the next media.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service#media-commands";
                    item.ShowCommandInput = false;
                    item.ShowKeyInput = false;
                    break;
                case AvailableCommands.PreviousCommand:
                    item.Description = "This command plays previous media.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service#media-commands";
                    item.ShowCommandInput = false;
                    item.ShowKeyInput = false;
                    break;
                case AvailableCommands.VolumeDownCommand:
                    item.Description = "Lowers the system volume.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service#media-commands";
                    item.ShowCommandInput = false;
                    item.ShowKeyInput = false;
                    break;
                case AvailableCommands.VolumeUpCommand:
                    item.Description = "Raises the system volume.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service#media-commands";
                    item.ShowCommandInput = false;
                    item.ShowKeyInput = false;
                    break;
                case AvailableCommands.MuteCommand:
                    item.Description = "Toggles muting the system volume.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service#media-commands";
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
            var item = (AddCommandViewModel)DataContext;
            BrowserUtil.OpenBrowser(item.MoreInfoLink);
        }

        public void Test(object sender, RoutedEventArgs args)
        {
            var item = (AddCommandViewModel)DataContext;

            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal,
                FileName = "cmd.exe",
                Arguments = $"/k {"echo You won't see this window normally. &&" + item.Command}"
            };
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}