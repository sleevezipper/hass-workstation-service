using hass_workstation_service.Communication.InterProcesCommunication.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserInterface.ViewModels
{
    public class BackgroundServiceSettingsViewModel : ViewModelBase
    {
        private string host;
        private string username;
        private string password;
        private string message;
        private bool isRunning;

        public bool IsRunning { get => isRunning; set => this.RaiseAndSetIfChanged(ref isRunning, value); }
        public string Message { get => message; set => this.RaiseAndSetIfChanged(ref message, value); }

        public void UpdateStatus(bool isRunning, string message)
        {
            this.IsRunning = isRunning;
            this.Message = message;
        }
    }
}
