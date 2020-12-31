using hass_workstation_service.Communication.InterProcesCommunication.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserInterface.ViewModels
{
    public class BrokerSettingsViewModel : ViewModelBase
    {
        private string host;
        private string username;
        private string password;
        private string message;
        private bool isConnected;

        public bool IsConnected { get => isConnected; set => this.RaiseAndSetIfChanged(ref isConnected, value); }
        public string Message { get => message; set => this.RaiseAndSetIfChanged(ref message, value); }
        public string Host { get => host; set => this.RaiseAndSetIfChanged(ref host, value); }
        public string Username { get => username; set => this.RaiseAndSetIfChanged(ref username, value); }
        public string Password { get => password; set => this.RaiseAndSetIfChanged(ref password, value); }
        public void Update(MqttSettings settings)
        {
            this.Host = settings.Host;
            this.Username = settings.Username;
            this.Password = settings.Password;
        }

        public void UpdateStatus(MqqtClientStatus status)
        {
            this.IsConnected = status.IsConnected;
            this.Message = status.Message;
        }
    }
}
