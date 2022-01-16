using hass_workstation_service.Communication.InterProcesCommunication.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        private int? port;
        private bool useTLS;
        private bool retainLWT = true;
        private string rootCaPath;
        private string clientCertPath;

        public bool IsConnected { get => isConnected; set => this.RaiseAndSetIfChanged(ref isConnected, value); }
        public string Message { get => message; set => this.RaiseAndSetIfChanged(ref message, value); }
        [Required(AllowEmptyStrings = false)]
        public string Host { get => host; set => this.RaiseAndSetIfChanged(ref host, value); }
        public string Username { get => username; set => this.RaiseAndSetIfChanged(ref username, value); }
        public string Password { get => password; set => this.RaiseAndSetIfChanged(ref password, value); }
        [Required]
        [Range(1, 65535)]
        public int? Port { get => port; set => this.RaiseAndSetIfChanged(ref port, value); }
        public bool UseTLS { get => useTLS; set => this.RaiseAndSetIfChanged(ref useTLS, value); }


        public bool RetainLWT { get => retainLWT; set => this.RaiseAndSetIfChanged(ref retainLWT, value); }

        public string RootCAPath { get => rootCaPath; set => this.RaiseAndSetIfChanged(ref rootCaPath, value); }

        public string ClientCertPath { get => clientCertPath; set => this.RaiseAndSetIfChanged(ref clientCertPath, value); }


        public void Update(MqttSettings settings)
        {
            this.Host = settings.Host;
            this.Username = settings.Username;
            this.Password = settings.Password;
            this.Port = settings.Port;
            this.UseTLS = settings.UseTLS;
            this.RetainLWT = settings.RetainLWT;
            this.RootCAPath = settings.RootCAPath;
            this.ClientCertPath = settings.ClientCertPath;
        }

        public void UpdateStatus(MqqtClientStatus status)
        {
            this.IsConnected = status.IsConnected;
            this.Message = status.Message;
        }
    }
}
