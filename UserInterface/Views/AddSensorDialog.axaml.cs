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
    public class AddSensorDialog : Window
    {
        private readonly IIpcClient<ServiceContractInterfaces> client;
        public ComboBox comboBox { get; set; }
        public AddSensorDialog()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.comboBox = this.FindControl<ComboBox>("ComboBox");
            this.comboBox.Items = Enum.GetValues(typeof(AvailableSensors)).Cast<AvailableSensors>();

            // register IPC clients
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddNamedPipeIpcClient<ServiceContractInterfaces>("addsensor", pipeName: "pipeinternal")
                .BuildServiceProvider();

            // resolve IPC client factory
            IIpcClientFactory<ServiceContractInterfaces> clientFactory = serviceProvider
                .GetRequiredService<IIpcClientFactory<ServiceContractInterfaces>>();

            // create client
            this.client = clientFactory.CreateClient("addsensor");


            DataContext = new AddSensorViewModel();
        }

        public async void Save(object sender, RoutedEventArgs args)
        {
            var item = ((AddSensorViewModel)this.DataContext);
            dynamic model = new { Name = item.Name };
            string json = JsonSerializer.Serialize(model);
            await this.client.InvokeAsync(x => x.AddSensor(item.SelectedType, json));
            Close();
        }

        public void ComboBoxClosed(object sender, SelectionChangedEventArgs args)
        {
            var item = ((AddSensorViewModel)this.DataContext);
            switch (item.SelectedType)
            {
                case AvailableSensors.UserNotificationStateSensor:
                    item.Description = "This sensor watches the UserNotificationState. This is normally used in applications to determine if it is appropriate to send a notification but we can use it to expose this state. \n ";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service#usernotificationstate";
                    break;
                case AvailableSensors.DummySensor:
                    item.Description = "This sensor spits out a random number every second. Useful for testing, maybe you'll find some other use for it.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service#dummy";
                    break;
                default:
                    item.Description = null;
                    item.MoreInfoLink = null;
                    break;
            }
        }
        public void OpenInfo(object sender, RoutedEventArgs args)
        {
            var item = ((AddSensorViewModel)this.DataContext);
            BrowserUtil.OpenBrowser(item.MoreInfoLink);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
